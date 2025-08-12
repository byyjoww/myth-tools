using ROTools.Utils;
using System;
using System.Collections.Generic;

namespace ROTools.Mobs
{
    public class MobData
    {
        public class Drop
        {
            public string Item { get; set; }
            public int Rate { get; set; }
            public string RandomOptionGroup { get; set; }
            public bool? StealProtected { get; set; }
        }

        public enum EClass
        {
            Unknown = 0,
            Normal = 1,
            Elite = 2,
            Rare = 3,
            Mini = 4,
            Boss = 5,
        }

        public int Id { get; set; }
        public string AegisName { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public bool Omnibook { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int BaseExp { get; set; }
        public int JobExp { get; set; }
        public int Attack { get; set; }
        public int Attack2 { get; set; }
        public int Defense { get; set; }
        public int MagicDefense { get; set; }
        public int Str { get; set; }
        public int Agi { get; set; }
        public int Vit { get; set; }
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Luk { get; set; }
        public int AttackRange { get; set; }
        public int SkillRange { get; set; }
        public int ChaseRange { get; set; }
        public string Size { get; set; }
        public string Race { get; set; }
        public string Element { get; set; }
        public int ElementLevel { get; set; }
        public int WalkSpeed { get; set; }
        public int AttackDelay { get; set; }
        public int AttackMotion { get; set; }
        public int DamageMotion { get; set; }
        public string Ai { get; set; }
        public Dictionary<string, string> Modes { get; set; }
        public List<Drop> Drops { get; set; }

        public EClass GetClass()
        {
            string mobClass = Class != string.Empty
                ? Class
                : EClass.Normal.ToString();

            var parsedClass = EnumExtensions.ParseEnumIgnoringCaseOrDefault<EClass>(mobClass);
            if (parsedClass == EClass.Boss)
            {
                return TryGetMode("mvp", out bool isMvp) && isMvp
                    ? EClass.Boss
                    : EClass.Mini;
            }

            return parsedClass;
        }

        public bool TryGetMode(string key, out bool value)
        {
            value = false;
            if (Modes == null) { return false; }
            return Modes.TryGetValue(key, out string modeString) && bool.TryParse(modeString, out value);
        }

        public Mob.EClass GetMobClass()
        {
            return (Mob.EClass)GetClass();
        }
    }
}