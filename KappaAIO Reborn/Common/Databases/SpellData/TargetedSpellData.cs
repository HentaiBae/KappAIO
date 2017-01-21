using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class TargetedSpellData
    {
        public Champion hero;
        public SpellSlot slot;
        public int DangerLevel;
        public float CastDelay;
        public bool FastEvade;
        public string MenuItemName => $"{this.hero} {this.slot}";
    }
}
