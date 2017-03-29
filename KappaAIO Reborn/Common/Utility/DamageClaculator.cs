using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappAIO_Reborn.Common.Utility
{
    public class RawDamage
    {
        public delegate float PreCalculated();
        public PreCalculated PreCalculatedDamage;
        public Obj_AI_Base Source;
        public Spell.SpellBase Spell;
        public int Stage;
    }

    public class CalculatedDamage
    {
        public RawDamage rawDamage;
        public Obj_AI_Base Target;
        public float lastCalculated;
        private float? cachedDamage;
        public bool ShouldRecalculate => Core.GameTickCount - this.lastCalculated > 100 + Game.Ping / 2;

        public float GetDamage()
        {
            if (!this.rawDamage.Spell.IsReady())
                return 0;

            if (this.cachedDamage.HasValue && !this.ShouldRecalculate)
                return this.cachedDamage.Value;

            this.lastCalculated = Core.GameTickCount;
            this.cachedDamage = this.rawDamage.Source.CalculateDamageOnUnit(this.Target, this.rawDamage.Spell.DamageType, this.rawDamage.PreCalculatedDamage());
            return this.cachedDamage.Value;
        }
    }

    public static class DamageClaculator
    {
        static DamageClaculator()
        {
            if (!invoked)
            {
                invoked = true;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            }
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!_calculatedDamages.Any(c => c.Value.Target.IdEquals(sender) || c.Value.rawDamage.Source.IdEquals(sender) || (args.Target != null && c.Value.Target.IdEquals(args.Target))))
                return;

            foreach (var d in _calculatedDamages.Where(c => c.Value.Target.IdEquals(sender) || c.Value.rawDamage.Source.IdEquals(sender) || (args.Target != null && c.Value.Target.IdEquals(args.Target))))
            {
                d.Value.lastCalculated = 0;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!_calculatedDamages.Any(c => c.Value.Target.IdEquals(sender) || c.Value.rawDamage.Source.IdEquals(sender) || (args.Target != null && c.Value.Target.IdEquals(args.Target))))
                return;

            foreach (var d in _calculatedDamages.Where(c => c.Value.Target.IdEquals(sender) || c.Value.rawDamage.Source.IdEquals(sender) || (args.Target != null && c.Value.Target.IdEquals(args.Target))))
            {
                d.Value.lastCalculated = 0;
            }
        }

        private static List<RawDamage> _rawDamages = new List<RawDamage>();
        private static Dictionary<object, CalculatedDamage> _calculatedDamages = new Dictionary<object, CalculatedDamage>();

        private static bool invoked;
        public static void AddRawDamage(this Spell.SpellBase spell, RawDamage rawDamage)
        {
            _rawDamages.Add(rawDamage);
        }

        public static float CalculateDamage(this Spell.SpellBase spell, Obj_AI_Base target, Obj_AI_Base source = null, int stage = 1)
        {
            if (source == null)
                source = Player.Instance;

            if (target == null)
                return 0;

            object key = spell.Slot + target.NetworkId + source.NetworkId + stage;
            if (_calculatedDamages.ContainsKey(key))
            {
                var damage = _calculatedDamages[key];
                return damage.GetDamage();
            }

            var dmg = new CalculatedDamage
                {
                    rawDamage = _rawDamages.FirstOrDefault(s => s.Source.IdEquals(source) && s.Spell.Equals(spell) && s.Stage.Equals(stage)),
                    Target = target
                };

            _calculatedDamages.Add(key, dmg);

            return dmg.GetDamage();
        }
    }
}
