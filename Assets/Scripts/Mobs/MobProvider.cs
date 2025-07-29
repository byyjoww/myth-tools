using SLS.Core.Logging;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ROTools.Mobs
{
    public class MobProvider
    {
        private IUnityLogger logger = default;

        private Dictionary<int, Mob> mobs = default;

        public bool IsLoaded => mobs.Count > 0;
        public IReadOnlyDictionary<int, Mob> Mobs => mobs;

        public event UnityAction OnValueChanged;

        public MobProvider(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
            mobs = new Dictionary<int, Mob>();
        }

        public void AddMob(int mobID, string mobName)
        {
            AddMobWithoutNotify(mobID, mobName);
            TriggerOnValueChanged();
        }

        public void AddMobs(IEnumerable<KeyValuePair<int, string>> mobs)
        {
            foreach (var kvp in mobs)
            {
                AddMobWithoutNotify(kvp.Key, kvp.Value);
            }
            TriggerOnValueChanged();
        }

        private void AddMobWithoutNotify(int mobID, string mobName)
        {
            mobs.TryAdd(mobID, new Mob
            {
                ID = mobID,
                Name = mobName,
                Model = null,
            });
        }

        public bool TryGetMob(int mobID, out Mob mob)
        {
            return mobs.TryGetValue(mobID, out mob);
        }

        public void Clear()
        {
            mobs.Clear();
        }

        private void TriggerOnValueChanged()
        {
            OnValueChanged?.Invoke();
        }
    }
}
