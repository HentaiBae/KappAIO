using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using SharpDX;

namespace KappAIO_Reborn.Common.Utility
{
    static class Extensions
    {
        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static Vector3 CenterVectors(this List<Vector3> vectors)
        {
            return vectors.ToArray().CenterVectors();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static Vector3 CenterVectors(this IEnumerable<Vector3> vectors)
        {
            return vectors.ToArray().CenterVectors();
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static Vector3 CenterVectors(this Vector3[] vectors)
        {
            return vectors.Aggregate(Vector3.Zero, (current, vector) => current + vector) / vectors.Length;
        }

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target, float range = -1f, bool SpellShields = false, bool UndyingBuffs = false, bool ReviveBuffs = false)
        {
            if (range.Equals(-1f))
                range = int.MaxValue;

            if (target == null)
                return false;

            if (!target.IsValidTarget(range))
                return false;

            if (target.IsZombie || target.IsDead || target.Health <= 0)
                return false;

            if (UndyingBuffs && (target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuffOfType(BuffType.PhysicalImmunity) || (target.HasBuff("kindredrnodeathbuff") && target.HealthPercent <= 15) || target.IsInvulnerable))
                return false;

            if (SpellShields && (target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw") || b.Name.ToLower().Contains("sivire")) || target.HasBuff("bansheesveil")))
                return false;

            var client = target as AIHeroClient;
            if (client != null)
                return UndyingBuffs && !client.HasUndyingBuff(true);

            if (ReviveBuffs)
                return !target.HasReviveBuff();

            return true;
        }

        private static Dictionary<string, string> reviveBuffs = new Dictionary<string, string>
            {
            {"Aatrox", "aatroxpassiveready" },
            {"Zac", "zacrebirthready" },
            {"Anivia", "rebirthready" }
            };

        public static bool HasReviveBuff(this Obj_AI_Base target)
        {
            if (target == null || !reviveBuffs.ContainsKey(target.BaseSkinName))
                return false;

            var buff = reviveBuffs.FirstOrDefault(s => s.Key.Equals(target.BaseSkinName));

            return target.HasBuff(buff.Value);
        }

        /// <summary>
        ///     Returns true if the target will die before the spell finish him.
        /// </summary>
        public static bool WillDie(this Obj_AI_Base target, Spell.SpellBase spell)
        {
            if (spell.CastDelay == 0)
                spell.CastDelay = 250;

            var incDmg = 0f;
            var delay = spell.CastDelay;

            var incSkillshots = SkillshotDetector.SkillshotsDetected.FindAll(s => s.WillHit(target));
            if (incSkillshots.Any())
                incDmg += incSkillshots.Sum(s => s.Caster.GetSpellDamage(target, s.Data.slot));
            var incTargetedSpell = TargetedSpellDetector.DetectedTargetedSpells.FindAll(s => s.Target.IdEquals(target));
            if (incTargetedSpell.Any())
                incDmg += incTargetedSpell.Sum(s => s.Caster.GetSpellDamage(target, s.Data.slot));
            var incDangerBuff = DangerBuffDetector.DangerBuffsDetected.FindAll(s => s.Target.IdEquals(target));
            if (incDangerBuff.Any())
                incDmg += incDangerBuff.Sum(s => s.Caster.GetSpellDamage(target, s.Data.Slot));
            var incAA = EmpoweredAttackDetector.DetectedEmpoweredAttacks.FindAll(s => s.Target.IdEquals(target));
            if (incAA.Any())
                incDmg += incAA.Sum(s => s.Caster.GetAutoAttackDamage(target, true));

            return spell.GetHealthPrediction(target) <= 0f || target.PredictHealth(delay) <= incDmg;
        }

        /// <summary>
        ///     Returns a recreated name of the target.
        /// </summary>
        public static string Name(this Obj_AI_Base target)
        {
            if (ObjectManager.Get<Obj_AI_Base>().Count(o => o.BaseSkinName.Equals(target.BaseSkinName)) > 1)
            {
                return target.BaseSkinName + "(" + target.Name + ")";
            }
            return target.BaseSkinName;
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            return m.Add(id, new CheckBox(name, defaultvalue));
        }

        /// <summary>
        ///     Creates a slider.
        /// </summary>
        public static Slider CreateSlider(this Menu m, string id, string name, int defaultvalue = 0, int MinValue = 0, int MaxValue = 100)
        {
            return m.Add(id, new Slider(name, defaultvalue, MinValue, MaxValue));
        }

        /// <summary>
        ///     Creates a KeyBind.
        /// </summary>
        public static KeyBind CreateKeyBind(this Menu m, string id, string name, bool defaultvalue, KeyBind.BindTypes BindType, uint key1 = 27U, uint key2 = 27U)
        {
            return m.Add(id, new KeyBind(name, defaultvalue, BindType, key1, key2));
        }

        /// <summary>
        ///     Returns KeyBind Value.
        /// </summary>
        public static bool KeyBindValue(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Returns ComboBox Value.
        /// </summary>
        public static int ComboBoxValue(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<CheckBox>().CurrentValue;
            }
            catch (Exception)
            {
                Console.WriteLine($"Wrong ID:- Menu: {m.DisplayName} - ID: {id}");
                return false;
            }
        }

        /// <summary>
        ///     Returns Slider Value.
        /// </summary>
        public static int SliderValue(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<Slider>().CurrentValue;
            }
            catch (Exception)
            {
                Console.WriteLine($"Wrong ID:- Menu: {m.DisplayName} - ID: {id}");
                return 0;
            }
        }

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static bool IsBigMinion(this Obj_AI_Base target)
        {
            return target.BaseSkinName.ToLower().Contains("siege") || target.BaseSkinName.ToLower().Contains("super");
        }

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static Vector3 PrediectPosition(this Obj_AI_Base target, float Time = 250)
        {
            if (Time == 250)
                Time += Game.Ping;
            return Prediction.Position.PredictUnitPosition(target, (int)Time).To3D();
        }

        public static float PredictHealth(this Obj_AI_Base target, int Time = 250)
        {
            if (Time == 250)
                Time += Game.Ping;
            return Prediction.Health.GetPrediction(target, Time);
        }

        public static float GetCurrentDamage(this float[] damages, float[] mods, SpellSlot slot, float scale, int index = -1)
        {
            index = index == -1 ? Player.Instance.Spellbook.GetSpell(slot).Level - 1 : index;

            if (index < 0)
                return 0;

            var dmg = damages[index];
            var mod = mods[index];
            var currentdmg = dmg + (scale * mod);

            return currentdmg;
        }

        private static Text text;
        public static void DrawDamage(this Obj_AI_Base target, float dmg)
        {
            if (text == null)
                text = new Text(string.Empty, new Font("Tahoma", 9, FontStyle.Bold)) { Color = System.Drawing.Color.White };

            if (!target.IsHPBarRendered || !target.HPBarPosition.IsOnScreen())
                return;
            
            var y2 = target.BaseSkinName.Equals("Annie") ? 12 : target.BaseSkinName.Equals("Jhin") ? 14 : 0;

            float x = target.HPBarPosition.X;
            float y = target.HPBarPosition.Y - y2 - 12;
            text.Color = System.Drawing.Color.White;
            if (dmg >= target.Health)
            {
                text.Color = System.Drawing.Color.Red;
            }
            text.TextValue = (int)dmg + " / " + (int)target.Health;
            text.Position = new Vector2(x, y);
            text.Draw();
        }
    }
}
