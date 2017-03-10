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
        public float CastDelay => this.Missile != null ? 0 : this.Data.CastDelay;
        public float MaxTravelTime => this.Start.Distance(this.Target.ServerPosition) / this.Data.Speed * 1000 + this.CastDelay;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.MaxTravelTime;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        public bool Ended => this.TicksLeft <= 0;

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
