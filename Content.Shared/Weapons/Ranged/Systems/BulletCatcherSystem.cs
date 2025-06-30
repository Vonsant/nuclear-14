using System.Numerics;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Ranged.Systems;

/// <summary>
/// Handles <see cref="BulletCatcherComponent"/> logic and absorbs projectiles
/// that would hit the wielder.
/// </summary>
public sealed class BulletCatcherSystem : EntitySystem
{
    [Dependency] private readonly ItemToggleSystem _toggle = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BulletCatcherUserComponent, ProjectileReflectAttemptEvent>(OnProjectileAttempt);
        SubscribeLocalEvent<BulletCatcherUserComponent, HitScanReflectAttemptEvent>(OnHitscanAttempt);

        SubscribeLocalEvent<BulletCatcherComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<BulletCatcherComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<BulletCatcherComponent, GotEquippedHandEvent>(OnHandEquipped);
        SubscribeLocalEvent<BulletCatcherComponent, GotUnequippedHandEvent>(OnHandUnequipped);
        SubscribeLocalEvent<BulletCatcherComponent, ItemToggledEvent>(OnToggle);
    }

    private void OnProjectileAttempt(EntityUid uid, BulletCatcherUserComponent comp, ref ProjectileReflectAttemptEvent args)
    {
        foreach (var item in _inventory.GetHandOrInventoryEntities(uid, SlotFlags.All & ~SlotFlags.POCKET))
        {
            if (!TryAbsorbProjectile(item, args.ProjUid))
                continue;

            args.Cancelled = true;
            break;
        }
    }

    private void OnHitscanAttempt(EntityUid uid, BulletCatcherUserComponent comp, ref HitScanReflectAttemptEvent args)
    {
        foreach (var item in _inventory.GetHandOrInventoryEntities(uid, SlotFlags.All & ~SlotFlags.POCKET))
        {
            if (!TryAbsorbHitscan(item))
                continue;

            args.Direction = Vector2.Zero;
            args.Reflected = true;
            break;
        }
    }

    private bool TryAbsorbProjectile(EntityUid item, EntityUid projectile)
    {
        if (!TryComp(item, out BulletCatcherComponent catcher) || !_toggle.IsActivated(item))
            return false;

        if (catcher.Sound != null)
            _audio.PlayPvs(catcher.Sound, item);

        QueueDel(projectile);
        return true;
    }

    private bool TryAbsorbHitscan(EntityUid item)
    {
        if (!TryComp(item, out BulletCatcherComponent catcher) || !_toggle.IsActivated(item))
            return false;

        if (catcher.Sound != null)
            _audio.PlayPvs(catcher.Sound, item);

        return true;
    }

    private void OnEquipped(EntityUid uid, BulletCatcherComponent component, GotEquippedEvent args)
    {
        if (_timing.ApplyingState)
            return;

        EnsureComp<BulletCatcherUserComponent>(args.Equipee);
    }

    private void OnUnequipped(EntityUid uid, BulletCatcherComponent component, GotUnequippedEvent args)
    {
        RefreshUser(args.Equipee);
    }

    private void OnHandEquipped(EntityUid uid, BulletCatcherComponent component, GotEquippedHandEvent args)
    {
        if (_timing.ApplyingState)
            return;

        EnsureComp<BulletCatcherUserComponent>(args.User);
    }

    private void OnHandUnequipped(EntityUid uid, BulletCatcherComponent component, GotUnequippedHandEvent args)
    {
        RefreshUser(args.User);
    }

    private void OnToggle(EntityUid uid, BulletCatcherComponent component, ref ItemToggledEvent args)
    {
        if (args.User is {} user)
            RefreshUser(user);
    }

    private void RefreshUser(EntityUid user)
    {
        foreach (var ent in _inventory.GetHandOrInventoryEntities(user, SlotFlags.All & ~SlotFlags.POCKET))
        {
            if (HasComp<BulletCatcherComponent>(ent) && _toggle.IsActivated(ent))
            {
                EnsureComp<BulletCatcherUserComponent>(user);
                return;
            }
        }

        RemCompDeferred<BulletCatcherUserComponent>(user);
    }
}
