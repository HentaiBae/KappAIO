using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using SharpDX;

namespace KappAIO_Reborn.Common.Utility
{
    static class Extensions
    {
        #region vectors

        public static bool IsUnderEnemyTurret(this Vector3 pos)
        {
            return EntityManager.Turrets.Enemies.Any(t => t.IsValidTarget(t.GetAutoAttackRange(), false, pos));
        }

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
        ///     Prediects the target Position on the given time
        /// </summary>
        public static Vector3 PrediectPosition(this Obj_AI_Base target, float Time = 250)
        {
            if (Time == 250)
                Time += Game.Ping;
            return Prediction.Position.PredictUnitPosition(target, (int)Time).To3D();
        }

        public static Vector2 HpBarPos(this Obj_AI_Base target) => new Vector2(target.HPBarPosition.X, target.HPBarPosition.Y - 5);

        #endregion

        #region Spell Extensions
        
        public static bool IsSummonerSpell(this SpellSlot slot)
        {
            return slot == SpellSlot.Summoner1 || slot == SpellSlot.Summoner2;
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

            if (!target.IsHPBarRendered || !target.HpBarPos().IsOnScreen())
                return;

            var y2 = target.BaseSkinName.Equals("Annie") ? 12 : target.BaseSkinName.Equals("Jhin") ? 14 : 0;

            var twoThirdhealth = (target.TotalShieldHealth() / 3) * 2;
            float x = target.HpBarPos().X;
            float y = target.HpBarPos().Y - y2 - 12;

            text.Color = System.Drawing.Color.White;

            if (dmg >= target.Health)
            {
                text.Color = System.Drawing.Color.Red;
            }
            else
            {
                if (dmg > twoThirdhealth)
                {
                    text.Color = System.Drawing.Color.Yellow;
                }
            }

            text.TextValue = (int)dmg + " / " + (int)target.Health;
            text.Position = new Vector2(x, y);
            text.Draw();
        }

        public static IEnumerable<AIHeroClient> GetKillStealTargets(this Spell.SpellBase spell)
        {
            return EntityManager.Heroes.Enemies.OrderByDescending(TargetSelector.GetPriority).Where(o => spell.WillKill(o) && o.IsKillable());
        }

        public static AIHeroClient GetKillStealTarget(this Spell.SpellBase spell, float dmg = 0, DamageType damageType = DamageType.Mixed)
        {
            if (dmg > 0)
            {
                return EntityManager.Heroes.Enemies.OrderBy(TargetSelector.GetPriority).FirstOrDefault(t => t.IsKillable(spell.Range, false, true, true) && Player.Instance.CalculateDamageOnUnit(t, damageType, dmg) > spell.GetHealthPrediction(t));
            }
            return spell.GetKillStealTargets().FirstOrDefault(o => o.IsKillable(spell.Range));
        }

        public static float GetTravelTime(this Spell.Skillshot spell, Vector3 end)
        {
            return Player.Instance.ServerPosition.Distance(end) / spell.Speed * 1000 + spell.CastDelay;
        }

        public static string GetSpellName(this SpellDataInst instance)
        {
            if (instance.Name.Contains("SummonerSmite"))
                return "SummonerSmite";

            if (instance.Name.Equals("TeleportCancel"))
                return "SummonerTeleport";

            if (instance.Name.Equals("SnowballFollowupCast"))
                return "SummonerSnowball";

            return instance.Name;
        }

