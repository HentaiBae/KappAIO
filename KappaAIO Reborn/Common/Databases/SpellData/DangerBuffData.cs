using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class DangerBuffData
    {
        public Champion Hero;
        public SpellSlot Slot;
        public string BuffName;
        public int DangerLevel;
        public string MenuItemName => $"{this.Hero} {this.Slot} ({this.BuffName})";
    }
}
