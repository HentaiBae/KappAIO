using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;

namespace KappAIO_Reborn.Common.Databases.Items
{
    public static class ItemsDatabase
    {
        public static ItemData Mercurial_Scimitar;
        public static ItemData Quicksilver_Sash;
        public static ItemData Dervish_Blade;
        public static ItemData Mikaels;

        public static ItemData Redemption;
        public static ItemData Randuins;

        public static ItemData Youmuus;
        public static ItemData Cutlass;
        public static ItemData Botrk;
        public static ItemData Tiamat;
        public static ItemData Hydra;
        public static ItemData TitanicHydra;
        public static ItemData HydraItem => Hydra.Ready ? Hydra : TitanicHydra.Ready ? TitanicHydra : Tiamat;

        public static List<ItemData> Current = new List<ItemData>();

        static ItemsDatabase()
        {
            if (Current == null || !Current.Any())
            {
                Current = new List<ItemData>();
                Current.Add(Tiamat = new ItemData(3077, ItemCastType.Active, 200, int.MaxValue, 0, TargetingType.AllEnemies)
                {
                    CastTimes = new [] { CastTime.PostAttack },
                    OrbModes = new [] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass, Orbwalker.ActiveModes.LaneClear, Orbwalker.ActiveModes.JungleClear }
                });
                Current.Add(Hydra = new ItemData(3074, ItemCastType.Active, 200, int.MaxValue, 0, TargetingType.AllEnemies)
                {
                    CastTimes = new[] { CastTime.PostAttack },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass, Orbwalker.ActiveModes.LaneClear, Orbwalker.ActiveModes.JungleClear }
                });
                Current.Add(TitanicHydra = new ItemData(3748, ItemCastType.Active, 200, int.MaxValue, 0, TargetingType.AllEnemies)
                {
                    CastTimes = new[] { CastTime.PostAttack },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass, Orbwalker.ActiveModes.LaneClear, Orbwalker.ActiveModes.JungleClear },
                });
                Current.Add(Cutlass = new ItemData(3144, ItemCastType.Targeted, 550, int.MaxValue, 0, TargetingType.EnemyHeros)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.EnemyHealth, CastTime.OnTick },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass },
                    MyHP = 90, EnemyHP = 80
                });
                Current.Add(Botrk = new ItemData(3153, ItemCastType.Targeted, 550, int.MaxValue, 0, TargetingType.EnemyHeros)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.EnemyHealth, CastTime.OnTick },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass },
                    MyHP = 80, EnemyHP = 75
                });
                Current.Add(Youmuus = new ItemData(3142, ItemCastType.Active, int.MaxValue, int.MaxValue, 0, TargetingType.EnemyHeros)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.PreAttck },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo, Orbwalker.ActiveModes.Harass },
                    MyHP = 100
                });

                Current.Add(Randuins = new ItemData(3143, ItemCastType.Active, 450, int.MaxValue, 0, TargetingType.EnemyHeros)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.AoE, CastTime.OnTick },
                    OrbModes = new[] { Orbwalker.ActiveModes.Combo },
                    MyHP = 90, AoeHit = 2,
                });

                Current.Add(Redemption = new ItemData(3107, ItemCastType.Position, 5500, int.MaxValue, 2500, TargetingType.AllyHeros, TargetingType.EnemyHeros)
                {
                    CastTimes = new[] { CastTime.AllyHealth, CastTime.AoE, CastTime.OnTick },
                    AllyHP = 45, AoeHit = 1,
                    Width = 525
                });

                Current.Add(Quicksilver_Sash = new ItemData(3140, ItemCastType.Active, 1, int.MaxValue, 0, TargetingType.MyHero)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.Cleanse },
                    MyHP = 95,
                });
                Current.Add(Mercurial_Scimitar = new ItemData(3139, ItemCastType.Active, 1, int.MaxValue, 0, TargetingType.MyHero)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.Cleanse },
                    MyHP = 95, 
                });
                Current.Add(Dervish_Blade = new ItemData(3137, ItemCastType.Active, 1, int.MaxValue, 0, TargetingType.MyHero)
                {
                    CastTimes = new[] { CastTime.MyHealth, CastTime.Cleanse },
                    MyHP = 95,
                });
                Current.Add(Mikaels = new ItemData(3222, ItemCastType.Targeted, 600, int.MaxValue, 0, TargetingType.AllyHeros)
                {
                    CastTimes = new[] { CastTime.AllyHealth, CastTime.Cleanse },
                    AllyHP = 80,
                });
            }
        }
    }
}
