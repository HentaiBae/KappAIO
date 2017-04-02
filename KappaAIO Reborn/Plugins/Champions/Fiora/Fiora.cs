using System;
using System.Linq;
using System.Media;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using static KappAIO_Reborn.Plugins.Champions.Fiora.FioraStuff;
using static KappAIO_Reborn.Common.Databases.Items.ItemsDatabase;

namespace KappAIO_Reborn.Plugins.Champions.Fiora
{
    public class Fiora : ChampionBase
    {
        private static AIHeroClient _rTarget => EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(user, true)).FirstOrDefault(e => e.IsValidTarget(Q2.Range + 50) && e.HasBuff("fiorarmark"));
        private static AIHeroClient _qTarget => TargetSelector.SelectedTarget.IsValidTarget(Q2.Range + 50) ? TargetSelector.SelectedTarget : Q2.GetTarget();
        public static AIHeroClient QTarget => Config.focusRTarget ? _rTarget ?? _qTarget : _qTarget;

        public static Spell.Skillshot Q1, Q2, W;
        public static Spell.Active E;
        public static Spell.Targeted R;

        public override void OnLoad()
        {
            Q1 = new Spell.Skillshot(SpellSlot.Q, 400, SkillShotType.Circular, 100, 3000, 50, DamageType.Physical) { AllowedCollisionCount = int.MaxValue };
            Q2 = new Spell.Skillshot(SpellSlot.Q, 700, SkillShotType.Circular, 100, 3000, 50, DamageType.Physical) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Skillshot(SpellSlot.W, 750, SkillShotType.Linear, 500, 3200, 70, DamageType.Magical) { AllowedCollisionCount = int.MaxValue };
            E = new Spell.Active(SpellSlot.E, 275, DamageType.Physical);
            R = new Spell.Targeted(SpellSlot.R, 500, DamageType.True);

            Q1.AddRawDamage(new RawDamage
                {
                    PreCalculatedDamage = (() =>
                        {
                            if (!Q1.IsLearned)
                                return 0;
                            var index = Q1.Level - 1;
                            var dmg = new[] { 65f, 75f, 85f, 95f, 105f }[index];
                            var bonus = new[] { 0.95f, 1f, 1.05f, 1.1f, 1.15f }[index] * Player.Instance.FlatPhysicalDamageMod;
                            return dmg + bonus;
                        }),
                    Source = Player.Instance,
                    Spell = Q1,
                    Stage = 1
                });

            Q2.AddRawDamage(new RawDamage
            {
                PreCalculatedDamage = (() =>
                {
                    if (!Q1.IsLearned)
                        return 0;
                    var index = Q1.Level - 1;
                    var dmg = new[] { 65f, 75f, 85f, 95f, 105f }[index];
                    var bonus = new[] { 0.95f, 1f, 1.05f, 1.1f, 1.15f }[index] * Player.Instance.FlatPhysicalDamageMod;
                    return dmg + bonus;
                }),
                Source = Player.Instance,
                Spell = Q2,
                Stage = 1
            });

            W.AddRawDamage(new RawDamage
            {
                PreCalculatedDamage = (() =>
                {
                    if (!W.IsLearned)
                        return 0;
                    var index = W.Level - 1;
                    var dmg = new[] { 90f, 130f, 170f, 210f, 250f }[index];
                    var bonus = Player.Instance.TotalMagicalDamage;
                    return dmg + bonus;
                }),
                Source = Player.Instance,
                Spell = W,
                Stage = 1
            });

            E.AddRawDamage(new RawDamage
            {
                PreCalculatedDamage = (() =>
                {
                    if (!E.IsLearned)
                        return 0;
                    var index = E.Level - 1;
                    var mod = new[] { 1.4f, 1.55f, 1.7f, 1.85f, 2f }[index];
                    var dmg = Player.Instance.TotalAttackDamage;
                    return dmg * mod;
                }),
                Source = Player.Instance,
                Spell = E,
                Stage = 1
            });

            Config.Init();
            VitalManager.Init();
            SpellBlocker.Init();

            Orbwalker.OverrideOrbwalkPosition += this.OverrideOrbwalkPosition;
            Orbwalker.OnPostAttack += this.Orbwalker_OnPostAttack;
            Orbwalker.OnUnkillableMinion += this.Orbwalker_OnUnkillableMinion;
            Drawing.OnEndScene += Drawing_OnDraw;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }

