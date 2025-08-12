using SLS.Core.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.Skills
{
    public class SkillProvider
    {
        private IUnityLogger logger = default;

        private Dictionary<int, Skill> skills = default;
        private Dictionary<int, int> skillIndexToID = default;
        private Dictionary<int, (Skill Skill, int SkillIndex)> skillIDToSkillAndIndex = default;

        public bool IsLoaded => skills.Count > 0;
        public IReadOnlyDictionary<int, Skill> Skills => skills;
        public IReadOnlyDictionary<int, (Skill Skill, int SkillIndex)> AllSkillOptions => skillIDToSkillAndIndex;

        public event UnityAction OnValueChanged;

        public SkillProvider(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
            skills = new Dictionary<int, Skill>();
        }

        public void AddSkill(SkillData skill)
        {
            AddSkillWithoutNotify(skill);
            RebuildSkillDictionaries();
            TriggerOnValueChanged();
        }

        public void AddSkills(IEnumerable<SkillData> skills)
        {
            foreach (var skill in skills)
            {
                AddSkillWithoutNotify(skill);
            }

            RebuildSkillDictionaries();
            TriggerOnValueChanged();
        }

        private void AddSkillWithoutNotify(SkillData skill)
        {
            if (skills.ContainsKey(skill.Id))
            {
                logger.LogDebug($"Duplicate entry for skill {skill.Id} ({skill.Name})");
            }

            skills.TryAdd(skill.Id, new Skill
            {
                Id = skill.Id,
                Name = skill.Name,
                Target = skill.GetTargetType(),                
            });
        }

        public bool TryGetSkill(int skillID, out Skill skill)
        {
            return skills.TryGetValue(skillID, out skill);
        }

        public bool TryGetSkillByIndex(int skillIndex, out Skill skill)
        {
            skill = null;
            return skillIndexToID.TryGetValue(skillIndex, out int skillID)
                && skills.TryGetValue(skillID, out skill);
        }

        public void Clear()
        {
            skills.Clear();
            skillIndexToID = null;
            skillIDToSkillAndIndex = null;
        }

        private void RebuildSkillDictionaries()
        {
            var orderedSkillOpts = skills
                .OrderBy(x => x.Key)
                .Select((x, i) => (skillID: x.Key, index: i, skillName: x.Value));

            skillIndexToID = orderedSkillOpts.ToDictionary(x => x.index, y => y.skillID);
            skillIDToSkillAndIndex = orderedSkillOpts.ToDictionary(x => x.skillID, y => (y.skillName, y.index));
        }

        private void TriggerOnValueChanged()
        {
            OnValueChanged?.Invoke();
        }
    }
}