using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappAIO_Reborn.Plugins.Champions.Darius
{
    public class Darius : ChampionBase
    {
        internal class Config // hellsing's style
        {
            private static Menu fMenu, cMenu, hMenu, lMenu, kMenu, dMenu, mMenu;

            private static CheckBox fleeQ; // flee
            private static CheckBox Qaoe, QAA, q, w, e, aoeE, r, ksR, Blade; // combo ks
            private static CheckBox hQAA, hq, hw, hBlade; // harass
            private static CheckBox outQ, inQ, dunk, dmg, dmgP, dmgQ, dmgW, dmgR, dE, dR, stackTime, RexpireTime; // Drawings
            private static CheckBox antidashE, intE; // Misc
            private static CheckBox laneW; // LaneClear

            private static Slider Qaoehits, EaoeHits, intDanger, fleeQHP;

            private static ComboBox antiDashmode;

            public static int Ehits => EaoeHits.CurrentValue;
            public static int hitsQaoe => Qaoehits.CurrentValue;
            public static int Edanger => intDanger.CurrentValue;
            public static int dashMode => antiDashmode.CurrentValue;
            public static int qfleeHP => fleeQHP.CurrentValue;

            public static bool hnoQAA => hQAA.CurrentValue;
            public static bool huseQ => hq.CurrentValue;
            public static bool huseW => hw.CurrentValue;
            public static bool hhitBlade => hBlade.CurrentValue;

            public static bool useWlane => laneW.CurrentValue;
            public static bool useE => e.CurrentValue;
            public static bool useEaoe => aoeE.CurrentValue;
            public static bool useQaoe => Qaoe.CurrentValue;
            public static bool noQAA => QAA.CurrentValue;
            public static bool useComboQ => q.CurrentValue;
            public static bool hitBlade => Blade.CurrentValue;
            public static bool useComboW => w.CurrentValue;
            public static bool useComboR => r.CurrentValue;
            public static bool useKSR => ksR.CurrentValue;
            public static bool drawoutQ => outQ.CurrentValue;
            public static bool drawinQ => inQ.CurrentValue;
            public static bool drawinRdmg => dmgR.CurrentValue;
            public static bool drawE => dE.CurrentValue;
            public static bool drawR => dR.CurrentValue;
            public static bool dashE => antidashE.CurrentValue;
            public static bool interE => intE.CurrentValue;
            public static bool drawDmg => dmg.CurrentValue;
            public static bool calcP=> dmgP.CurrentValue;
            public static bool calcQ => dmgQ.CurrentValue;
            public static bool calcW => dmgW.CurrentValue;
            public static bool calcR => dmgR.CurrentValue;
            public static bool dunkable => dunk.CurrentValue;
            public static bool stacksTimer => stackTime.CurrentValue;
            public static bool ultTimer => RexpireTime.CurrentValue;
            public static bool useQFlee => fleeQ.CurrentValue;

            public Config()
            {
                #region combo

                cMenu = menu.AddSubMenu("Darius: Combo");
                q = cMenu.CreateCheckBox("q", "Combo Q");
                QAA = cMenu.CreateCheckBox("noAAQ", "No Q When target in AA Range");
                Qaoe = cMenu.CreateCheckBox("Qaoe", "Combo Q AOE");
                Blade = cMenu.CreateCheckBox("blade", "Hit blade Combo Q");
                Qaoehits = cMenu.CreateSlider("qhits", "Q AOE Hit Count", 2, 2, 6);

                e = cMenu.CreateCheckBox("e", "Combo E if target outside AA Range");
                w = cMenu.CreateCheckBox("W", "Combo W AA Reset");
                r = cMenu.CreateCheckBox("r", "Combo R");

                aoeE = cMenu.CreateCheckBox("aoeE", "Use E AOE", false);
                EaoeHits = cMenu.CreateSlider("ehits", "E Hit Count", 3, 1, 6);

                #endregion combo

                #region harass

                hMenu = menu.AddSubMenu("Darius: Harass");
                hq = hMenu.CreateCheckBox("q", "Harass Q");
                hQAA = hMenu.CreateCheckBox("noAAQ", "No Q When target in AA Range");
                hBlade = hMenu.CreateCheckBox("blade", "Hit blade Harass Q", false);
                hw = hMenu.CreateCheckBox("W", "Harass W AA Reset");

                #endregion harass

                #region laneclear

                lMenu = menu.AddSubMenu("Darius: LaneClear");
                lMenu.AddGroupLabel("LaneClear and LastHit Settings");
                laneW = lMenu.CreateCheckBox("w", "Use W On Unkillable minions");

                #endregion laneclear

                #region killsteal

                kMenu = menu.AddSubMenu("Darius: KillSteal");
                ksR = kMenu.CreateCheckBox("r", "Use R");

                #endregion killsteal

                #region flee

                fMenu = menu.AddSubMenu("Darius: Flee");
                fleeQ = fMenu.CreateCheckBox("fleeQ", "Use Q Flee");
                fleeQHP = fMenu.CreateSlider("fleeQHP", "Use Q Flee under {0}% HP", 90, 0, 100);

                #endregion flee

                #region drawing

                dMenu = menu.AddSubMenu("Darius: Drawing");
                outQ = dMenu.CreateCheckBox("outQ", "draw Outer Q Range");
                inQ = dMenu.CreateCheckBox("inQ", "draw Inner Q Range");
                dE = dMenu.CreateCheckBox("dE", "Draw E Range");
                dR = dMenu.CreateCheckBox("dR", "Draw R Range");
                stackTime = dMenu.CreateCheckBox("stackTime", "Draw Stacks Timer");
                dunk = dMenu.CreateCheckBox("dunk", "Draw Text above Killable Enemies by R");
                RexpireTime = dMenu.CreateCheckBox("RexpireTime", "Draw Time until R Expires on HUD");

                dMenu.AddGroupLabel("Damage Drawings");
                dmg = dMenu.CreateCheckBox("dmg", "Draw Damage");
                dmgP = dMenu.CreateCheckBox("dmgP", "Passive Damage", false);
                dmgQ = dMenu.CreateCheckBox("dmgQ", "Q Damage", false);
                dmgW = dMenu.CreateCheckBox("dmgW", "W Damage", false);
                dmgR = dMenu.CreateCheckBox("dmgR", "R Damage");

                #endregion drawing

                #region misc

                mMenu = menu.AddSubMenu("Darius: Misc");
                antiDashmode = mMenu.Add("antiDashmode", new ComboBox("Anti-Dash Mode", 0, "Away", "Always"));
                antidashE = mMenu.CreateCheckBox("antidashE", "Anti-Dash E (Outside E Range)");
                intE = mMenu.CreateCheckBox("intE", "Interrupter E");
                intDanger = mMenu.CreateSlider("intDanger", "Interrupter DangerLevel", 1, 0, 2);

                #endregion misc
            }
        }

        internal static Spell.Active Q, W;
        internal static Spell.Skillshot E;
        internal static Spell.Targeted R;

        private static float _lastQCast;
        private static float _qChargeLeft => _lastQCast + Q.CastDelay - Core.GameTickCount;
        private static float _currentQChargeTime => _qChargeLeft < 0 ? Q.CastDelay : _qChargeLeft;

        private static bool startedQ;
        private static bool ultReady => R.IsReady() || DariusStuff.HasDariusUltResetBuff;
        private static bool IsCastingQ => Core.GameTickCount - _lastQCast < Q.CastDelay;
        private static bool IsChargingQ => DariusStuff.HasDariusQChargingBuff || startedQ;

        private static float outerBlade => Q.Range;
        private static float bladeStart => 245;

        public override void OnLoad()
        {
            Q = new Spell.Active(SpellSlot.Q, 425, DamageType.Physical) { CastDelay = 750 };
            W = new Spell.Active(SpellSlot.W, 100, DamageType.Physical) { CastDelay = 300 };
            E = new Spell.Skillshot(SpellSlot.E, 510, SkillShotType.Cone, 250, int.MaxValue, 80, DamageType.Physical) { ConeAngleDegrees = 50 };
            R = new Spell.Targeted(SpellSlot.R, 460, DamageType.True) { CastDelay = 250 };

            new Config();

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OverrideOrbwalkPosition += this.OverrideOrbwalkPosition;
            Orbwalker.OnPostAttack += this.Orbwalker_OnPostAttack;
            Gapcloser.OnGapcloser += this.Gapcloser_OnGapcloser;
            Dash.OnDash += this.Dash_OnDash;
            Interrupter.OnInterruptableSpell += this.Interrupter_OnInterruptableSpell;
            Spellbook.OnStopCast += this.Spellbook_OnStopCast;
            Obj_AI_Base.OnBuffGain += this.Obj_AI_Base_OnBuffGain;
            Orbwalker.OnUnkillableMinion += this.Orbwalker_OnUnkillableMinion;
        }

        private void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Orbwalker.IsAutoAttacking)
                return;

            if (!target.IsKillable(user.GetAutoAttackRange(target) + W.Range) || !W.IsReady() || !Config.useWlane)
                return;

            var shoulduse = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);

            if (shoulduse && user.GetAutoAttackDamage(target) + DariusStuff.Wdmg(target) >= target.PredictHealth(W.CastDelay) && !target.WillDie(W))
            {
                Orbwalker.ForcedTarget = target;
                W.Cast();
                Core.DelayAction((() => Orbwalker.ForcedTarget = null), (int)(Orbwalker.AttackCastDelay * 1000f));
            }
        }

        private void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender == null || !sender.IsMe)
                return;

            if (args.Buff.Type == BuffType.Stun)
                startedQ = false;
        }

        private void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if(sender == null || !sender.IsMe)
                return;

            startedQ = false;
        }

        private void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && E.IsReady() && sender.IsKillable(E.Range, true) && Config.interE && Common.Utility.Extensions.DangerLevels[Config.Edanger] >= e.DangerLevel)
                E.Cast(sender);
        }

        private void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if(sender.IsEnemy)
                antiDashE(sender, e.EndPos);
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsEnemy)
                antiDashE(sender, e.End);
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if(!target.IsValidTarget() || !target.IsChampion() || !W.IsReady() || IsChargingQ)
                return;

            var combow = Config.useComboW && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            var harassw = Config.huseW && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);

            if (combow || harassw)
                W.Cast();
        }

        private Vector3? OverrideOrbwalkPosition()
        {
            var pos = qPos();
            var shouldmove = Config.hhitBlade && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.hitBlade;
            return shouldmove && IsChargingQ && pos != null && pos != Vector3.Zero ? qPos() : null;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!sender.IsMe)
                return;

            if (args.SData.Name.Equals("DariusCleave"))
            {
                _lastQCast = Core.GameTickCount;
                startedQ = true;
            }
        }

        private static Text passive = new Text("", new Font("Arial", 13, FontStyle.Bold));
        private static Text dunktext = new Text("", new Font("Arial", 15, FontStyle.Bold));
        private static Text ulttext = new Text("", new Font("Arial", 15, FontStyle.Bold));

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.HPBarPosition.IsOnScreen() && e.IsValidTarget()))
            {
                if (Config.drawDmg)
                    e.DrawDamage(DariusStuff.ComboDamage(e, Config.calcP, Config.calcQ, Config.calcW, Config.calcR));

                if (Config.stacksTimer)
                {
                    var buff = DariusStuff.GetDariusPassive(e);
                    if (buff != null)
                    {
                        var timeLeft = buff.EndTime - Game.Time;
                        var mypos = e.ServerPosition.WorldToScreen();
                        var buffcount = Math.Max(1, buff.Count);
                        var ra = 51 * buffcount;
                        var ba = 255 - ra;
                        var ga = 255 - ra;
                        var c = Color.FromArgb(ra, ga, ba);
                        passive.Draw($"Stacks: {buff.Count} ({timeLeft.ToString("F1")})", c, new Vector2(mypos.X, mypos.Y - 36));
                    }
                }
                
                if (Config.dunkable)
                {
                    var killable = R.IsReady() && DariusStuff.Rdmg(e) > e.TotalShieldHealth() && e.IsKillable(-1, true, true, true);
                    if (killable)
                    {
                        var hpos = e.HpBarPos();
                        var drawpos = new Vector2(hpos.X, hpos.Y - 24);
                        dunktext.Draw("DUNK = KILL", Color.Red, drawpos);
                    }
                }
            }

            if (Config.ultTimer && DariusStuff.HasDariusUltResetBuff)
            {
                var x = Drawing.Width * 0.35f;
                var y = Drawing.Height * 0.8f;
                var drawpos = new Vector2(x, y);
                var timer = (DariusStuff.DariusUltResetBuff.EndTime - Game.Time).ToString("F1");
                ulttext.Draw($"R Expire Timer: {timer}", Color.OrangeRed, drawpos);
            }
            
            var pos = qPos();
            if(pos != null && pos != Vector3.Zero)
                pos.Value.DrawCircle(100, SharpDX.Color.Red);
            
            if(Config.drawoutQ)
                Circle.Draw(SharpDX.Color.AliceBlue, outerBlade, user);

            if (Config.drawinQ)
                Circle.Draw(SharpDX.Color.AliceBlue, bladeStart, user);

            if (Config.drawE)
                E.DrawRange(Color.AliceBlue);

            if (Config.drawR)
                R.DrawRange(Color.AliceBlue);
        }

        public override void OnTick()
        {
            if (DariusStuff.HasDariusQChargingBuff || !IsCastingQ)
                startedQ = false;
        }

        public override void Combo()
        {
            if(Config.useComboQ)
                ComboQ(Config.noQAA);
            
            ComboE();

            if (Config.useComboR)
                ComboR();
        }

        public override void Flee()
        {
            if (Config.useQFlee && Config.qfleeHP > user.HealthPercent && Q.IsReady())
            {
                var hitCount = _countHits(user.PrediectPosition(_currentQChargeTime));
                if (hitCount > 0)
                {
                    Q.Cast();
                }
            }
        }

        public override void Harass()
        {
            if(Config.huseQ)
                ComboQ(Config.hnoQAA);
        }

        public override void LaneClear()
        {

        }

        public override void JungleClear()
        {

        }

        public override void KillSteal()
        {
            if (Config.useKSR)
                ComboR();
        }

        private static void ComboQ(bool noQAA)
        {
            var target = qPos();
            var QAACheck = !noQAA || noQAA && user.CountEnemyChampionsInRange((int)user.GetAutoAttackRange()) == 0;

            if (target != Vector3.Zero && target != null && QAACheck)
            {
                Q.Cast();
            }
        }

        private static void ComboE()
        {
            if(!E.IsReady())
                return;
            var target = E.GetTarget();
            if(!target.IsKillable(E.Range, true))
                return;

            if (Config.useE)
            {
                if (!user.IsInAutoAttackRange(target))
                {
                    E.Cast(target, 45);
                }
            }

            if (Config.useEaoe)
            {
                var targets = EntityManager.Heroes.Enemies.FindAll(e => e.IsKillable(E.Range, true));

                if (targets.Count >= Config.Ehits)
                {
                    var results = new Dictionary<int, Geometry.Polygon.Sector>();
                    foreach (var t in targets)
                    {
                        var sector = new Geometry.Polygon.Sector(user.ServerPosition, E.GetPrediction(t).CastPosition, (float)(E.ConeAngleDegrees * Math.PI / 180), E.Range);
                        results.Add(targets.Count(sector.IsInside), sector);
                    }
                    
                    if (results.Any(r => r.Key >= Config.Ehits))
                    {
                        var bestHits = results.OrderByDescending(e => e.Key).FirstOrDefault(r => r.Key >= Config.Ehits);
                        var castpos = bestHits.Value.CenterOfPolygon().To3D();
                        E.Cast(castpos);
                    }
                }
            }
        }

        private static void ComboR()
        {
            if(!ultReady)
                return;

            var validEnemies = EntityManager.Heroes.Enemies.FindAll(e => e.IsKillable(R.Range, true, true, true));
            if(!validEnemies.Any())
                return;

            var ksTarget = validEnemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => !t.WillDie(R) && DariusStuff.Rdmg(t) > t.TotalShieldHealth());
            if (ksTarget != null)
            {
                R.Cast(ksTarget);
            }
        }

        private static AIHeroClient _getQTarget()
        {
            var target = TargetSelector.GetTarget(Q.Range * 1.3f, DamageType.Physical);
            if (target != null && canHitBlade(target))
            {
                return target;
            }

            return null;
        }

        private static Vector3? qPos()
        {
            var aoepos = aoeQPos(Config.hitsQaoe);
            var normalq = hitBladePos(_getQTarget());
            return aoepos != null && aoepos != Vector3.Zero ? aoepos : normalq;
        }

        private static Vector3? aoeQPos(int hitCount)
        {
            if (!Config.useQaoe)
                return null;

            var validEnemies = EntityManager.Heroes.Enemies.FindAll(e => e.IsValidTarget(Q.Range * 1.5f));
            if (hitCount > validEnemies.Count)
                return null;

            var hits = validEnemies.FindAll(canHitBlade);
            var mostHits = hits.OrderByDescending(e => _countHits(hitBladePos(e)));

            if (hitCount > mostHits.Count())
                return null;
            
            return hitBladePos(mostHits.FirstOrDefault());
        }

        private static Vector3 hitBladePos(AIHeroClient target)
        {
            if (target == null)
                return Vector3.Zero;

            var pred = target.PrediectPosition(_currentQChargeTime);
            var pos = pred.Extend(user, bladeStart + target.BoundingRadius).To3D();

            return pos;
        }
        
        private static bool canHitBlade(Obj_AI_Base target)
        {
            var chargeTime = _currentQChargeTime;
            var pred = target.PrediectPosition(chargeTime);
            var mypred = user.PrediectPosition(_currentQChargeTime);

            return _isInsideBlade(target) || mypred.IsInRange(pred, outerBlade - target.BoundingRadius);
        }

        private static bool _isInsideBlade(Obj_AI_Base target)
        {
            return _isInsideBlade(target.PrediectPosition(_currentQChargeTime)) && _isInsideBlade(target.ServerPosition);
        }

        private static bool _isInsideBlade(Vector3 target, Vector3? start = null)
        {
            var distance = start.GetValueOrDefault(user.ServerPosition).Distance(target);
            return distance > bladeStart && distance < outerBlade;
        }

        private static int _countHits(Vector3 pos)
        {
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget() && _isInsideBlade(e.PrediectPosition(_currentQChargeTime), pos));

            return enemies.Count();
        }

        private static void antiDashE(Obj_AI_Base target, Vector3 end)
        {
            if(!Config.dashE || !E.IsReady())
                return;

            var away = !E.IsInRange(end) && target.IsKillable(E.Range, true);

            var pred = E.GetPrediction(target);

            if (!E.IsInRange(pred.CastPosition))
                return;

            if (Config.dashMode == 0)
            {
                if (away)
                {
                    E.Cast(pred.CastPosition);
                }
            }
            else
            {
                E.Cast(pred.CastPosition);
            }
        }
    }
}
