using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;

namespace KappAIO_Reborn.Common.Databases.Spells
{
    public static class DangerBuffDataDatabase
    {
        public static DangerBuffData[] Current;

        static DangerBuffDataDatabase()
        {
            if(Current != null)
                return;

            Current = List.Where(s => s.Hero == Champion.Unknown || EntityManager.Heroes.AllHeroes.Any(h => s.Hero.Equals(h.Hero))).ToArray();
        }

        private static DangerBuffData[] List =
            {
                new DangerBuffData
                    {
                        Hero = Champion.Brand,
                        Slot = SpellSlot.Unknown,
                        BuffName = "BrandAblazeDetonateMarker",
                        DangerLevel = 2
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Jax,
                        Slot = SpellSlot.E,
                        BuffName = "JaxEvasion",
                        Range = 300,
                        DangerLevel = 4
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Kled,
                        Slot = SpellSlot.Q,
                        BuffName = "KledQMark",
                        DangerLevel = 4
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Karthus,
                        Slot = SpellSlot.R,
                        BuffName = "karthusfallenone",
                        DangerLevel = 5
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Karma,
                        Slot = SpellSlot.W,
                        BuffName = "karmaspiritbind",
                        DangerLevel = 3
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Kalista,
                        Slot = SpellSlot.E,
                        BuffName = "KalistaExpungeMarker",
                        DangerLevel = 2,
                        StackCount = 5,
                        MaxStackCount = 15,
                        RequireCast = true,
                        Range = 1000
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Tristana,
                        Slot = SpellSlot.E,
                        BuffName = "tristanaechargesound",
                        DangerLevel = 2
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Leblanc,
                        Slot = SpellSlot.E,
                        BuffName = "LeblancE",
                        DangerLevel = 4
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Nocturne,
                        Slot = SpellSlot.E,
                        BuffName = "NocturneUnspeakableHorror",
                        DangerLevel = 5,
                        Delay = 2000
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Morgana,
                        Slot = SpellSlot.R,
                        BuffName = "soulshackles",
                        DangerLevel = 5
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Fizz,
                        Slot = SpellSlot.R,
                        BuffName = "fizzrcling",
                        DangerLevel = 5
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Warwick,
                        Slot = SpellSlot.E,
                        BuffName = "WarwickE",
                        DangerLevel = 4,
                        Range = 300,
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Zed,
                        Slot = SpellSlot.R,
                        BuffName = "zedultexecute",
                        DangerLevel = 4
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Zilean,
                        Slot = SpellSlot.Q,
                        BuffName = "ZileanQEnemyBomb",
                        DangerLevel = 2
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Varus,
                        Slot = SpellSlot.R,
                        BuffName = "VarusRShackles",
                        DangerLevel = 5
                    },
                new DangerBuffData
                    {
                        Hero = Champion.Vladimir,
                        Slot = SpellSlot.R,
                        BuffName = "vladimirhemoplaguedebuff",
                        DangerLevel = 5
                    },
                /*TODO
                 * Add/Find more buffs
                 */
            };
    }
}
