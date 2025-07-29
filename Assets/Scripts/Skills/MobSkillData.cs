using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public static MobSkillData Default => new MobSkillData
        {
            // must set these values after creation
            InstanceID = Guid.Empty,
            MobID = 0,
            Description = string.Empty,
            SkillID = 0,

            // default values
            SkillLevel = 1,
            Rate = 2000,
            CastTime = 500,
            Delay = 10000,
            Cancelable = false,
            Emotion = 0,
            Chat = 0,
            State = MobState.Any,
            Target = MobTarget.Target,
            Condition = SkillCondition.Always,
            ConditionValue = 0,
            Values = new int[] { 0, 0, 0, 0, 0, },
            Extras = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        };

        public void UpdateDescriptionSkillName(string name)
        {
            string mobName = GetDescriptionMobName();
            Description = $"{mobName}@{name}";
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

        public Guid GetGuid()
        {
            return Guid.NewGuid();
            //string line = BuildLine();
            //return CreateGuidFromString(line);
        }

        public string BuildLine()
        {
            return BuildLine(this);
        }

        private Guid CreateGuidFromString(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return new Guid(hash);
            }
        }

        private string BuildLine(MobSkillData skill)
        {
            return string.Join(",",
                skill.MobID,
                skill.Description,
                skill.State.ToString().ToLower(),
                skill.SkillID,
                skill.SkillLevel,
                skill.Rate,
                skill.CastTime,
                skill.Delay,
                skill.Cancelable ? "yes" : "no",
                skill.Target.ToString().ToLower(),
                skill.Condition.ToString().ToLower(),
                skill.ConditionValue,
                skill.Values[0],
                skill.Values[1],
                skill.Values[2],
                skill.Values[3],
                skill.Values[4],
                skill.Emotion,
                skill.Chat,
                skill.Extras[0],
                skill.Extras[1],
                skill.Extras[2],
                skill.Extras[3],
                skill.Extras[4],
                skill.Extras[5],
                skill.Extras[6],
                skill.Extras[7],
                skill.Extras[8],
                skill.Extras[9],
                skill.Extras[10],
                skill.Extras[11],
                skill.Extras[12],
                skill.Extras[13],
                skill.Extras[14],
                skill.Extras[15]
            );
        }
    }
}