using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Components;
using System.Numerics;

namespace Content.Shared.Weapons.Ranged.Systems;

/// <summary>
/// Handles absorbing projectiles and hitscan shots for users wielding a <see cref="BulletAbsorberComponent"/>.
/// </summary>
public sealed class BulletAbsorberSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BulletAbsorberComponent, GotEquippedHandEvent>(OnEquippedHand);
        SubscribeLocalEvent<BulletAbsorberComponent, GotUnequippedHandEvent>(OnUnequippedHand);

        SubscribeLocalEvent<BulletAbsorberUserComponent, ProjectileReflectAttemptEvent>(OnProjectile);
        SubscribeLocalEvent<BulletAbsorberUserComponent, HitScanReflectAttemptEvent>(OnHitscan);
    }

    private void OnEquippedHand(EntityUid uid, BulletAbsorberComponent component, GotEquippedHandEvent args)
    {
        EnsureComp<BulletAbsorberUserComponent>(args.User);
    }

    private void OnUnequippedHand(EntityUid uid, BulletAbsorberComponent component, GotUnequippedHandEvent args)
    {
        RefreshUser(args.User);
    }

    private void RefreshUser(EntityUid user)
    {
        foreach (var ent in _inventory.GetHandOrInventoryEntities(user, SlotFlags.HANDS))
        {
            if (HasComp<BulletAbsorberComponent>(ent))
            {
                EnsureComp<BulletAbsorberUserComponent>(user);
                return;
            }
        }

        RemCompDeferred<BulletAbsorberUserComponent>(user);
    }

    private void OnProjectile(EntityUid uid, BulletAbsorberUserComponent component, ref ProjectileReflectAttemptEvent args)
    {
        // Delete the projectile and prevent it from damaging the user.
        QueueDel(args.ProjUid);
        args.Cancelled = true;
    }

    private void OnHitscan(EntityUid uid, BulletAbsorberUserComponent component, ref HitScanReflectAttemptEvent args)
    {
        // Absorb the hitscan by marking it as reflected with no direction.
        args.Direction = Vector2.Zero;
        args.Reflected = true;
    }
}
