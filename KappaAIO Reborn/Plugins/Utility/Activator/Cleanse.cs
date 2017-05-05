using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Databases.Items;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Utility;
using Extensions = KappAIO_Reborn.Common.Utility.Extensions;
using ItemData = KappAIO_Reborn.Common.Databases.Items.ItemData;

namespace KappAIO_Reborn.Plugins.Utility.Activator
{
    public static class Cleanse
    {
        public static bool Enabled => menu.CheckBoxValue("Enable");
        public static bool Allies => menu.CheckBoxValue("allies");
        public static int minDelay => menu.SliderValue("minDelay");
        public static int maxDelay => menu.SliderValue("maxDelay");
        public static int Health => menu.SliderValue("health");
        public static float lastQssAttempt;
        private static Menu menu;
        
        public static List<cleanseInstance> instances = new List<cleanseInstance>();
        
        public static void Init(params ItemData[] items)
        {
            if(!items.Any(i => i.item.ItemInfo.AvailableForMap))
                return;

            menu = Program.UtilityMenu.AddSubMenu("Cleanse");
            menu.AddGroupLabel("Cleanse Settings:");
            menu.CreateCheckBox("Enable", "Enable Cleanse");
            menu.AddGroupLabel("Humanizer Delay:");
            var min = menu.CreateSlider("minDelay", "Min Delay", 50, 50, 500);
            var max = menu.CreateSlider("maxDelay", "Max Delay", 500, 50, 500);

            min.OnValueChange += (sender, args) => max.MinValue = min.CurrentValue + 1;
            max.OnValueChange += (sender, args) => min.MaxValue = max.CurrentValue - 1;

            menu.AddSeparator(0);
            menu.AddGroupLabel("Target Checks:");
            menu.CreateCheckBox("allies", "Use On Allies");
            menu.CreateSlider("health", "Target Health Less Than [{0}%]", 90, 1);

            menu.AddSeparator(0);
            menu.AddGroupLabel("Cleanse Items:");
            instances.AddRange(items.Select(x => new cleanseInstance(menu, x)));

            menu.AddSeparator(0);
            menu.AddGroupLabel("CC Buffs to Cleanse:");
            foreach (var b in Extensions.CCBuffTypes)
                menu.CreateCheckBox(b.BuffType.ToString(), $"Cleanse {b.BuffType}", b.DangerLevel > 2);
        }

        public static bool BuffIsEnabled(BuffType buff)
        {
            if (Extensions.CCBuffTypes.All(b => b.BuffType != buff))
                return false;

            return menu.CheckBoxValue(buff);
        }

        public static bool HasBuffType(this Obj_AI_Base target, BuffType buff)
        {
            return BuffIsEnabled(buff) && target.HasBuffOfType(buff);
        }
    }

    public class cleanseInstance
    {
        private Menu iMenu;
        private ItemData Item;

        public cleanseInstance(Menu menu, ItemData item)
        {
            this.iMenu = menu;
            this.Item = item;
            
            menu.CreateCheckBox($"{item.Name}enable", $"Enable {this.Item.Name}");

            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Game.OnTick += Game_OnTick;
        }

        private float currentDelay = Game.Ping;
        private Random rnd = new Random();

        private void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if(!sender.IsChampion())
                return;

            currentDelay = rnd.Next(Cleanse.minDelay, Cleanse.maxDelay);
        }

        private void Game_OnTick(EventArgs args)
        {
            if(!this.iMenu.CheckBoxValue($"{this.Item.Name}enable") || !this.Item.Ready)
                return;
            
            var target = this.getQssTarget();
            if(target == null)
                return;
            
            if (Core.GameTickCount - Cleanse.lastQssAttempt < this.currentDelay + 500)
                return;
            
            Cleanse.lastQssAttempt = Core.GameTickCount;
            Core.DelayAction(() => this.Item.Cast(target), (int)this.currentDelay);
        }

        private void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsChampion() || !sender.IsAlly)
                return;

            currentDelay = rnd.Next(Cleanse.minDelay, Cleanse.maxDelay);
        }
        
        private AIHeroClient getQssTarget()
        {
            var instances = this.getBuffInstances();
            if (instances == null || !instances.Any())
                return null;
            
            return instances.OrderByDescending(x => x.Data.DangerLevel * TargetSelector.GetPriority(x.Owner)).FirstOrDefault(x => this.Item.IsInRange(x.Owner))?.Owner;
        }

        private List<buffInstance> getBuffInstances()
        {
            var result = new List<buffInstance>();
            foreach (var hero in EntityManager.Heroes.Allies.Where(h => h.IsValidTarget()))
                result.AddRange(Extensions.CCBuffTypes.Where(x => hero.HasBuffType(x.BuffType)).Select(buff => new buffInstance(hero, buff)));

            return result;
        }

        private class buffInstance
        {
            public buffInstance(AIHeroClient owner, DangerBuffTypes data)
            {
                this.Owner = owner;
                this.Data = data;
            }
            public DangerBuffTypes Data;
            public AIHeroClient Owner;
        }
    }
}
