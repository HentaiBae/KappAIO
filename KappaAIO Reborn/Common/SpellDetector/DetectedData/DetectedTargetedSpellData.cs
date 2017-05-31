using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;
using SharpDX;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedTargetedSpellData
    {
        public AIHeroClient Caster;
        public Obj_AI_Base Target;
        public MissileClient Missile;
        public Vector3 Start;
        public TargetedSpellData Data;
        private float? _speed;
        public float Speed
        {
            get { return this._speed ?? this.Data.Speed; }
            set { _speed = value; }
        }
        public float CastDelay => this.Missile != null && this.Data.Speed > 0 && this.Speed < int.MaxValue ? 0 : this.Data.CastDelay;
        public float MaxTravelTime => this.Start.Distance(this.Target.ServerPosition) / this.Speed * 1000 + this.CastDelay;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.MaxTravelTime;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        private bool? _ended;
        public bool Ended {get {return this._ended ?? this.TicksLeft <= 0; } set { this._ended = value; } }

        public float GetSpellDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0 : this.Caster.GetSpellDamage(target, this.Data.slot);
        }

        public bool WillHit(Obj_AI_Base target)
        {
            if (target == null)
                return false;

            if (this.Data.WindWall && Prediction.Position.Collision.GetYasuoWallCollision(this.Missile?.Position ?? this.Caster.ServerPosition, target.ServerPosition).IsValid())
            {
                return false;
            }

            return this.Target.IdEquals(target);
        }
    }
}
