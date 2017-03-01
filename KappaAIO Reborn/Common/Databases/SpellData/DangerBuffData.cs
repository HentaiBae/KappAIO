using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class DangerBuffData
    {
        public Champion Hero;
        public SpellSlot Slot;
        public string BuffName;
        public int Delay;
        public int DangerLevel;
        public int Range = int.MaxValue;
        public bool IsRanged => this.Range < int.MaxValue;
        public string MenuItemName => $"{this.Hero} {(this.Slot == SpellSlot.Unknown ? "Passive" : this.Slot.ToString())} ({this.BuffName})";
    }
}
