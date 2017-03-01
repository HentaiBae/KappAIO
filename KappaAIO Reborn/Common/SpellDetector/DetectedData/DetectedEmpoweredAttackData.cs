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
        public EmpoweredAttackData Data;
        public Vector3 Start;
        public float AttackCastDelay;
        public float Speed;
        public float MaxTravelTime => this.Start.Distance(this.Target) / this.Speed * 1000f + this.AttackCastDelay;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.MaxTravelTime;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => this.StartTick - Core.GameTickCount;
        public bool Ended => this.TicksLeft <= 0 || this.TicksPassed > 1000 && this.Caster.IsMelee;

        public bool WillHit(Obj_AI_Base target)
        {
            if (target == null)
                return false;

            return this.Target != null && this.Target.IdEquals(target);
        }
    }
}
