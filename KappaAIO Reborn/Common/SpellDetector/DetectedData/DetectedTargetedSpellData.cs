using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedTargetedSpellData
    {
        public AIHeroClient Caster;
        public Obj_AI_Base Target;
        public TargetedSpellData Data;
        public float StartTick = Core.GameTickCount;
        public float EndTick => this.StartTick + this.Data.CastDelay;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public bool Ended => this.TicksLeft <= 0;
    }
}
