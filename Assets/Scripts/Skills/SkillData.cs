using System;
using System.Collections.Generic;

namespace ROTools.Skills
{
    public class SkillData
    {
        public class DataSkillFlags
        {
            public bool AfterCastAdelay { get; set; }
            public bool TargetTrap { get; set; }
        }

        public class DataDamageFlags
        {
            public bool Splash { get; set; }
        }

        public class DataCopyFlags
        {
            public SkillCopyFlags Skill { get; set; }
        }

        public class SkillCopyFlags
        {
            public bool Plagiarism { get; set; }
            public bool Reproduce { get; set; }
            public bool Simulation { get; set; }
        }

        public class LevelArea
        {
            public int Level { get; set; }
            public int Area { get; set; }
        }

        public class LevelTime
        {
            public int Level { get; set; }
            public int Time { get; set; }
        }

        public class SkillRequirements
        {
            public List<LevelAmount> SpCost { get; set; }
            public List<LevelAmount> HpCost { get; set; }
        }

        public class LevelAmount
        {
            public int Level { get; set; }
            public int Amount { get; set; }
        }

        public enum ETargetType
        {
            Unknown,
            Attack, // target enemy
            Support, // target ally
            Self,
            Ground,
        }

        public enum EDamageType 
        {
            Unknown,
            Magic,
            Weapon,
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxLevel { get; set; }
        public string Type { get; set; }
        public string TargetType { get; set; }
        public string Hit { get; set; }
        // public int HitCount { get; set; }
        // public string Element { get; set; }
        public string Status { get; set; }
        // public DataSkillFlags SkillFlags { get; set; }
        // public DataDamageFlags DamageFlags { get; set; }
        // public DataCopyFlags CopyFlags { get; set; }
        // public List<LevelArea> SplashArea { get; set; }
        // public int Knockback { get; set; }
        // public List<LevelTime> Duration2 { get; set; }
        // public int Cooldown { get; set; }
        // public int AfterCastActDelay { get; set; }
        // public SkillRequirements Requires { get; set; }

        public ETargetType GetTargetType()
        {
            return Enum.TryParse(TargetType, out ETargetType tt) 
                ? tt 
                : ETargetType.Unknown;
        }

        public EDamageType GetDamageType()
        {
            return Enum.TryParse(TargetType, out EDamageType dt)
                ? dt
                : EDamageType.Unknown;
        }
    }
}