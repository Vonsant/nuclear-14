using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Added to a user while they are holding a bullet absorber so that bullet events can be routed to them.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BulletAbsorberUserComponent : Component
{
}
