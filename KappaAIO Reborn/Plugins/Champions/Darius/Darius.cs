using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappAIO_Reborn.Plugins.Champions.Darius
{
    public class Darius : ChampionBase
    {
        internal static Spell.Skillshot Q, E;
        internal static Spell.Active W;
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
            Q = new Spell.Skillshot(SpellSlot.Q, 425, SkillShotType.Circular, 750, int.MaxValue, 425, DamageType.Physical);
            W = new Spell.Active(SpellSlot.W, 200, DamageType.Physical);
            E = new Spell.Skillshot(SpellSlot.E, 450, SkillShotType.Cone, 250, int.MaxValue, 70, DamageType.Physical);
            R = new Spell.Targeted(SpellSlot.R, 460, DamageType.True);

            this.menu.CreateCheckBox("q", "Combo Q");
            this.menu.CreateCheckBox("blade", "Hit blade Combo Q");
            this.menu.CreateCheckBox("r", "Combo R");
            
            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OverrideOrbwalkPosition += this.OverrideOrbwalkPosition;
        }

        private Vector3? OverrideOrbwalkPosition()
        {
            if (this.menu.CheckBoxValue("blade"))
            {
                var target = TargetSelector.GetTarget(Q.Range * 2, DamageType.Physical);
                if (target != null && canHitBlade(target) && IsChargingQ)
                {
                    var pred = target.PrediectPosition(_currentQChargeTime);
                    var pos = pred.Extend(Player.Instance, bladeStart + target.BoundingRadius).To3D();
                    return pos;
                }
            }

            return null;
        }

        private static bool canHitBlade(Obj_AI_Base target)
        {
            var chargeTime = _currentQChargeTime;
            var pred = target.PrediectPosition(chargeTime);
            var mypred = Player.Instance.PrediectPosition(chargeTime);

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

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!sender.IsMe)
                return;

            if (args.SData.Name.Equals("DariusCleave"))
                _lastQCast = Core.GameTickCount;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var e in EntityManager.Heroes.Enemies)
            {
                e.DrawDamage(DariusStuff.ComboDamage(e));
            }

            var target = TargetSelector.GetTarget(Q.Range * 2, DamageType.Physical);
            if (target != null && canHitBlade(target))
            {
                var pred = target.PrediectPosition(_currentQChargeTime);
                var pos = pred.Extend(Player.Instance, bladeStart + target.BoundingRadius).To3D();
                pos.DrawCircle(100, SharpDX.Color.Red);
            }
            Circle.Draw(SharpDX.Color.AliceBlue, outerBlade, Player.Instance);
            Circle.Draw(SharpDX.Color.AliceBlue, bladeStart, Player.Instance);

            Drawing.DrawText(Player.Instance.ServerPosition.WorldToScreen(), Color.AliceBlue, IsChargingQ.ToString(), 10);
        }

        public override void OnTick()
        {
        }

        public override void Combo()
        {
            if(this.menu.CheckBoxValue("q"))
                ComboQ();

            if(this.menu.CheckBoxValue("r"))
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

        }

        private static void ComboQ()
        {
            var target = TargetSelector.GetTarget(Q.Range * 2, DamageType.Physical);
            if (target != null && canHitBlade(target))
            {
                Q.Cast(Player.Instance.ServerPosition);
            }
        }

        private static void ComboR()
        {
            var ksTarget = EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault(t => t.IsKillable(R.Range) && !t.WillDie(R) && DariusStuff.Rdmg(t) > t.TotalShieldHealth());
            if (ksTarget != null && (R.IsReady() || DariusStuff.HasDariusUltResetBuff))
            {
                R.Cast(ksTarget);
            }
        }
    }
}
