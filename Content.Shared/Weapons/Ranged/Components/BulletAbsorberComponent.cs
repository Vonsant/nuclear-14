using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// When equipped, this item allows the user to absorb incoming bullets instead of being hit.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BulletAbsorberComponent : Component
{
}
