using System.Collections.Generic;
using UnityEngine.Events;

namespace ROTools.Mobs
{
    public interface IMobSkillDBMobProvider
    {
        bool IsLoaded { get; }
        IReadOnlyDictionary<int, Mob> Mobs { get; }

        event UnityAction OnValueChanged;

        bool TryGetMob(int mobID, out Mob mob);
    }
}
