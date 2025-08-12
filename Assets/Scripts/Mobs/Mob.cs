using UnityEngine;

namespace ROTools.Mobs
{
    public struct Mob
    {
        public int Id;
        public string Name;
        public EClass Class;
        public GameObject Model;

        public enum EClass
        {
            Unknown = 0,
            Normal = 1,
            Elite = 2,
            Rare = 3,
            Mini = 4,
            Mvp = 5,
            All = 6,
            Boss = 7,
            NonBoss = 8,
        }

        public string GetDisplayName()
        {
            return $"{Name} ({Id})";
        }
    }
}
