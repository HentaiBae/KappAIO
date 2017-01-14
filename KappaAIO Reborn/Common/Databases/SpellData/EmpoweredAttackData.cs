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
        public int RequiredBuffCount;
        public int TargetRequiredBuffCount;
        public int DangerLevel;
        public bool CrowdControl;
    }
}
