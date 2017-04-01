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
        public int StackCount = 0;
        public int MaxStackCount = 15;
        public delegate int MenuStackCount();
        public MenuStackCount StackCountFromMenu = null;
        public bool IsRanged => this.Range < int.MaxValue;
        public bool RequireCast;
        public string MenuItemName => $"{this.Hero} {(this.Slot == SpellSlot.Unknown ? "Passive" : this.Slot.ToString())} ({this.BuffName})";
        public bool HasStackCount => this.StackCount > 0 && this.StackCount < int.MaxValue;
    }
}
