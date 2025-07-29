using ROTools.Mobs;
using SLS.Core.Extensions;
using SLS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.Skills
{
    public class SkillEditor
    {
        private IUnityLogger logger = default;
        private MobProvider mobProvider = default;
        private SkillProvider skillProvider = default;

        private Dictionary<Guid, MobSkillData> mobSkillData = default;
        private Dictionary<int, HashSet<Guid>> mobIDToMobSkillDataID = default;

        private Dictionary<MobSkillData.MobState, (string StateName, int StateIndex)> allStateOptions;
        private Dictionary<MobSkillData.MobTarget, (string TargetName, int TargetIndex)> allTargetOptions;
        private Dictionary<MobSkillData.SkillCondition, (string ConditionName, int ConditionIndex)> allConditionOptions;

        public bool IsLoaded => mobSkillData.Count > 0;
        public IReadOnlyDictionary<Guid, MobSkillData> MobSkillData => mobSkillData;
        public IReadOnlyDictionary<MobSkillData.MobState, (string StateName, int StateIndex)> AllStateOptions => allStateOptions;
        public IReadOnlyDictionary<MobSkillData.MobTarget, (string TargetName, int TargetIndex)> AllTargetOptions => allTargetOptions;
        public IReadOnlyDictionary<MobSkillData.SkillCondition, (string ConditionName, int ConditionIndex)> AllConditionOptions => allConditionOptions;

        public event UnityAction<Guid> OnMobSkillAdded;
        public event UnityAction<Guid> OnMobSkillChanged;
        public event UnityAction<Guid> OnMobSkillRemoved;
        public event UnityAction<int> OnMobSkillsCleared;
        public event UnityAction OnValueChanged;

        public SkillEditor(IUnityLogger logger, MobProvider mobProvider, SkillProvider skillProvider)
        {
            this.logger = new UnityLoggerWrapper(logger);
            this.mobProvider = mobProvider;
            this.skillProvider = skillProvider;

            mobSkillData = new Dictionary<Guid, MobSkillData>();
            mobIDToMobSkillDataID = new Dictionary<int, HashSet<Guid>>();

            allStateOptions = GetEnumValues<MobSkillData.MobState>()
               .OrderBy(x => (int)x)
               .ToDictionary(x => x, y => (y.ToString(), (int)y));
            allTargetOptions= GetEnumValues<MobSkillData.MobTarget>()
                .OrderBy(x => (int)x)
                .ToDictionary(x => x, y => (y.ToString(), (int)y));
            allConditionOptions = GetEnumValues<MobSkillData.SkillCondition>()
                .OrderBy(x => (int)x)
                .ToDictionary(x => x, y => (y.ToString(), (int)y));
        }

        public MobSkillData[] GetMobSkillData(int mobID)
        {
            var mobData = new List<MobSkillData>();
            if (mobIDToMobSkillDataID.TryGetValue(mobID, out HashSet<Guid> skillIDs))
            {
                foreach (Guid id in skillIDs)
                {
                    if (mobSkillData.TryGetValue(id, out MobSkillData skill))
                    {
                        mobData.Add(skill);
                    }
                }
            }

            return mobData
                .OrderBy(x => x.SkillID)                
                .ThenBy(x => x.InstanceID)
                .ToArray();
        }

        public void AddMobSkillData(int mobID, int skillID)
        {
            if (mobProvider.TryGetMob(mobID, out var mob) && skillProvider.TryGetSkill(skillID, out Skill skill))
            {
                var data = Skills.MobSkillData.Default;
                data.MobID = mob.ID;
                data.Description = $"{mob.Name}@{skill.Name}";
                data.SkillID = skillID;
                data.InstanceID = data.GetGuid();
                AddMobSkillData(data);                
            }
        }

        public void AddMobSkillData(IEnumerable<MobSkillData> data)
        {
            foreach (var d in data)
            {
                AddMobSkillDataWithoutNotify(d);
            }

            OnValueChanged?.Invoke();
        }

        public void AddMobSkillData(MobSkillData data)
        {
            AddMobSkillDataWithoutNotify(data);
            OnMobSkillAdded?.Invoke(data.InstanceID);
            OnValueChanged?.Invoke();
        }

        private void AddMobSkillDataWithoutNotify(MobSkillData data)
        {
            mobIDToMobSkillDataID.TryAdd(data.MobID, new HashSet<Guid>());
            mobIDToMobSkillDataID[data.MobID].Add(data.InstanceID);
            mobSkillData.Add(data.InstanceID, data);
        }

        public void UpdateSkillID(Guid instanceID, int skillIndex)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data) && skillProvider.TryGetSkillByIndex(skillIndex, out Skill skill))
            {
                data.SkillID = skill.ID;
                data.UpdateDescriptionSkillName(skill.Name);
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillLevel(Guid instanceID, int level)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.SkillLevel = level;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillRate(Guid instanceID, int rate)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Rate = rate;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillCastTime(Guid instanceID, int castTime)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.CastTime = castTime;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillDelay(Guid instanceID, int delay)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Delay = delay;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillState(Guid instanceID, int stateIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.State = (MobSkillData.MobState)stateIndex;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillTarget(Guid instanceID, int targetIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.Target = (MobSkillData.MobTarget)targetIndex;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillCondition(Guid instanceID, int conditionIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.Condition = (MobSkillData.SkillCondition)conditionIndex;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillConditionValue(Guid instanceID, int conditionValue)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.ConditionValue = conditionValue;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillEmotion(Guid instanceID, int emotion)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Emotion = emotion;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillChat(Guid instanceID, int chat)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Chat = chat;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillValue(Guid instanceID, int index, int value)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Values[index] = value;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void UpdateSkillExtra(Guid instanceID, int index, int extra)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Extras[index] = extra;
                OnMobSkillChanged?.Invoke(instanceID);
                OnValueChanged?.Invoke();
            }
        }

        public void DeleteSkill(Guid instanceID)
        {
            mobSkillData.Remove(instanceID);
            OnMobSkillRemoved?.Invoke(instanceID);
            OnValueChanged?.Invoke();
        }

        public void ClearSkills(int mobID)
        {
            var ids = mobIDToMobSkillDataID[mobID];
            ids.ForEach(x => DeleteSkill(x));
            OnMobSkillsCleared?.Invoke(mobID);
            OnValueChanged?.Invoke();
        }

        public void Clear()
        {
            mobIDToMobSkillDataID.Clear();
            mobSkillData.Clear();
            OnValueChanged?.Invoke();
        }

        private T[] GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        private bool TryGetMobSkillData(Guid instanceID, out MobSkillData data)
        {
            if (mobSkillData.TryGetValue(instanceID, out data))
            {
                return true;
            }

            logger.LogError($"Failed to get mob skill data for id '{instanceID}'");
            data = null;
            return false;
        }
    }
}