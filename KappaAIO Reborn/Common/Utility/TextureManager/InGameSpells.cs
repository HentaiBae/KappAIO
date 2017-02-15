using EloBuddy;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class InGameSpells
    {
        public InGameSpells(Champion champ, InGameSpell[] spells)
        {
            this.Champion = champ;
            this.Spells = spells;
        }
        public Champion Champion;
        public InGameSpell[] Spells;
    }

    public class InGameSpell
    {
        public InGameSpell(SpellSlot slot, string name)
        {
            this.Slot = slot;
            this.Name = name;
        }
        public SpellSlot Slot;
        public string Name;
    }
}
