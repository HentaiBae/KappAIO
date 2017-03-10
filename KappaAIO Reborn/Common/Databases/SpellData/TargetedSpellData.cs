using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class TargetedSpellData
    {
        public Champion hero;
        public SpellSlot slot;
        public string[] MissileNames;
        public int DangerLevel;
        public float CastDelay = 0;
        public float Speed = float.MaxValue;
        public bool FastEvade;
        public bool WindWall;
        public string MenuItemName => $"{this.hero} {this.slot}";
    }
}
