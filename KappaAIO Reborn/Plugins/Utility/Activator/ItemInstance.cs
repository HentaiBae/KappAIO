using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappAIO_Reborn.Common.Databases.Items;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using ItemData = KappAIO_Reborn.Common.Databases.Items.ItemData;

namespace KappAIO_Reborn.Plugins.Utility.Activator
{
    public class ItemInstance
    {
        private bool Enabled => this.Data.Ready && this.ModeActive() && this.menu.CheckBoxValue($"{this.Data.Name}enable");
        private bool ResetAA => this.menu.CheckBoxValue($"{this.Data.Name}Reset");
        private bool PreAA => this.menu.CheckBoxValue($"{this.Data.Name}Pre");
        private Dictionary<Orbwalker.ActiveModes, CheckBox> orbModes = new Dictionary<Orbwalker.ActiveModes, CheckBox>();

        public ItemInstance(ItemData data, Menu subMenu)
        {
            this.Data = data;
            this.menu = subMenu;

            this.menu.AddGroupLabel($"- {data.item.ItemInfo.Name}");
            this.menu.CreateCheckBox($"{data.Name}enable", "Enable");

            if (this.Data.CastTimes != null)
            {
                foreach (var time in this.Data.CastTimes.OrderByDescending(x => x))
                {
                    switch (time)
                    {
                        case CastTime.PostAttack:
                            this.menu.CreateCheckBox($"{data.Name}Reset", "Use After Attack");
                            Orbwalker.OnPostAttack += this.Orbwalker_OnPostAttack;
                            break;
                        case CastTime.PreAttck:
                            this.menu.CreateCheckBox($"{data.Name}Pre", "Use Before Attack");
                            Orbwalker.OnPreAttack += this.Orbwalker_OnPreAttack;
                            break;
                        case CastTime.MyHealth:
                            this.menu.CreateSlider($"{data.Name}MyHealth", "Use if my Health less than [{0}%]", data.MyHP, 1);
                            break;
                        case CastTime.AllyHealth:
                            this.menu.CreateSlider($"{data.Name}AllyHealth", "Use if Ally Health less than [{0}%]", data.AllyHP, 1);
                            break;
                        case CastTime.EnemyHealth:
                            this.menu.CreateSlider($"{data.Name}EnemyHealth", "Use if Target Health less than [{0}%]", data.EnemyHP, 1);
                            break;
                        case CastTime.AoE:
                            this.menu.CreateSlider($"{data.Name}AoE", "Use if Will Hit [{0}]", data.AoeHit, 1, 6);
                            break;
                        case CastTime.OnTick:
                            Game.OnTick += this.Game_OnTick;
                            break;
                    }
                }
            }

            if (data.OrbModes != null)
            {
                this.menu.AddLabel($"- {data.Name} Orbwalker Modes (SDK)", 15);
                foreach (var mode in data.OrbModes)
                {
                    this.orbModes.Add(mode, this.menu.CreateCheckBox($"{data.Name}{mode}", $"Use On {mode}", mode == Orbwalker.ActiveModes.Combo || mode == Orbwalker.ActiveModes.Harass));
                }
            }
            
            this.menu.AddSeparator(0);
        }

        private void Game_OnTick(EventArgs args)
        {
            if(!this.Enabled)
                return;

            if (this.Data.matchCastType(CastTime.AoE))
            {
                var targets = this.getAoeTargets();
                
                if (targets.Count >= this.menu.SliderValue($"{this.Data.Name}AoE"))
                {
                    if (this.Data.CastType == ItemCastType.Active)
                        this.Data.Cast();
                    else
                        this.Data.Cast(this.getBestAOEPos(targets));
                }
                return;
            }

            var target = this.getTarget();
            if (target == null)
                return;

            this.Data.Cast(target);
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Enabled || !PreAA)
                return;

            if (!this.Data.ShouldCast(target))
                return;

            var unitBase = target as Obj_AI_Base;
            if (unitBase == null)
                return;

            if (!this.Data.ShouldCast(unitBase))
                return;

            if (healthCheck(Player.Instance) && healthCheck(unitBase))
            {
                this.Data.Cast(unitBase);
            }
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!Enabled || !ResetAA)
                return;

