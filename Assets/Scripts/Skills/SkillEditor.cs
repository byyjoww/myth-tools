using SLS.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.Skills
{
    public class SkillEditor
    {
        private IUnityLogger logger = default;
        private ISkillDBParser parser = default;
        private ISkillDBBuilder builder = default;

        // old file data
        private string[] headers = default;
        private string path = default;

        // mob data
        private Dictionary<Guid, MobSkillData> mobSkillData = default;
        private Dictionary<int, HashSet<Guid>> mobSkillDataForMob = default;
        private Mob[] allMobs = default;

        // skills       
        private Dictionary<int, string> skillIDToName = default;
        private Dictionary<int, int> skillIndexToID = default;
        private Dictionary<int, (string SkillName, int SkillIndex)> skillIDToNameAndIndex = default;

        // enums
        private Dictionary<MobSkillData.MobState, (string StateName, int StateIndex)> allStateOptions;
        private Dictionary<MobSkillData.MobTarget, (string TargetName, int TargetIndex)> allTargetOptions;
        private Dictionary<MobSkillData.SkillCondition, (string ConditionName, int ConditionIndex)> allConditionOptions;

        public bool IsLoaded => mobSkillDataForMob.Count > 0;
        public Mob[] AllMobs => allMobs;
        public IReadOnlyDictionary<Guid, MobSkillData> AllMobData => mobSkillData;
        public IReadOnlyDictionary<int, (string SkillName, int SkillIndex)> AllSkillOptions => skillIDToNameAndIndex;
        public IReadOnlyDictionary<MobSkillData.MobState, (string StateName, int StateIndex)> AllStateOptions => allStateOptions;
        public IReadOnlyDictionary<MobSkillData.MobTarget, (string TargetName, int TargetIndex)> AllTargetOptions => allTargetOptions;
        public IReadOnlyDictionary<MobSkillData.SkillCondition, (string ConditionName, int ConditionIndex)> AllConditionOptions => allConditionOptions;

        public event UnityAction OnSkillDBLoaded;
        public event UnityAction<Guid> OnSkillChanged;

        public SkillEditor(IUnityLogger logger, ISkillDBParser parser, ISkillDBBuilder builder)
        {
            this.logger = new UnityLoggerWrapper(logger);
            this.parser = parser;
            this.builder = builder;

            mobSkillData = new Dictionary<Guid, MobSkillData>();
            mobSkillDataForMob = new Dictionary<int, HashSet<Guid>>();
            skillIDToName = new Dictionary<int, string>();

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

        public MobSkillData[] GetMobSkills(int mobID)
        {
            var mobData = new List<MobSkillData>();
            if (mobSkillDataForMob.TryGetValue(mobID, out HashSet<Guid> skillIDs))
            {
                foreach (Guid id in skillIDs)
                {
                    if (mobSkillData.TryGetValue(id, out MobSkillData skill))
                    {
                        mobData.Add(skill);
                    }
                }
            }
            else
            {
                logger.LogError($"No skills in list for mob {mobID}");
            }

            return mobData
                .OrderBy(x => x.SkillID)
                .ThenBy(x => x.InstanceID)
                .ToArray();
        }

        public void UpdateSkillID(Guid instanceID, int skillIndex)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data) && skillIndexToID.TryGetValue(skillIndex, out int skillID))
            {
                data.SkillID = skillID;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillLevel(Guid instanceID, int level)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.SkillLevel = level;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillRate(Guid instanceID, int rate)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Rate = rate;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillCastTime(Guid instanceID, int castTime)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.CastTime = castTime;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillDelay(Guid instanceID, int delay)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Delay = delay;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillState(Guid instanceID, int stateIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.State = (MobSkillData.MobState)stateIndex;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillTarget(Guid instanceID, int targetIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.Target = (MobSkillData.MobTarget)targetIndex;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillCondition(Guid instanceID, int conditionIndex)
        {
            if (TryGetMobSkillData(instanceID, out var data))
            {
                data.Condition = (MobSkillData.SkillCondition)conditionIndex;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillConditionValue(Guid instanceID, int conditionValue)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.ConditionValue = conditionValue;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillEmotion(Guid instanceID, int emotion)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Emotion = emotion;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillChat(Guid instanceID, int chat)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Chat = chat;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillValue(Guid instanceID, int index, int value)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Values[index] = value;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void UpdateSkillExtra(Guid instanceID, int index, int extra)
        {
            if (TryGetMobSkillData(instanceID, out MobSkillData data))
            {
                data.Extras[index] = extra;
                OnSkillChanged?.Invoke(instanceID);
            }
        }

        public void Load(string[] paths)
        {
            Clear();

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                path = paths[0];

                string content = File.ReadAllText(path);
                var parsedContent = parser.Parse(content);
                headers = parsedContent.headers;
                MobSkillData[] skillData = parsedContent.data;
                foreach (MobSkillData msd in skillData)
                {
                    mobSkillDataForMob.TryAdd(msd.MobID, new HashSet<Guid>());
                    mobSkillDataForMob[msd.MobID].Add(msd.InstanceID);
                    mobSkillData.TryAdd(msd.InstanceID, msd);
                    skillIDToName.TryAdd(msd.SkillID, msd.GetDescriptionSkillName());
                }

                // build mobs
                allMobs = mobSkillDataForMob.Keys
                    .Select(x => CreateMob(x))
                    .OrderBy(x => x.ID)
                    .ToArray();

                // build skill options
                var orderedSkillOpts = skillIDToName
                    .OrderBy(x => x.Key)
                    .Select((x, i) => (skillID: x.Key, index: i, skillName: x.Value));

                skillIndexToID = orderedSkillOpts.ToDictionary(x => x.index, y => y.skillID);
                skillIDToNameAndIndex = orderedSkillOpts.ToDictionary(x => x.skillID, y => (y.skillName, y.index));

                logger.LogInfo($"Loaded content from path {path}:\n{content}");
                // logger.LogInfo($"Created DB:\n[{string.Join(",", mobSkillData.Values.SelectMany(x => x).Select(x => $"{x.MobID}:{x.SkillID}").ToArray())}]");
            }

            OnSkillDBLoaded?.Invoke();
        }

        public void Save()
        {
            string directory = Path.GetDirectoryName(path);
            string oldFileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            string newFilePath;
            int counter = 1;

            do
            {
                string newFilename = $"{oldFileName}({counter}){extension}";
                newFilePath = Path.Combine(directory, newFilename);
                counter++;
            }
            while (File.Exists(newFilePath));

            MobSkillData[] data = mobSkillData.Values
                .OrderBy(x => x.MobID)
                .ThenBy(x => x.SkillID)
                .ThenBy(x => (int)x.State)
                .ThenBy(x => x.InstanceID)
                .ToArray();            
            
            string content = builder.Build(headers, data);
            File.WriteAllText(newFilePath, content);

            logger.LogInfo($"Saving content at path {newFilePath}:\n{content}");
        }

        public void Clear()
        {
            path = string.Empty;

            mobSkillDataForMob.Clear();
            mobSkillData.Clear();
            allMobs = null;

            skillIDToName.Clear();
            skillIndexToID = null;
            skillIDToNameAndIndex = null;
        }

        private Mob CreateMob(int mobID)
        {
            string name = $"{mobID}";
            if (mobSkillDataForMob.TryGetValue(mobID, out HashSet<Guid> skillIDs) && skillIDs.Count > 0)
            {
                Guid firstID = skillIDs.FirstOrDefault();
                MobSkillData skill = mobSkillData[firstID];
                name = skill.GetDescriptionMobName();
            }

            return new Mob
            {
                ID = mobID,
                Name = name,
                Model = null,
            };
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