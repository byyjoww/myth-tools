using System;
using System.Linq;

namespace ROTools.Skills
{
    public class MobSkillData
    {
        public const int NUM_OF_ARGS = 35;

        public Guid InstanceID;
        public int MobID;
        public string Description;
        public int SkillID;
        public int SkillLevel;
        public int Rate;
        public int CastTime;
        public int Delay;
        public bool Cancelable;
        public int Emotion;
        public int Chat;

        // Requirements
        public MobState State;
        public MobTarget Target;
        public SkillCondition Condition;
        public int ConditionValue;

        public int[] Values;
        public int[] Extras;

        public enum MobState
        {
            Any,
            AnyTarget,
            Dead,
            Loot,
            Idle,
            Walk,
            Attack,
            Angry,
            Chase,
            Follow,
        }

        public enum MobTarget
        {
            Target,
            RandomTarget,
            Self,
            Friend,
            FriendTarget,
            Master,
            Summoner,
            Around,
            Around1,
            Around2,
            Around3,
            Around4,
            Around5,
            Around6,
            Around7,
            Around8,
            Role,
            MobRandom,
        }

        public enum SkillCondition
        {
            Always,
            OnSpawn,
            OnDead,
            MyHPLTMaxRate,
            MyHPInRate,
            MasterHPLTMaxRate,
            MasterAttacked,
            SummonerHPLTMaxRate,
            FriendHPLTMaxRate,
            FriendHPInRate,
            FriendStatusOn,
            FriendStatusOff,
            AttackPCGT,
            AttackPCGE,
            SlaveLT,
            SlaveLE,
            SummonLT,
            SummonLE,
            ClosedAttacked,
            LongRangeAttacked,
            SkillUsed,
            AfterSkill,
            CastTargeted,
            RudeAttacked,
            Expanded,
            MobNearbyGT,
            NearPortal,
            MyStatusOn,
            MyStatusOff,
            MasterStatusOn,
            MasterStatusOff,
            SummonerStatusOn,
            SummonerStatusOff,
            EnemyStatusOn,
            EnemyStatusOff,
            GroundAttacked,
            DamagedGT,
            Alchemist,
        }

        public string GetDescriptionMobName()
        {
            return Description
                .Split("@")
                .FirstOrDefault();
        }

        public string GetDescriptionSkillName()
        {
            return Description
                .Split("@")
                .LastOrDefault();
        }
    }
}