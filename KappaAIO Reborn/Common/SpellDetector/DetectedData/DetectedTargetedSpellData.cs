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
        public TargetedSpellData Data;
        public Vector3 Start;
        public float TravelTime => this.Start.Distance(this.Target.ServerPosition) / this.Data.Speed * 1000;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.Data.CastDelay + this.TravelTime;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        public bool Ended => this.TicksLeft <= 0;
    }
}
