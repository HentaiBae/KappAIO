using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class SkillshotData
    {
        public Type type;
        public Champion hero;
        public SpellSlot slot;
        public int CollideCount;
        public int DangerLevel;
        public int RequireBuffCount;
        public float Range;
        public float Angle;
        public float Width;
        public float Speed;
        public float CastDelay;
        public string TargetName;
        public string MissileName;
        public string ParticalName;
        public string ParticalObjectName;
        public string SpellName;
        public string RequireBuff;
        public string[] ExtraSpellName;
        public string[] ExtraMissileName;
        public string[] DodgeFrom;
        public bool EndIsCasterDirection;
        public bool ForceRemove;
        public bool IsFixedRange;
        public bool DetectByMissile;
        public bool EndSticksToCaster;
        public bool SticksToCaster;
        public bool SticksToMissile;
        public bool FastEvade;
        public bool StartsFromTarget;
        public Collision[] Collisions;
    }

    public enum Type
    {
        LineMissile,
        CircleMissile,
        Cone
    }

    public enum Collision
    {
        YasuoWall,
        Minions,
        Heros,
        Walls,
        Caster
    }
}
