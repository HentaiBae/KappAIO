using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class EmpoweredAttackData
    {
        public Champion Hero;
        public SpellSlot Slot;
        public string AttackName;
        public string DisplayName;
        public string RequireBuff;
        public string DontHaveBuff;
        public string TargetRequiredBuff;
        public int RequiredBuffCount = 1;
        public int TargetRequiredBuffCount = 1;
        public int DangerLevel;
        public bool CrowdControl;
        public string MenuItemName => $"{(this.Hero == Champion.Unknown ? "All Champions" : this.Hero.ToString())} {(this.Slot == SpellSlot.Unknown ? "" : this.Slot.ToString())} ({(!string.IsNullOrEmpty(DisplayName) ? DisplayName : !string.IsNullOrEmpty(AttackName) ? AttackName : !string.IsNullOrEmpty(RequireBuff) ? RequireBuff : TargetRequiredBuff)})";
    }
}
