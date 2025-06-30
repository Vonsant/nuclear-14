using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
///     When equipped and active, this weapon absorbs incoming projectiles
///     aimed at the wielder, preventing damage.
///     Requires <see cref="ItemToggleComponent"/> to control activation.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BulletCatcherComponent : Component
{
    /// <summary>
    /// Sound played when a projectile is absorbed.
    /// </summary>
    [DataField("sound")]
    public SoundSpecifier? Sound;
}
