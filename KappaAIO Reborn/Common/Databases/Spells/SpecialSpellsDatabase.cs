using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;

namespace KappAIO_Reborn.Common.Databases.Spells
{
    public static class SpecialSpellsDatabase
    {
        public static SpecialSpellData[] Current;

        static SpecialSpellsDatabase()
        {
            if (Current != null)
                return;

            Current = List.Where(s => s.Hero == Champion.Unknown || EntityManager.Heroes.AllHeroes.Any(h => s.Hero.Equals(h.Hero))).ToArray();
        }

        private static SpecialSpellData[] List =
            {
                new SpecialSpellData
                    {
                        Hero = Champion.LeeSin,
                        Slot = SpellSlot.Q,
                        SpellName = "BlindMonkQTwo",
                        DisplayName = "2nd Cast",
                        CastDelay = 100,
                        DangerLevel = 2
                    },
                new SpecialSpellData
                    {
                        Hero = Champion.Tryndamere,
                        Slot = SpellSlot.W,
                        SpellName = "TryndamereW",
                        DisplayName = "Slow / Facing",
                        Range = 850,
                        CastDelay = 250,
                        DangerLevel = 3
                    },
            };
    }
}
