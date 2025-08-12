using SLS.Core.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.Mobs
{
    public class MobProvider : IMobSkillDBMobProvider
    {
        private IUnityLogger logger = default;

        private Dictionary<int, Mob> mobs = default;

        bool IMobSkillDBMobProvider.IsLoaded => mobs.Count > 0;
        IReadOnlyDictionary<int, Mob> IMobSkillDBMobProvider.Mobs => GetMobsForMobSkillDB();

        public event UnityAction OnValueChanged;

        public MobProvider(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
            mobs = new Dictionary<int, Mob>();
        }

        bool IMobSkillDBMobProvider.TryGetMob(int mobID, out Mob mob)
        {
            return GetMobsForMobSkillDB().TryGetValue(mobID, out mob);
        }

        public void AddMob(MobData mob)
        {
            AddMobWithoutNotify(mob);
            TriggerOnValueChanged();
        }

        public void AddMobs(IEnumerable<MobData> mobs)
        {
            foreach (var mob in mobs)
            {
                AddMobWithoutNotify(mob);
            }
            TriggerOnValueChanged();
        }        

        public void Clear()
        {
            mobs.Clear();
        }

        private void AddMobWithoutNotify(MobData mob)
        {
            if (mobs.ContainsKey(mob.Id))
            {
                logger.LogDebug($"Duplicate entry for mob {mob.Id} ({mob.Name})");
            }

            mobs.TryAdd(mob.Id, new Mob
            {
                Id = mob.Id,
                Name = mob.Name,
                Class = mob.GetMobClass(),
                Model = null,
            });
        }               

        // Note: if a negative MobID is provided, the skill will be treated as 'global'
        //	-1: added for all boss types (mini and mvp)
        //	-2: added for all normal types (normal, elite, rare)
        //	-3: added for all mobs
        private IReadOnlyDictionary<int, Mob> GetMobsForMobSkillDB()
        {
            var dict = new Dictionary<int, Mob>(mobs);

            dict.TryAdd(-1, new Mob
            {
                Id = -1,
                Name = "All Boss",
                Class = Mob.EClass.Boss,
                Model = null,
            });

            dict.TryAdd(-2, new Mob
            {
                Id = -2,
                Name = "All Non-Boss",
                Class = Mob.EClass.NonBoss,
                Model = null,
            });

            dict.TryAdd(-3, new Mob
            {
                Id = -3,
                Name = "All",
                Class = Mob.EClass.All,
                Model = null,
            });

            return dict;
        }

        private void TriggerOnValueChanged()
        {
            OnValueChanged?.Invoke();
        }
    }
}
