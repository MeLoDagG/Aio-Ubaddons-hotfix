using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;
using UBAddons.Libs;

namespace UBAddons.UBCore.BaseUlt
{
    internal class Ultilmate_Infomation
    {
        public Champion Champion;
        public SpellSlot Slot;
        public SkillShotType Type;
        public uint Range;
        public int Width;
        public int CastDelay;
        public int Speed;
        public int AllowedCollisionCount;
        public Ultilmate_Infomation(Champion champion, SpellSlot slot, SkillShotType type, uint range, int width, int castdelay, int speed, int allowedCollisionCount)
        {
            Champion = champion;
            Slot = slot;
            Type = type;
            Range = range;
            Width = width;
            CastDelay = castdelay;
            Speed = speed;
            AllowedCollisionCount = allowedCollisionCount;
        }
    }
    internal class Teleport_Infomation
    {
        public Obj_AI_Base Enemy;
        public bool IsRecall;
        public float Lastseen;
        public float End;
        public float Duration;
        public float Started;
        public Teleport_Infomation(Obj_AI_Base enemy, bool isRecall)
        {
            Enemy = enemy;
            IsRecall = isRecall;
            Lastseen = 0f;
            Duration = 0f;
        }

        public float Percent
        {
            get
            {
                return Math.Max(0, Math.Min(100, (End - Core.GameTickCount) / Duration * 100));
            }
        }
        public float Remaining
        {
            get
            {
                return Math.Max(0, End - Core.GameTickCount) / 1000f;
            }
        }
    }
}