            Core.DelayAction(
                () =>
                {
                    if (Core.GameTickCount - Program.LastAAReset < 250)
                        return;

                    var unitBase = target as Obj_AI_Base;
                    if (unitBase == null)
                        return;

                    if (!this.Data.ShouldCast(unitBase))
                        return;

                    if (healthCheck(Player.Instance) && healthCheck(unitBase))
                    {
                        this.Data.Cast(unitBase);
                    }
                }, 100 + Math.Min(50, Game.Ping / 2));
        }

        private bool healthCheck(AttackableUnit unit)
        {
            var castTime = unit.IsMe ? new[] { CastTime.MyHealth, CastTime.AllyHealth }
                       : new[] { unit.IsAlly ? CastTime.AllyHealth : CastTime.EnemyHealth };
            
            return this.Data.CastTimes == null
                || !this.Data.matchCastType(castTime.ToArray())
                || castTime.Any(t => this.Data.CastTimes.Contains(t) && this.menu.SliderValue($"{this.Data.Name}{t}") >= unit.HealthPercent);
        }

        private bool ModeActive()
        {
            return this.Data.OrbModes == null || (this.Data.ModeIsActive
                && this.Data.OrbModes.Any(o => Orbwalker.ActiveModesFlags.HasFlag(o)
                && this.menu.CheckBoxValue($"{this.Data.Name}{o}"))) || this.orbModes.All(m => !m.Value.CurrentValue);
        }
        
        private Obj_AI_Base getTarget()
        {
            if (this.Data.TargetType != null)
            {
                foreach (var t in this.Data.TargetType)
                {
                    Obj_AI_Base x = null;
                    if (TargetingType.AllyHeros == t)
                    {
                        x = EntityManager.Heroes.Allies.OrderByDescending(e => e.MaxHealth).ThenBy(e => e.Health).FirstOrDefault(e => e.IsValidTarget(this.Data.Range) && !e.IsMe && this.healthCheck(e) && this.Data.ShouldCast(e));
                        if (x != null)
                            return x;
                    }
                    if (TargetingType.EnemyHeros == t)
                    {
                        x = EntityManager.Heroes.Enemies.OrderByDescending(e => e.MaxHealth).ThenBy(e => e.Health).FirstOrDefault(e => e.IsValidTarget(this.Data.Range) && !e.IsMe && this.healthCheck(e) && this.Data.ShouldCast(e));
                        if (x != null)
                            return x;
                    }
                }
            }
            return null;
        }

        private List<Obj_AI_Base> getAoeTargets()
        {
            var result = new List<Obj_AI_Base>();
            foreach (var t in this.Data.TargetType)
            {
                switch (t)
                {
                    case TargetingType.All:
                        result.AddRange(ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.AllAllies:
                        result.AddRange(EntityManager.Allies.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.AllEnemies:
                        result.AddRange(EntityManager.Enemies.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.AllyHeros:
                        result.AddRange(EntityManager.Heroes.AllHeroes.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.AllyMinions:
                        result.AddRange(EntityManager.MinionsAndMonsters.AlliedMinions.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.EnemyHeros:
                        result.AddRange(EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                    case TargetingType.EnemyMinions:
                        result.AddRange(EntityManager.MinionsAndMonsters.EnemyMinions.Where(o => o.IsValidTarget() && this.healthCheck(o) && this.Data.IsInRange(o)));
                        break;
                }
            }
            
            return result;
        }
        
        private Vector3 getBestAOEPos(List<Obj_AI_Base> targets)
        {
            var bestCenterTarget = targets.OrderByDescending(t => targets.Count(x => x != t && this.Data.GetPrediction(t).IsInRange(this.Data.GetPrediction(x), this.Data.Width + t.BoundingRadius / 2))).FirstOrDefault();
            if (bestCenterTarget != null)
            {
                var centerVector = targets.Where(t => t.IsInRange(bestCenterTarget, this.Data.Width + t.BoundingRadius/2)).Select(x => this.Data.GetPrediction(x)).CenterVectors();
                return centerVector;
            }

            return bestCenterTarget != null ? this.Data.GetPrediction(bestCenterTarget) : Vector3.Zero;
        }

        public ItemData Data;
        private Menu menu;
    }
}