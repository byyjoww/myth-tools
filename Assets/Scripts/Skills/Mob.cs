using UnityEngine;

namespace ROTools.Skills
{
    public struct Mob
    {
        public int ID;
        public string Name;
        public GameObject Model;

        public string GetDisplayName()
        {
            return $"{Name} ({ID})";
        }
    }
}