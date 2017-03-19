using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Utility;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedDangerBuffData
    {
        public AIHeroClient Caster;
        public Obj_AI_Base Target;
        public DangerBuffData Data;
        public BuffInstance Buff;
        public float StartTick = Core.GameTickCount;
        public float EndTick
        {
            get
            {
                if (this.Data.Delay > 0 && this.Data.Delay < int.MaxValue)
                {
                    return this.StartTick + this.Data.Delay;
                }
                if (this.Buff != null)
                {
                    return this.Buff.EndTime * 1000f;
                }
                return this.StartTick + 2000f;
            }
        }
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public bool Ended => this.TicksLeft <= 0 || (this.Caster != null && this.Caster.IsDead) || (this.Target != null && this.Target.IsDead);

        public bool WillHit(Obj_AI_Base target)
        {
            if (target == null)
                return false;

            if (this.Data.IsRanged)
            {
                return this.Caster != null && target.IsInRange(this.Caster.PrediectPosition(TicksLeft), this.Data.Range);
            }

            return this.Target != null && this.Target.IdEquals(target);
        }
    }
}