        /// <summary>
        ///     Casts spell with selected hitchancepercent.
        /// </summary>
        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, float hitchancepercent)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChancePercent >= hitchancepercent)
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static SpellDataInst SlotToSpell(this AIHeroClient hero, SpellSlot slot)
        {
            return hero.Spellbook.GetSpell(slot);
        }

        public static string CoolDown(this SpellDataInst spell)
        {
            return ToTimeSpan((spell.CooldownExpires - Game.Time) + 1);
        }

        public static string ToTimeSpan(this float num)
        {
            var t = num;
            var ts = TimeSpan.FromSeconds(t);
            var s = t > 60 ? $"{ts.Minutes}:{ts.Seconds:D2}" : t > 3 ? $"{t:0}" : $"{t:0}.{ts.Milliseconds.ToString("D")}";
            return s;
        }

        public static float CurrentCD(this SpellDataInst spell)
        {
            var t = spell.CooldownExpires - Game.Time;
            return t;
        }

        public static float CurrentCD(this AIHeroClient hero, SpellSlot slot)
        {
            var spell = SlotToSpell(hero, slot);
            return spell.CurrentCD();
        }

        public static bool IsOnCoolDown(this AIHeroClient hero, SpellSlot slot)
        {
            return CurrentCD(hero, slot) > 0;
        }

        public static List<SpellSlot> KnownSpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R, SpellSlot.Summoner1, SpellSlot.Summoner2 };

        #endregion
        
        #region custom XP api

        public static float XPToCurrentLevel(this AIHeroClient hero)
        {
            var Totalxp = new[] { 280, 660, 1140, 1720, 2400, 3180, 4060, 5040, 6120, 7300, 8580, 9960, 11440, 13020, 14700, 16480, 18360 };
            var aram = 0;
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                if (hero.Level <= 3)
                    return 0;

                aram = -2;
            }
            if (Game.MapId == GameMapId.CrystalScar)
            {
                var csTotalXPSum = new[] { 790, 1245, 1785, 2410, 3110, 3900, 4770, 5645, 6575, 7560, 8590, 9660, 10935, 12290, 13715 };
                return hero.Level <= 3 ? 0 : csTotalXPSum[Math.Min(17, hero.Level - 4)];
            }
            return hero.Level == 1 ? 0 : Totalxp[Math.Min(17, hero.Level - 2)] + aram;
        }

        public static float XPNeededToNextLevel(this AIHeroClient hero)
        {
            if (hero.Level >= 18)
            {
                return 0;
            }
            if (Game.MapId == GameMapId.CrystalScar)
            {
                var csTotalXP = new[] { 790, 455, 540, 625, 700, 790, 870, 875, 930, 985, 1030, 1070, 1275, 1355, 1425 };
                return csTotalXP[Math.Min(15, hero.Level <= 3 ? 0 : hero.Level - 3)];
            }
            var aramstart = Game.MapId == GameMapId.HowlingAbyss && hero.Level <= 3;
            var baseexpneeded = 180 + hero.Level * 100;

            return aramstart ? 1138 : baseexpneeded;
        }

        public static float CurrentXP(this AIHeroClient hero)
        {
            if (hero.Level >= 18)
            {
                return 0;
            }
            var TotalXp = TotalXP(hero);
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                if (hero.Level <= 3)
                    return TotalXp;
            }
            var currnet = TotalXp - XPToCurrentLevel(hero);
            return hero.Level == 1 ? TotalXp : currnet;
        }

        public static float CurrentXPPercent(this AIHeroClient hero)
        {
            return Math.Max(0, CurrentXP(hero) / XPNeededToNextLevel(hero) * 100);
        }

        public static float TotalXP(this AIHeroClient hero)
        {
            return hero.Experience.XP;
        }

        #endregion

        #region Menu Extensions

        /// <summary>
        ///     Returns KeyBind Value.
        /// </summary>
        public static bool KeyBindValue(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<KeyBind>().CurrentValue;
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to get keybind value Name: {0} | MenuID: {1}", id, m.DisplayName);
                return false;
            }
        }

        /// <summary>
        ///     Returns CheckBox Value.
        /// </summary>
        public static bool CheckBoxValue(this Menu m, object id)
        {
            try
            {
                return m[id.ToString()].Cast<CheckBox>().CurrentValue;
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to get checkbox value Name: {0} | MenuID: {1}", id, m.DisplayName);
                return false;
            }
        }

        /// <summary>
        ///     Creates a checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            try
            {
                return m.Add(id, new CheckBox(name, defaultvalue));
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to create checkbox Name: {0} | MenuID: {1}", id, m.DisplayName);
                return null;
            }
        }

        /// <summary>
        ///     Creates a slider.
        /// </summary>
        public static Slider CreateSlider(this Menu m, string id, string name, int defaultvalue = 0, int MinValue = 0, int MaxValue = 100)
        {
            try
            {
                return m.Add(id, new Slider(name, defaultvalue, MinValue, MaxValue));
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to create slider Name: {0} | MenuID: {1}", id, m.DisplayName);
                return null;
            }
        }

        /// <summary>
        ///     Creates a KeyBind.
        /// </summary>
        public static KeyBind CreateKeyBind(this Menu m, string id, string name, bool defaultvalue, KeyBind.BindTypes BindType, uint key1 = 27U, uint key2 = 27U)
        {
            try
            {
                return m.Add(id, new KeyBind(name, defaultvalue, BindType, key1, key2));
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to create keybind Name: {0} | MenuID: {1}", id, m.DisplayName);
                return null;
            }
        }

        /// <summary>
        ///     Returns ComboBox Value.
        /// </summary>
        public static int ComboBoxValue(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<ComboBox>().CurrentValue;
            }
            catch (Exception)
            {
                Logger.Error("KappAIO: Failed to get combobox value Name: {0} | MenuID: {1}", id, m.DisplayName);
                return 0;
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
                Logger.Error("KappAIO: Failed to get slider value Name: {0} | MenuID: {1}", id, m.DisplayName);
                return 0;
            }
        }

        #endregion

        #region Gameobjects Extensions

        public static float DeathDuration(this AIHeroClient hero)
        {
            var spawntime = 0f;

            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                return Math.Max(10f, hero.Level * 2f + 4f);
            }

            var currentminutes = Game.Time / 60f;

            float BRW = hero.Level * 2.5f + 7.5f;

            if (currentminutes > 15f && currentminutes < 30f)
            {
                spawntime = BRW + ((BRW / 100f) * (currentminutes - 15f) * 2f * 0.425f);
            }

            if (currentminutes > 30f && currentminutes < 45f)
            {
                spawntime = BRW + ((BRW / 100f) * (currentminutes - 15f) * 2f * 0.425f) + ((BRW / 100f) * (currentminutes - 30f) * 2f * 0.30f);
            }

            if (currentminutes > 45f && currentminutes < 53.5f)
            {
                spawntime = BRW + ((BRW / 100f) * (currentminutes - 15f) * 2f * 0.425f) + ((BRW / 100f) * (currentminutes - 30f) * 2f * 0.30f) * ((BRW / 100f) * (currentminutes - 45f) * 2f * 1.45f);
            }

            if (currentminutes > 53.5f)
            {
                spawntime = (BRW + ((BRW / 100f) * (currentminutes - 15f) * 2f * 0.425f) + ((BRW / 100f) * (currentminutes - 30f) * 2f * 0.30f) + ((BRW / 100f) * (currentminutes - 45f) * 2f * 1.45f)) * 1.5f;
            }

            if (spawntime.Equals(0f))
            {
                spawntime = BRW;
            }

            return spawntime;
        }

        private static Dictionary<string, string> reviveBuffs = new Dictionary<string, string>
            {
            {"all", "willrevive" },
            {"Aatrox", "aatroxpassiveready" },
            {"Zac", "zacrebirthready" },
            {"Anivia", "rebirthready" }
            };

        /// <summary>
        ///     Returns true if you can deal damage to the target.
        /// </summary>
        public static bool IsKillable(this Obj_AI_Base target, float range = -1f, bool SpellShields = false, bool UndyingBuffs = false, bool ReviveBuffs = false)
        {
            if (range.Equals(-1f))
                range = int.MaxValue;

            if (target == null)
                return false;

            if (target.HasBuff("SionPassiveZombie"))
                return false;

            if (!target.IsValidTarget(range))
                return false;

            if (ReviveBuffs)
                return !target.HasReviveBuff();

            if (target.IsZombie || target.IsDead || target.Health <= 0)
                return false;

            if (UndyingBuffs && (target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuffOfType(BuffType.PhysicalImmunity) || (target.HasBuff("kindredrnodeathbuff") && target.HealthPercent <= 15) || target.IsInvulnerable))
                return false;

            if (SpellShields && (target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw") || b.Name.ToLower().Contains("sivire")) || target.HasBuff("bansheesveil")))
                return false;

            var client = target as AIHeroClient;
            if (client != null && UndyingBuffs && client.HasUndyingBuff(true))
                return false;

            return true;
        }

        public static bool IsChampion(this GameObject obj)
        {
            return obj != null && obj.IsValid && obj is AIHeroClient;
        }

        public static bool HasReviveBuff(this Obj_AI_Base target)
        {
            if (target == null)
                return false;

            if (reviveBuffs.ContainsKey(target.BaseSkinName))
            {
                if (target.HasBuff(reviveBuffs[target.BaseSkinName]))
                {
                    return true;
                }
            }

            return target.HasBuff(reviveBuffs["all"]);
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
                incDmg += incSkillshots.Sum(s => (s.Caster as AIHeroClient)?.GetSpellDamage(target, s.Data.Slots[0]) ?? 0);
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
        ///     Returns true if the target will die before the spell finish him.
        /// </summary>
        public static bool WillDie(this Obj_AI_Base target, float delay = 250)
        {
            var incDmg = 0f;

            var incSkillshots = SkillshotDetector.SkillshotsDetected.FindAll(s => s.WillHit(target));
            if (incSkillshots.Any())
                incDmg += incSkillshots.Sum(s => (s.Caster as AIHeroClient)?.GetSpellDamage(target, s.Data.Slots[0]) ?? 0);
            var incTargetedSpell = TargetedSpellDetector.DetectedTargetedSpells.FindAll(s => s.Target.IdEquals(target));
            if (incTargetedSpell.Any())
                incDmg += incTargetedSpell.Sum(s => s.Caster.GetSpellDamage(target, s.Data.slot));
            var incDangerBuff = DangerBuffDetector.DangerBuffsDetected.FindAll(s => s.Target.IdEquals(target));
            if (incDangerBuff.Any())
                incDmg += incDangerBuff.Sum(s => s.Caster.GetSpellDamage(target, s.Data.Slot));
            var incAA = EmpoweredAttackDetector.DetectedEmpoweredAttacks.FindAll(s => s.Target.IdEquals(target));
            if (incAA.Any())
                incDmg += incAA.Sum(s => s.Caster.GetAutoAttackDamage(target, true));

            return target.PredictHealth((int)delay) <= incDmg;
        }

        /// <summary>
        ///     Returns true if the spell will kill the target.
        /// </summary>
        public static bool WillKill(this Spell.SpellBase spell, Obj_AI_Base target, float MultiplyDmgBy = 1, float ExtraDamage = 0, DamageType ExtraDamageType = DamageType.True)
        {
            return Player.Instance.GetSpellDamage(target, spell.Slot) * MultiplyDmgBy + Player.Instance.CalculateDamageOnUnit(target, ExtraDamageType, ExtraDamage) >= spell.GetHealthPrediction(target) && !target.WillDie(spell);
        }
        /// <summary>
        ///     Returns true if the spell will kill the target.
        /// </summary>
        public static bool WillKill(this Obj_AI_Base target, float rawDamage, float MultiplyDmgBy = 1, float ExtraDamage = 0, DamageType ExtraDamageType = DamageType.True)
        {
            return rawDamage * MultiplyDmgBy + Player.Instance.CalculateDamageOnUnit(target, ExtraDamageType, ExtraDamage) >= target.PredictHealth() && !target.WillDie();
        }

        public static bool CanKill(this AIHeroClient hero, AIHeroClient target)
        {
            return hero.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() && target.IsKillable(hero.GetAutoAttackRange(target));
        }

        /// <summary>
        ///     Returns true if the target is big minion (Siege / Super Minion).
        /// </summary>
        public static bool IsBigMinion(this Obj_AI_Base target)
        {
            return target.BaseSkinName.ToLower().Contains("siege") || target.BaseSkinName.ToLower().Contains("super");
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

        public static float PredictHealth(this Obj_AI_Base target, float Time = 250f)
        {
            if (Time == 250f)
                Time += Game.Ping;
            return Prediction.Health.GetPrediction(target, (int)Time);
        }

        public static string GetChampionName(this AIHeroClient target)
        {
            if (target.ChampionName == "FiddleSticks")
                return "Fiddlesticks";

            return target.ChampionName;
        }

        public static Obj_SpawnPoint GetSpawnPoint(this AIHeroClient target)
        {
            return ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(s => s.Team == target.Team);
        }

        #endregion
        
        public static DangerLevel[] DangerLevels = {DangerLevel.Low, DangerLevel.Medium, DangerLevel.High, };
        public static List<DangerBuffTypes> CCBuffTypes = new List<DangerBuffTypes>
            {
                //new DangerBuffTypes(BuffType.AttackSpeedSlow, 2),
                new DangerBuffTypes(BuffType.Blind, 4),
                new DangerBuffTypes(BuffType.Charm, 5),
                new DangerBuffTypes(BuffType.CombatDehancer, 1),
                new DangerBuffTypes(BuffType.Damage, 1),
                new DangerBuffTypes(BuffType.Disarm, 1),
                new DangerBuffTypes(BuffType.Fear, 5),
                new DangerBuffTypes(BuffType.Flee, 3),
                //new DangerBuffTypes(BuffType.Grounded, 3),
                new DangerBuffTypes(BuffType.Knockback, 4),
                new DangerBuffTypes(BuffType.Knockup, 4),
                new DangerBuffTypes(BuffType.NearSight, 2),
                new DangerBuffTypes(BuffType.Poison, 1),
                new DangerBuffTypes(BuffType.Polymorph, 5),
                new DangerBuffTypes(BuffType.Slow, 2),
                new DangerBuffTypes(BuffType.Shred, 1),
                new DangerBuffTypes(BuffType.Silence, 3),
                new DangerBuffTypes(BuffType.Snare, 5),
                new DangerBuffTypes(BuffType.Suppression, 5),
                new DangerBuffTypes(BuffType.Stun, 5),
                new DangerBuffTypes(BuffType.Taunt, 5),
            };

    }

    public class DangerBuffTypes
    {
        public DangerBuffTypes(BuffType b, int d)
        {
            this.BuffType = b;
            this.DangerLevel = d;
        }
        public BuffType BuffType;
        public int DangerLevel = 1;
    }
}