        private bool played;
        private void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!played && Config.PlayAudio && Game.Time < 240)
            {
                played = true;
                Core.DelayAction(
                    () =>
                    {
                        var player = new SoundPlayer(Properties.Resources.FioraPROJECT);
                        player.Play();
                        player.Dispose();
                    }, 200 + Game.Ping);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var vitals in EntityManager.Heroes.Enemies.Where(e => VitalManager.HasFioraPassiveBuff(e) && e.IsValidTarget()).Select(VitalManager.Vitals))
            {
                foreach (var v in vitals.Where(v => v.ValidVital))
                {
                    v.QPredVitalPos.DrawCircle(100, Color.AliceBlue);
                    v.Vitalsector.Draw(System.Drawing.Color.AliceBlue, 2);
                }
            }

            Q1.DrawRange(System.Drawing.Color.AliceBlue);

            foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget()))
            {
                e.DrawDamage(SpellManager.ComboDamage(e));
            }
        }

        private void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if(Orbwalker.IsAutoAttacking)
                return;
            var minion = target as Obj_AI_Minion;
            if (!Config.useEUnk || minion == null || (HydraItem.Ready && Orbwalker.UseTiamat) || !E.IsReady() || !(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
                return;

            if (minion.IsKillable(user.GetAutoAttackRange(minion), false, true, true) && user.GetAutoAttackDamage(minion) >= minion.Health)
            {
                E.Cast();
            }
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!target.IsValidTarget(user.GetAutoAttackRange(target)))
                return;

            var jungle = Orbwalker.ModeIsActive(Orbwalker.ActiveModes.JungleClear);
            var minion = target as Obj_AI_Minion;
            if (jungle && minion != null && minion.BaseSkinName.StartsWith("SRU_") && minion.IsMonster)
            {
                if (Config.Ejungle && E.IsReady())
                {
                    E.Cast();
                    return;
                }
            }

            if (!target.IsChampion())
                return;

            var combo = Orbwalker.ModeIsActive(Orbwalker.ActiveModes.Combo);

            if (combo)
            {
                if (Config.useEReset && E.IsReady())
                {
                    E.Cast();
                    return;
                }
                if (Config.useHydra && HydraItem.Ready)
                {
                    HydraItem.Cast();
                }
            }
        }

        private Vector3? OverrideOrbwalkPosition()
        {
            if (!Config.orbwalk || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return null;

            var target = QTarget;

            if (target == null)
                return null;

            var AAvital = Config.orbAAVital && target.IsKillable(user.GetAutoAttackRange(target) * 1.15f) || !Config.orbAAVital && target.IsKillable(user.GetAutoAttackRange(target) * 1.35f);

            var vital = VitalManager.vital(target);
            if (vital == null)
                return null;

            var orbRVital = Config.orbUltVital && vital.IsRVital || !Config.orbUltVital;
            var validpos = !vital.OrbWalkVitalPos.IsWall() && !vital.OrbWalkVitalPos.IsBuilding();

            if (!validpos)
                return null;

            if(orbRVital && AAvital)
                return vital.OrbWalkVitalPos;

            return null;
        }

        public override void OnTick()
        {
            /*var speed = 500 + Player.Instance.MoveSpeed * 2.5f;
            Q1.Speed = (int)speed;
            Q2.Speed = (int)speed;*/
        }

        public override void Combo()
        {
            var target = QTarget;

            useQ(false, Config.useQShortVital, Config.useQLongvital, Config.validVitals);

            if (Config.useR && target.IsKillable(R.Range, false, true, true))
            {
                var comboDamage = SpellManager.ComboDamage(target);
                var validR = (comboDamage >= target.TotalShieldHealth() && target.TotalShieldHealth() >= SpellManager.ComboDamage(target, false)
                    || target.Health > user.Health && comboDamage >= target.Health) && !target.WillDie(500) && target.Health > user.GetAutoAttackDamage(target, true);
                if (validR)
                {
                    if (!target.IsUnderHisturret() && (Q1.IsInRange(target) && Q1.IsReady() || target.IsValidTarget(user.GetAutoAttackRange(target))))
                        RCast(target);
                }
            }
        }

        public override void Flee()
        {

        }

        public override void Harass()
        {

        }

        public override void LaneClear()
        {

        }

        public override void JungleClear()
        {

        }

        public override void KillSteal()
        {
            if (Orbwalker.IsAutoAttacking || user.Spellbook.IsCastingSpell || user.Spellbook.IsCharging || user.Spellbook.IsChanneling)
                return;

            if (Q2.IsReady() && Config.useQks)
            {
                var qtarget = Q2.GetKillStealTarget(SpellManager.QDamage(null), DamageType.Physical);
                if (qtarget != null && !qtarget.HasBuff("PoppyWZone") && !SkillshotDetector.SkillshotsDetected.Any(s => s.WillHit(Q2.GetPrediction(qtarget).CastPosition.To2D())))
                {
                    if (!useQ(false, Config.useQShortVital, Config.useQLongvital, false, qtarget))
                        Q2.Cast(qtarget);
                    return;
                }
            }
            if (W.IsReady() && Config.useWks)
            {
                var wtarget = W.GetKillStealTarget(SpellManager.WDamage(null), DamageType.Magical);
                if (wtarget != null)
                    W.Cast(wtarget, 75);
            }
        }

        private static bool useQ(bool gapCloser, bool shortQ, bool longQ, bool validVitals, AIHeroClient qtarget = null)
        {
            if(!Q1.IsReady())
                return false;

            var target = QTarget;
            if (qtarget != null)
                target = qtarget;

            if (!target.IsKillable())
                return false;
            
            if(target.HasBuff("PoppyWZone"))
                return false;

            if (shortQ || longQ)
            {
                var vital = VitalManager.vital(target, validVitals);
                var vitalResult = VitalManager.CanQVital(vital, shortQ, longQ);

                if (vitalResult.HasValue)
                {
                    if (SkillshotDetector.SkillshotsDetected.Any(s => s.WillHit(vitalResult.Value.To2D())))
                    {
                        return false;
                    }

                    return Q2.Cast(vitalResult.Value);
                }
            }

            if (gapCloser)
            {
                
            }

            return false;
        }

        internal static void RCast(AIHeroClient target)
        {
            if (target.IsKillable(R.Range) && !Config.MiscMenu.CheckBoxValue(target.Name()) && R.IsReady())
            {
                R.Cast(target);
            }
        }
    }
}
