using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
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
        internal class ComboConfig // hellsing's style
        {
            private static Menu cMenu;

            private static CheckBox Qaoe;
            public static bool useQaoe => Qaoe.CurrentValue;

            private static Slider Qaoehits;
            public static int hitsQaoe => Qaoehits.CurrentValue;

            private static CheckBox Q;
            public static bool useQ => Q.CurrentValue;

            private static CheckBox Blade;
            public static bool hitBlade => Blade.CurrentValue;

            private static CheckBox R;
            public static bool useR => R.CurrentValue;

            public ComboConfig()
            {
                cMenu = menu.AddSubMenu("Combo");
                Q = cMenu.CreateCheckBox("q", "Combo Q");
                Qaoe = cMenu.CreateCheckBox("Qaoe", "Combo Q AOE");
                Qaoehits = cMenu.CreateSlider("qhits", "Q AOE Hit Count", 2, 1, 6);
                Blade = cMenu.CreateCheckBox("blade", "Hit blade Combo Q");
                R = cMenu.CreateCheckBox("r", "Combo R");
            }
        }

        internal class KillStealConfig
        {
            private static Menu kMenu;

            private static CheckBox R;
            public static bool useR => R.CurrentValue;

            public KillStealConfig()
            {
                kMenu = menu.AddSubMenu("KillSteal");
                R = kMenu.CreateCheckBox("r", "Use R");
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
        private static float bladeStart => 250;

        public override void OnLoad()
        {
            Q = new Spell.Active(SpellSlot.Q, 425, DamageType.Physical) { CastDelay = 750 };
            W = new Spell.Active(SpellSlot.W, 200, DamageType.Physical);
            E = new Spell.Skillshot(SpellSlot.E, 450, SkillShotType.Cone, 250, int.MaxValue, 70, DamageType.Physical);
            R = new Spell.Targeted(SpellSlot.R, 460, DamageType.True);

            new ComboConfig();
            new KillStealConfig();

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OverrideOrbwalkPosition += this.OverrideOrbwalkPosition;
        }
        
        private Vector3? OverrideOrbwalkPosition()
        {
            return ComboConfig.hitBlade && IsChargingQ && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? qPos() : null;
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
                Drawing.DrawText(e.ServerPosition.WorldToScreen(), Color.AliceBlue, e.HasReviveBuff().ToString(), 10);
                e.DrawDamage(DariusStuff.ComboDamage(e));
            }

            qPos()?.DrawCircle(100, SharpDX.Color.Red);

            //var pos = hitBladePos(_getQTarget());
            //pos?.DrawCircle(100, SharpDX.Color.Red);

            Circle.Draw(SharpDX.Color.AliceBlue, outerBlade, Player.Instance);
            Circle.Draw(SharpDX.Color.AliceBlue, bladeStart, Player.Instance);

            Drawing.DrawText(Player.Instance.ServerPosition.WorldToScreen(), Color.AliceBlue, IsChargingQ.ToString(), 10);
        }

        public override void OnTick()
        {
        }

        public override void Combo()
        {
            if(ComboConfig.useQ)
                ComboQ();

            if(ComboConfig.useR)
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
            if (KillStealConfig.useR)
                ComboR();
        }

        private static void ComboQ()
        {
            var target = qPos();

            if (target != null)
            {
                Q.Cast();
            }
        }

        private static void ComboR()
        {
            var ksTarget = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => t.IsKillable(R.Range, true, true, true) && !t.WillDie(R) && DariusStuff.Rdmg(t) > t.TotalShieldHealth());
            if (ksTarget != null && (R.IsReady() || DariusStuff.HasDariusUltResetBuff))
            {
                R.Cast(ksTarget);
            }
        }

        private static AIHeroClient _getQTarget()
        {
            var target = TargetSelector.GetTarget(Q.Range * 1.4f, DamageType.Physical);
            if (target != null && canHitBlade(target))
            {
                return target;
            }

            return null;
        }

        private static Vector3? qPos()
        {
            return aoeQPos(ComboConfig.hitsQaoe) ?? hitBladePos(_getQTarget());
        }

        private static Vector3? aoeQPos(int hitCount)
        {
            if (!ComboConfig.useQaoe)
                return null;

            var validEnemies = EntityManager.Heroes.Enemies.FindAll(e => e.IsKillable(Q.Range * 1.5f, true) && canHitBlade(e)).OrderByDescending(e => hitBladePos(e).GetValueOrDefault().CountEnemyHeroesInRangeWithPrediction((int)Q.Range, (int)_currentQChargeTime));

            if (validEnemies.Count() > hitCount)
            {
                var predCenter = validEnemies.Select(e => e.PrediectPosition(_currentQChargeTime)).CenterVectors();
                if (validEnemies.All(e => e.PrediectPosition(_currentQChargeTime).IsInRange(predCenter, Q.Range)))
                    return predCenter;
            }

            var mostHitsTarget = validEnemies.FirstOrDefault();
            if (mostHitsTarget != null && hitBladePos(mostHitsTarget).GetValueOrDefault().CountEnemyHeroesInRangeWithPrediction((int)Q.Range, (int)_currentQChargeTime) >= hitCount)
            {
                return hitBladePos(mostHitsTarget);
            }
            
            return null;
        }

        private static Vector3? hitBladePos(AIHeroClient target)
        {
            if (target == null)
                return null;

            var pred = target.PrediectPosition(_currentQChargeTime);
            var pos = pred.Extend(Player.Instance, bladeStart + target.BoundingRadius).To3D();

            return pos;
        }
        
        private static bool canHitBlade(Obj_AI_Base target)
        {
            var chargeTime = _currentQChargeTime;
            var pred = target.PrediectPosition(chargeTime);
            var mypred = Player.Instance.PrediectPosition(_currentQChargeTime);

            return _isInsideBlade(target) || mypred.IsInRange(pred, outerBlade - target.BoundingRadius);
        }

        private static bool _isInsideBlade(Obj_AI_Base target)
        {
            return _isInsideBlade(target.PrediectPosition(_currentQChargeTime)) && _isInsideBlade(target.ServerPosition);
        }

        private static bool _isInsideBlade(Vector3 target)
        {
            var distance = target.Distance(Player.Instance);
            return distance > bladeStart && distance < outerBlade;
        }
    }
}
