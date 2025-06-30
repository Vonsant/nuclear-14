using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Added to an entity while they are wielding an active <see cref="BulletCatcherComponent"/>.
/// Used to relay projectile events.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BulletCatcherUserComponent : Component
{
}
