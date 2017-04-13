using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Databases.Items;
using KappAIO_Reborn.Common.Utility;
using Extensions = KappAIO_Reborn.Common.Utility.Extensions;
using ItemData = KappAIO_Reborn.Common.Databases.Items.ItemData;

namespace KappAIO_Reborn.Plugins.Utility.Activator
{
    public static class Cleanse
    {
        public static int minDelay => menu.SliderValue("minDelay");
        public static int maxDelay => menu.SliderValue("maxDelay");
        public static bool Allies => menu.CheckBoxValue("allies");
        public static int Health => menu.SliderValue("health");
        private static Menu menu;

        public static bool Enabled => menu.CheckBoxValue("Enable");
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
    }

    public class cleanseInstance
    {
        private Menu iMenu;
        private ItemData Item;
        private class gainedBuff
        {
            public AIHeroClient owner;
            public BuffInstance buff;
            public float StartTick = Core.GameTickCount;
            public float Duration => (this.buff.EndTime - Game.Time) * 1000f;
            public float PassedTicks => Core.GameTickCount - this.StartTick;
            public bool ended => this.buff == null || Game.Time > this.buff.EndTime || !this.buff.IsValid || !this.buff.IsActive;

            public gainedBuff(AIHeroClient o, BuffInstance b)
            {
                this.owner = o;
                this.buff = b;
            }
        }

        private List<gainedBuff> gainedBuffs = new List<gainedBuff>();

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
            this.gainedBuffs.RemoveAll(b => b.owner.IdEquals(sender) && b.buff.Equals(args.Buff));
        }

        private void Game_OnTick(EventArgs args)
        {
            if(!this.iMenu.CheckBoxValue($"{this.Item.Name}enable") || !this.Item.Ready)
                return;

            this.verifyBuffs();
            var target = this.getQssTarget();
            if(target == null)
                return;
            
            Chat.Print($"cast QSS {target.owner.BaseSkinName}");
            this.Item.Cast(target.owner);
        }

        private void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsChampion() || !sender.IsAlly)
                return;

            currentDelay = rnd.Next(Cleanse.minDelay, Cleanse.maxDelay);
            this.add(sender as AIHeroClient, args.Buff);
        }

        private void add(AIHeroClient sender, BuffInstance buff)
        {
            this.gainedBuffs.Add(new gainedBuff(sender, buff));
        }

        private void verifyBuffs()
        {
            this.gainedBuffs.RemoveAll(b => b.ended || b.owner.IsDead);
        }
        
        private gainedBuff getQssTarget()
        {
            if (this.gainedBuffs == null || !this.gainedBuffs.Any())
                return null;

            var possible = new List<gainedBuff>();
            foreach (var t in this.Item.TargetType)
            {
                switch (t)
                {
                    case TargetingType.AllyHeros:
                        if(Cleanse.Allies)
                            possible.AddRange(this.gainedBuffs.Where(a => this.Item.IsInRange(a.owner) && a.owner.IsValid && a.owner.IsAlly && Cleanse.Health >= a.owner.HealthPercent));
                        break;
                    case TargetingType.MyHero:
                        possible.Add(this.gainedBuffs.FirstOrDefault(b => this.Item.IsInRange(b.owner) && b.owner.IsValid && b.owner.IsMe && Cleanse.Health >= b.owner.HealthPercent));
                        break;
                }
            }

            if (possible == null || !possible.Any())
                return null;

            return possible.Where(b => (b.PassedTicks > this.currentDelay || this.currentDelay > b.Duration) && Cleanse.BuffIsEnabled(b.buff.Type)).OrderByDescending(b => b.PassedTicks).FirstOrDefault();
        }
    }
}
