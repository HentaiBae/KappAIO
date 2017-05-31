using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;
using SharpDX;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedEmpoweredAttackData
    {
        public AIHeroClient Caster;
        public Obj_AI_Base Target;
        public MissileClient Missile;
        public EmpoweredAttackData Data;
        public Vector3 Start;
        public float AttackCastDelay;
        public float Speed;
        public float MaxTravelTime => this.Start.Distance(this.Target) / this.Speed * 1000f + this.AttackCastDelay;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.MaxTravelTime;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => this.StartTick - Core.GameTickCount;
        private bool? _ended;
        public bool Ended
        {
            get
            {
                return this._ended ?? (this.TicksLeft <= 0 || this.TicksPassed > this.AttackCastDelay + Game.Ping && this.Caster.IsMelee);
            }
            set
            {
                this._ended = value;
            }
        }

        public float GetSpellDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0 : this.Caster.GetAutoAttackDamage(target, true);
        }

        public bool WillHit(Obj_AI_Base target)
        {
            if (target == null)
                return false;

            if (this.Caster.IsRanged && Prediction.Position.Collision.GetYasuoWallCollision(this.Caster.ServerPosition, target.ServerPosition).IsValid())
            {
                return false;
            }

            return this.Target.IdEquals(target);
        }
    }
}
