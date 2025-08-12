using ROTools.Mobs;
using SLS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROTools.Skills
{
    public class MobSkillDBValidator
    {
        private IUnityLogger logger = default;

        private Dictionary<SkillData.ETargetType, HashSet<MobSkillData.MobTarget>> targetMapping = default;

        public MobSkillDBValidator(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
            targetMapping = CreateTargetMapping();
        }

        public void Validate(IReadOnlyDictionary<int, Mob> mobs, IReadOnlyDictionary<int, Skill> skills, IEnumerable<MobSkillData> mobSkills)
        {
            foreach (var ms in mobSkills)
            {
                bool mobExists = mobs.TryGetValue(ms.MobID, out var mob);
                if (!mobExists)
                {
                    logger.LogError($"[VALIDATION] Mob with id {ms.MobID} doesn't exist");
                }

                bool skillExists = skills.TryGetValue(ms.SkillID, out var skill);
                if (!skillExists)
                {
                    logger.LogError($"[VALIDATION] Skill with id {ms.SkillID} doesn't exist");
                }

                var skillTargetType = skill.Target;
                bool containsMapping = targetMapping.TryGetValue(skillTargetType, out var validTargets);
                if (!containsMapping)
                {
                    logger.LogError($"[VALIDATION] Mapping for skill target {skillTargetType}  doesn't exist");
                }

                if (containsMapping && !validTargets.Contains(ms.Target))
                {
                    logger.LogError($"[VALIDATION] Invalid target type {ms.Target} for skill {ms.SkillID} ({skill.Name}) with skill target type {skillTargetType}");
                }
            }
        }

        private Dictionary<SkillData.ETargetType, HashSet<MobSkillData.MobTarget>> CreateTargetMapping()
        {
            var attackTargets = new HashSet<MobSkillData.MobTarget>()
            {
                MobSkillData.MobTarget.Target,
                MobSkillData.MobTarget.RandomTarget,
                MobSkillData.MobTarget.Role,
            };

            var supportTargets = new HashSet<MobSkillData.MobTarget>()
            {
                MobSkillData.MobTarget.Friend,
                MobSkillData.MobTarget.FriendTarget,
                MobSkillData.MobTarget.Master,
                MobSkillData.MobTarget.Summoner,
                MobSkillData.MobTarget.Self,
            };

            var selfTargets = new HashSet<MobSkillData.MobTarget>()
            {
                MobSkillData.MobTarget.Self,
            };

            var groundTargets = new HashSet<MobSkillData.MobTarget>()
            {
                // attack
                MobSkillData.MobTarget.Target,
                MobSkillData.MobTarget.RandomTarget,
                MobSkillData.MobTarget.Role,

                // support
                MobSkillData.MobTarget.Friend,
                MobSkillData.MobTarget.FriendTarget,
                MobSkillData.MobTarget.Master,
                MobSkillData.MobTarget.Summoner,

                // self
                MobSkillData.MobTarget.Self,

                // ground
                MobSkillData.MobTarget.Around,
                MobSkillData.MobTarget.Around1,
                MobSkillData.MobTarget.Around2,
                MobSkillData.MobTarget.Around3,
                MobSkillData.MobTarget.Around4,
                MobSkillData.MobTarget.Around5,
                MobSkillData.MobTarget.Around6,
                MobSkillData.MobTarget.Around7,
                MobSkillData.MobTarget.Around8,
            };

            return new Dictionary<SkillData.ETargetType, HashSet<MobSkillData.MobTarget>>
            {
                { SkillData.ETargetType.Unknown, new HashSet<MobSkillData.MobTarget>() },
                { SkillData.ETargetType.Attack, attackTargets },
                { SkillData.ETargetType.Support, supportTargets },
                { SkillData.ETargetType.Self, selfTargets },
                { SkillData.ETargetType.Ground, groundTargets },
            };
        }
    }
}