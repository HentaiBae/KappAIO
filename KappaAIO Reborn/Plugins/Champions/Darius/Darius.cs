using System;
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
            private static Menu cMenu, kMenu, dMenu, mMenu;

            private static CheckBox Qaoe;
            public static bool useQaoe => Qaoe.CurrentValue;

            private static CheckBox QAA, Q, W, R, ksR, Blade, outQ, inQ, dmgR, dE, dR, antidashE, intE;
            public static bool noQAA => QAA.CurrentValue;

            private static Slider Qaoehits;
            public static int hitsQaoe => Qaoehits.CurrentValue;
            
            public static bool useComboQ => Q.CurrentValue;
            public static bool hitBlade => Blade.CurrentValue;
            public static bool useComboW => W.CurrentValue;
            public static bool useComboR => R.CurrentValue;
            public static bool useKSR => ksR.CurrentValue;
            public static bool drawoutQ => outQ.CurrentValue;
            public static bool drawinQ => inQ.CurrentValue;
            public static bool drawinRdmg => dmgR.CurrentValue;
            public static bool drawE => dE.CurrentValue;
            public static bool drawR => dR.CurrentValue;
            public static bool dashE => antidashE.CurrentValue;
            public static bool interE => intE.CurrentValue;

            public Config()
            {
                #region combo

                cMenu = menu.AddSubMenu("Darius: Combo");
                Q = cMenu.CreateCheckBox("q", "Combo Q");
                QAA = cMenu.CreateCheckBox("noAAQ", "No Q When target in AA Range");
                Qaoe = cMenu.CreateCheckBox("Qaoe", "Combo Q AOE");
                Qaoehits = cMenu.CreateSlider("qhits", "Q AOE Hit Count", 2, 2, 6);
                Blade = cMenu.CreateCheckBox("blade", "Hit blade Combo Q");

                W = cMenu.CreateCheckBox("W", "Combo W AA Reset");
                R = cMenu.CreateCheckBox("r", "Combo R");

                #endregion combo

                #region killsteal

                kMenu = menu.AddSubMenu("Darius: KillSteal");
                ksR = kMenu.CreateCheckBox("r", "Use R");

                #endregion killsteal

                #region drawing

                dMenu = menu.AddSubMenu("Darius: Drawing");
                outQ = dMenu.CreateCheckBox("outQ", "draw Outer Q Range");
                inQ = dMenu.CreateCheckBox("inQ", "draw Inner Q Range");
                dE = dMenu.CreateCheckBox("dE", "Draw E Range");
                dR = dMenu.CreateCheckBox("dR", "Draw R Range");
                dmgR = dMenu.CreateCheckBox("dmgR", "Draw R Damage");

                #endregion drawing

                #region misc

                mMenu = menu.AddSubMenu("Darius: Misc");
                antidashE = mMenu.CreateCheckBox("antidashE", "Anti-Dash E");
                intE = mMenu.CreateCheckBox("intE", "Interrupter E");

                #endregion misc

            }
        }

        internal static Spell.Active Q, W;
        internal static Spell.Skillshot E;
        internal static Spell.Targeted R;

        private static float _lastQCast;
        private static float _qChargeLeft => _lastQCast + Q.CastDelay - Core.GameTickCount;
        private static float _currentQChargeTime => _qChargeLeft < 0 ? Q.CastDelay : _qChargeLeft;

        private static bool IsCastingQ => Core.GameTickCount - _lastQCast < Q.CastDelay;
        private static bool IsChargingQ => DariusStuff.HasDariusQChargingBuff || !DariusStuff.HasDariusQChargingBuff && IsCastingQ;

        private static float outerBlade => Q.Range;
        private static float bladeStart => 245;

        public override void OnLoad()
        {
            Q = new Spell.Active(SpellSlot.Q, 425, DamageType.Physical) { CastDelay = 750 };
            W = new Spell.Active(SpellSlot.W, 200, DamageType.Physical);
            E = new Spell.Skillshot(SpellSlot.E, 510, SkillShotType.Cone, 250, int.MaxValue, 70, DamageType.Physical);
            R = new Spell.Targeted(SpellSlot.R, 460, DamageType.True);

            new Config();

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OverrideOrbwalkPosition += this.OverrideOrbwalkPosition;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Dash.OnDash += Dash_OnDash;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        private void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && E.IsReady() && sender.IsKillable(E.Range, true) && Config.interE)
                E.Cast(sender);
        }

        private void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            antiDashE(sender, e.EndPos);
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            antiDashE(sender, e.End);
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if(!target.IsValidTarget() || !target.IsChampion() || !W.IsReady() || IsChargingQ)
                return;

            if (Config.useComboW && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                W.Cast();
        }

        private Vector3? OverrideOrbwalkPosition()
        {
            var pos = qPos();
            return Config.hitBlade && IsChargingQ && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && pos != null && pos != Vector3.Zero ? qPos() : null;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!sender.IsMe)
                return;

            if (args.SData.Name.Equals("DariusCleave"))
                _lastQCast = Core.GameTickCount;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.HPBarPosition.IsOnScreen() && e.IsValidTarget()))
            {
                e.DrawDamage(DariusStuff.ComboDamage(e));
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

            Drawing.DrawText(user.ServerPosition.WorldToScreen(), Color.AliceBlue, IsChargingQ.ToString(), 10);
        }

        public override void OnTick()
        {
        }

        public override void Combo()
        {
            if(Config.useComboQ)
                ComboQ();

            if(Config.useComboR)
                ComboR();
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
            if (Config.useKSR)
                ComboR();
        }

        private static void ComboQ()
        {
            var target = qPos();
            var QAACheck = !Config.noQAA || Config.noQAA && user.CountEnemyChampionsInRange((int)user.GetAutoAttackRange()) == 0;

            if (target != Vector3.Zero && target != null && QAACheck)
            {
                Q.Cast();
            }
        }

        private static void ComboR()
        {
            var validEnemies = EntityManager.Heroes.Enemies.FindAll(e => e.IsKillable(R.Range, true, true, true));
            if(!validEnemies.Any())
                return;

            var ksTarget = validEnemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => !t.WillDie(R) && DariusStuff.Rdmg(t) > t.TotalShieldHealth());
            if (ksTarget != null && (R.IsReady() || DariusStuff.HasDariusUltResetBuff))
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

            if (!E.IsInRange(end) && target.IsKillable(E.Range, true))
            {
                var pred = E.GetPrediction(target);
                if(E.IsInRange(pred.CastPosition))
                    E.Cast(target.ServerPosition);
            }
        }
    }
}
