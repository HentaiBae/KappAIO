using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using KappAIO_Reborn.Common.SpellDetector.Events;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using static KappAIO_Reborn.Plugins.Champions.Fiora.Fiora;

namespace KappAIO_Reborn.Plugins.Champions.Fiora
{
    public static class FioraStuff
    {
        public static class VitalManager
        {
            public static void Init()
            {
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;

                foreach (var emitter in ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(FioraPassive))
                {
                    if (emitter.IsEnemy)
                    {
                        var passive = new FioraVital(emitter) { startTick = Core.GameTickCount };
                        if (!StoredPassives.Contains(passive))
                            StoredPassives.Add(passive);
                    }
                }
            }

            private static void GameObject_OnCreate(GameObject sender, EventArgs args)
            {
                var emitter = sender as Obj_GeneralParticleEmitter;
                if (emitter != null && emitter.Name.Contains("Fiora"))
                {
                    if (FioraPassive(emitter) && emitter.IsEnemy)
                    {
                        StoredPassives.RemoveAll(v => (v.Vital != null && (v.Vital.IsDead || !v.Vital.IsValid)) || (v.Caster != null && (!v.Caster.IsValid || v.Caster.IsDead)) || Core.GameTickCount - v.startTick > 15000);
                        var passive = new FioraVital(emitter) { startTick = Core.GameTickCount };
                        if (!StoredPassives.Contains(passive))
                            StoredPassives.Add(passive);
                    }
                }
            }

            private static void GameObject_OnDelete(GameObject sender, EventArgs args)
            {
                var emitter = sender as Obj_GeneralParticleEmitter;
                if (emitter != null && emitter.IsEnemy)
                {
                    if (FioraPassive(emitter))
                    {
                        if (StoredPassives.Any(p => p.Vital.IdEquals(emitter)))
                            StoredPassives.RemoveAll(v => (v.Vital != null && (v.Vital.IdEquals(emitter) || v.Vital.IsDead || !v.Vital.IsValid)) || (v.Caster != null && (!v.Caster.IsValid || v.Caster.IsDead)) || Core.GameTickCount - v.startTick > 15000);
                    }
                }
            }

            internal static string[] PassiveBuffNames = { "fiorapassivemanager", "fiorarmark" };

            internal static bool HasFioraPassiveBuff(AIHeroClient hero)
            {
                return PassiveBuffNames.Any(hero.HasBuff);
            }

            public class FioraVital
            {
                public float startTick;
                public AIHeroClient Caster { get { return EntityManager.Heroes.AllHeroes.OrderBy(e => e.Distance(this.Vital)).FirstOrDefault(h => HasFioraPassiveBuff(h) && h.Distance(this.Vital) <= 50 + h.BoundingRadius); } }
                public Obj_GeneralParticleEmitter Vital;
                public bool ValidVital
                {
                    get
                    {
                        return !this.Vitalsector.Center.IsWall() && this.Vital.IsValid && !this.Vital.IsDead && !this.OrbWalkVitalPos.IsBuilding() 
                            && (this.Vital.Name.ToLower().Contains("timeout") || (!this.Vital.Name.ToLower().Contains("warning") || Core.GameTickCount - this.startTick > 1500 - Game.Ping));
                    }
                }

                public bool WillBeValid(float time)
                {
                    return (this.Vital.Name.ToLower().Contains("timeout") || (!this.Vital.Name.ToLower().Contains("warning") || Core.GameTickCount - this.startTick > 1500 - Game.Ping - time))
                        && !this.Vitalsector.Center.IsWall() && this.Vital.IsValid && !this.Vital.IsDead && !this.OrbWalkVitalPos.IsBuilding();
                }
                public bool IsRVital => this.Vital != null && this.Vital.Name.Contains("_R_Mark");
                public Vector3 OrbWalkVitalPos
                {
                    get
                    {
                        var range = 150;
                        var travelTime = Player.Instance.Distance(this.CorrectPos) / Player.Instance.MoveSpeed * 1000f;
                        var pos = this.Caster.PrediectPosition(travelTime);
                        var x2 = this.Vital.Name.Contains("_NW") ? range : this.Vital.Name.Contains("_SE") ? -range : 0;
                        var y2 = this.Vital.Name.Contains("_NE") ? range : this.Vital.Name.Contains("_SW") ? -range : 0;

                        return new Vector3(pos.X + x2, pos.Y + y2, pos.Z);
                    }
                }
                public Vector3 QPredVitalPos
                {
                    get
                    {
                        var range = 150;
                        var x2 = this.Vital.Name.Contains("_NW") ? range : this.Vital.Name.Contains("_SE") ? -range : 0;
                        var y2 = this.Vital.Name.Contains("_NE") ? range : this.Vital.Name.Contains("_SW") ? -range : 0;
                        var predcaster = Q2.GetPrediction(this.Caster).CastPosition;
                        return new Vector3(predcaster.X + x2, predcaster.Y + y2, predcaster.Z);
                    }
                }
                public Vector3 CorrectPos
                {
                    get
                    {
                        var range = 175;
                        var pos = this.Caster.ServerPosition;
                        var x2 = this.Vital.Name.Contains("_NW") ? range : this.Vital.Name.Contains("_SE") ? -range : 0;
                        var y2 = this.Vital.Name.Contains("_NE") ? range : this.Vital.Name.Contains("_SW") ? -range : 0;
                        return new Vector3(pos.X + x2, pos.Y + y2, pos.Z);
                    }
                }
                public Geometry.Polygon.Sector QpredVitalsector { get { return VitalSector(this.Caster, this.QPredVitalPos); } }
                public Geometry.Polygon.Sector Vitalsector { get { return VitalSector(this.Caster, this.CorrectPos); } }

                public FioraVital(Obj_GeneralParticleEmitter target)
                {
                    this.Vital = target;
                }
            }

            private static readonly List<string> Directions = new List<string> { "NE", "NW", "SE", "SW" };

            public static bool FioraPassive(Obj_GeneralParticleEmitter emitter)
            {
                return emitter != null && emitter.IsValid && !emitter.IsDead
                       && (emitter.Name.Contains("Fiora_Base_R_Mark") || (emitter.Name.Contains("Fiora_Base_R") && emitter.Name.Contains("Timeout"))
                           || (emitter.Name.Contains("Fiora_Base_Passive") && Directions.Any(emitter.Name.Contains)));
            }

            public static readonly List<FioraVital> StoredPassives = new List<FioraVital>();

            public static IEnumerable<FioraVital> Vitals(AIHeroClient hero)
            {
                return StoredPassives.Where(v => v.Caster.IdEquals(hero));
            }

            public static FioraVital vital(AIHeroClient hero, bool Valid = false)
            {
                return Vitals(hero).OrderBy(v => v.OrbWalkVitalPos.Distance(Player.Instance)).FirstOrDefault(v => !Valid || v.ValidVital || v.WillBeValid(Q2.GetTravelTime(v.QPredVitalPos)));
            }

            public static Geometry.Polygon.Sector VitalSector(AIHeroClient start, Vector3 end)
            {
                return new Geometry.Polygon.Sector(start.ServerPosition, end, (float)(90f * Math.PI / 180), Player.Instance.AttackRange);
            }

            public static Vector3? CanQVital(FioraVital v, bool shortVital, bool longVital)
            {
                var target = v?.Caster;
                if (target == null)
                    return null;

                var center = v.QpredVitalsector.CenterOfPolygon().To3DWorld();
                var qpos = v.QPredVitalPos;

                if (shortVital)
                {
                    if(Q1.IsInRange(qpos))
                        return qpos;
                    if (Q1.IsInRange(center))
                        return center;
                }

                if (longVital)
                {
                    var vitdis = v.QPredVitalPos.Distance(Player.Instance, true);
                    var tardis = target.Distance(Player.Instance, true);
                    var farvit = tardis > vitdis || Q1.IsInRange(center);
                    var maxQCast = Player.Instance.ServerPosition.Extend(v.QPredVitalPos, Q2.IsInRange(v.QPredVitalPos) ? Player.Instance.Distance(v.QPredVitalPos) : Q2.Range).To3D();
                    var qrect = new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, maxQCast, Q2.Width + target.BoundingRadius);
                    var insidepoint = v.QpredVitalsector.Points.Any(p => qrect.IsInside(p)) && !qrect.IsInside(target);

                    if (Q2.IsInRange(v.QPredVitalPos) && (farvit || insidepoint))
                    {
                        return v.QPredVitalPos;
                    }
                }

                return null;
            }

            internal static bool CanWVital(AIHeroClient target)
            {
                var targetvital = vital(target, true);
                if (targetvital == null)
                    return false;
                var distancevital = Player.Instance.Distance(targetvital.OrbWalkVitalPos);
                var distancetarget = Player.Instance.Distance(target);
                return W.IsInRange(targetvital.OrbWalkVitalPos) && distancetarget > distancevital;
            }

            public static float VitalDamage(AIHeroClient target)
            {
                var vitaldamage = 0.02f + (0.045f * (Player.Instance.FlatPhysicalDamageMod / 100f)) * target.MaxHealth;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, vitaldamage);
            }

            public static float VitalsDamage(AIHeroClient target)
            {
                return Vitals(target).Count() * VitalDamage(target);
            }
        }

        public static class SpellManager
        {
            public static float QDamage(Obj_AI_Base target)
            {
                return Q1.CalculateDamage(target);
            }
            public static float WDamage(Obj_AI_Base target)
            {
                return W.CalculateDamage(target);
            }
            public static float EDamage(Obj_AI_Base target)
            {
                return E.CalculateDamage(target);
            }
            public static float RDamage(AIHeroClient target)
            {
                if (!R.IsLearned)
                    return 0;
                return (VitalManager.VitalDamage(target) + Player.Instance.GetAutoAttackDamage(target)) * 4f;
            }
            public static float ComboDamage(AIHeroClient target, bool r = true)
            {
                var qdmg = Q1.IsReady() ? QDamage(target) : 0;
                var wdmg = W.IsReady() ? WDamage(target) : 0;
                var edmg = E.IsReady() ? EDamage(target) : 0;
                var rdmg = r ? R.IsReady() ? RDamage(target) : VitalManager.VitalDamage(target) : VitalManager.VitalDamage(target);
                return qdmg + wdmg + edmg + rdmg;
            }
        }

        public static class SpellBlocker
        {
            public static void Init()
            {
                Game.OnTick += Game_OnTick;
                OnEmpoweredAttackDetected.OnDetect += OnEmpoweredAttackDetected_OnDetect;
                OnDangerBuffDetected.OnDetect += OnDangerBuffDetected_OnDetect;
                OnTargetedSpellDetected.OnDetect += OnTargetedSpellDetected_OnDetect;
                OnSkillShotDetected.OnDetect += OnSkillShotDetected_OnDetect;
                OnSpecialSpellDetected.OnDetect += OnSpecialSpellDetected_OnDetect;
                //Drawing.OnDraw += Drawing_OnDraw;
            }

            private static float _lastBlock;
            private static void Game_OnTick(EventArgs args)
            {
                if(delay > Core.GameTickCount - _lastBlock || !W.IsReady())
                    return;

                var BlockSpells = DetectedSpells.OrderByDescending(s => s.DangerLevel);
                if (BlockSpells != null && BlockSpells.Any())
                {
                    foreach (var BlockSpell in BlockSpells)
                    {
                        #region skillshot
                        var skillshot = BlockSpell.DetectedData as DetectedSkillshotData;
                        if (skillshot != null && Config.BlockSkillshots)
                        {
                            var dashInfo = Player.Instance.GetDashInfo();
                            if (dashInfo != null)
                            {
                                var travelTime = (Player.Instance.ServerPosition.Distance(dashInfo.EndPos) / Q1.Speed) * 1000f;
                                var predPos = Player.Instance.PrediectPosition(travelTime);
                                var skillshotpred = Player.Instance.PrediectPosition(skillshot.TravelTime(predPos.To2D()));
                                if(predPos.Distance(skillshotpred) > Player.Instance.BoundingRadius)
                                    continue; // skip if dashing outside of skillshot
                            }
                            if (skillshot.Ended || !SkillshotDetector.SkillshotsDetected.Contains(skillshot))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (skillshot.WillHit(Player.Instance))
                            {
                                if(skillshot.TravelTime(Player.Instance) <= delay || BlockSpell.FastEvade)
                                    CastW(skillshot.Caster, BlockSpell.SpellName, skillshot.Start);
                            }

                            break;
                        }
                        #endregion

                        #region buff
                        var buff = BlockSpell.DetectedData as DetectedDangerBuffData;
                        if (buff != null && Config.BlockBuff)
                        {
                            if (buff.Ended || !DangerBuffDetector.DangerBuffsDetected.Contains(buff))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (buff.WillHit(Player.Instance))
                            {
                                if (buff.TicksLeft <= delay)
                                    CastW(buff.Caster, BlockSpell.SpellName);
                            }

                            break;
                        }
                        #endregion

                        #region targted
                        var targted = BlockSpell.DetectedData as DetectedTargetedSpellData;
                        if (targted != null && Config.BlockTargeted)
                        {
                            if (targted.Ended || !TargetedSpellDetector.DetectedTargetedSpells.Contains(targted))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (targted.WillHit(Player.Instance))
                            {
                                if (targted.TicksLeft <= delay || BlockSpell.FastEvade)
                                    CastW(targted.Caster, BlockSpell.SpellName, targted.Start.To2D());
                            }

                            break;
                        }
                        #endregion

                        #region autoattack
                        var autoattack = BlockSpell.DetectedData as DetectedEmpoweredAttackData;
                        if (autoattack != null && Config.BlockAA)
                        {
                            if (autoattack.Ended || !EmpoweredAttackDetector.DetectedEmpoweredAttacks.Contains(autoattack))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (autoattack.WillHit(Player.Instance))
                            {
                                if (autoattack.TicksLeft <= delay)
                                    CastW(autoattack.Caster, BlockSpell.SpellName, autoattack.Start.To2D());
                            }

                            break;
                        }
                        #endregion

                        #region special
                        var special = BlockSpell.DetectedData as DetectedSpecialSpellData;
                        if (special != null && Config.BlockSpecial)
                        {
                            if (special.Ended || !SpecialSpellDetector.DetectedSpecialSpells.Contains(special))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (special.WillHit(Player.Instance))
                            {
                                if (special.TicksLeft <= delay || BlockSpell.FastEvade)
                                    CastW(special.Caster, BlockSpell.SpellName, special.Position.To2D());
                            }

                            break;
                        }
                        #endregion
                    }
                }
            }

            private static float delay => W.CastDelay + (Game.Ping / 2f); 

            private static void OnSpecialSpellDetected_OnDetect(DetectedSpecialSpellData args)
            {
                if (!args.IsEnemy || !args.WillHit(Player.Instance))
                    return;
                
                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));
                if (spell == null)
                {
                    Logger.Warn($"{spellname} Not valid Spell");
                    return;
                }
                
                if (!spell.Enabled && (!Config.BlockExecute || Player.Instance.PredictHealth(args.TicksLeft) > args.GetSpellDamage(Player.Instance)))
                {
                    Logger.Warn($"{spellname} Not Enabled from Menu");
                    return;
                }

                var newSpell = new DetectedSpell
                {
                    Caster = args.Caster,
                    DangerLevel = spell.DangerLevel,
                    FastEvade = spell.FastEvade,
                    SpellName = spellname,
                    DetectedData = args
                };
                
                if (!DetectedSpells.Contains(newSpell))
                    DetectedSpells.Add(newSpell);

                /*
                if (spell != null && spell.FastEvade)
                    CastW(args.Caster, spellname);
                else
                {
                    if (args.TicksLeft <= delay)
                        CastW(args.Caster, spellname);
                }*/
            }

            private static void OnEmpoweredAttackDetected_OnDetect(DetectedEmpoweredAttackData args)
            {
                if (args.Caster == null || !args.Caster.IsEnemy || !args.WillHit(Player.Instance))
                    return;

                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));

                if (spell == null)
                {
                    Logger.Warn($"{spellname} Not valid Spell");
                    return;
                }

                if (!spell.Enabled && (!Config.BlockExecute || Player.Instance.PredictHealth(args.TicksLeft) > args.GetSpellDamage(Player.Instance)))
                {
                    Logger.Warn($"{spellname} Not Enabled from Menu");
                    return;
                }

                var newSpell = new DetectedSpell
                {
                    Caster = args.Caster,
                    DangerLevel = spell.DangerLevel,
                    SpellName = spellname,
                    DetectedData = args
                };

                if (!DetectedSpells.Contains(newSpell))
                    DetectedSpells.Add(newSpell);

                //CastW(args.Caster, spellname);
            }

            private static void OnDangerBuffDetected_OnDetect(DetectedDangerBuffData args)
            {
                if (args.Caster == null || !args.Caster.IsEnemy || !args.WillHit(Player.Instance))
                    return;

                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));
                
                if (spell == null)
                {
                    Logger.Warn($"{spellname} Not valid Spell");
                    return;
                }

                if (!spell.Enabled && (!Config.BlockExecute || Player.Instance.PredictHealth(args.TicksLeft) > args.GetSpellDamage(Player.Instance)))
                {
                    Logger.Warn($"{spellname} Not Enabled from Menu");
                    return;
                }

                var newSpell = new DetectedSpell
                {
                    Caster = args.Caster,
                    DangerLevel = spell.DangerLevel,
                    SpellName = spellname,
                    DetectedData = args
                };

                if (!DetectedSpells.Contains(newSpell))
                    DetectedSpells.Add(newSpell);

                /*
                if (args.TicksLeft <= delay)
                    CastW(args.Caster, spellname);*/
            }

            private static void OnTargetedSpellDetected_OnDetect(DetectedTargetedSpellData args)
            {
                if (args.Caster == null || !args.Caster.IsEnemy || !args.Target.IsMe || !args.WillHit(Player.Instance))
                    return;

                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));

                //var kill = args.Caster.GetSpellDamage(args.Target, args.Data.slot) >= args.Target.Health;

                if (spell == null)
                {
                    Logger.Warn($"{spellname} Not valid Spell");
                    return;
                }

                if (!spell.Enabled && (!Config.BlockExecute || Player.Instance.PredictHealth(args.TicksLeft) > args.GetSpellDamage(Player.Instance)))
                {
                    Logger.Warn($"{spellname} Not Enabled from Menu");
                    return;
                }

                var newSpell = new DetectedSpell
                {
                    Caster = args.Caster,
                    DangerLevel = spell.DangerLevel,
                    FastEvade = spell.FastEvade,
                    SpellName = spellname,
                    DetectedData = args
                };

                if (!DetectedSpells.Contains(newSpell))
                    DetectedSpells.Add(newSpell);

                /*
                if (spell != null && spell.FastEvade)
                    CastW(args.Caster, spellname);
                else
                {
                    if (args.TicksLeft <= delay)
                        CastW(args.Caster, spellname);
                }*/
            }

            private static void Drawing_OnDraw(EventArgs args)
            {
                /*if (!Config.DrawMenu.CheckBoxValue("draw"))
                    return;

                foreach (var s in SkillshotDetector.SkillshotsDetected.Where(s=> s.Caster.IsEnemy))
                {
                    s.Polygon?.Draw(System.Drawing.Color.AliceBlue, 2);
                }
                foreach (var s in SpecialSpellDetector.DetectedSpecialSpells.Where(s => s.IsEnemy))
                {
                    s.Position.DrawCircle((int)s.Data.Range, Color.AliceBlue);
                }*/
            }

            private static void OnSkillShotDetected_OnDetect(DetectedSkillshotData args)
            {
                if (args.Caster == null || !args.Caster.IsEnemy || !args.WillHit(Player.Instance))
                    return;

                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));

                if (spell == null)
                {
                    Logger.Warn($"{spellname} Not valid Spell");
                    return;
                }

                if (!spell.Enabled && (!Config.BlockExecute || Player.Instance.PredictHealth(args.TravelTime(Player.Instance)) > args.GetSpellDamage(Player.Instance)))
                {
                    Logger.Warn($"{spellname} Not Enabled from Menu");
                    return;
                }

                var newSpell = new DetectedSpell
                    {
                        Caster = args.Caster,
                        DangerLevel = spell.DangerLevel,
                        FastEvade = spell.FastEvade,
                        SpellName = spellname,
                        DetectedData = args
                    };

                if(!DetectedSpells.Contains(newSpell))
                    DetectedSpells.Add(newSpell);

                /*
                if (spell.FastEvade)
                    CastW(args.Caster, spellname);
                else
                {
                    if (args.TravelTime(Player.Instance) <= delay)
                        CastW(args.Caster, spellname);
                }*/
            }

            private static void CastW(Obj_AI_Base caster, string spellname = "", Vector2 castPos = new Vector2())
            {
                if(!Config.evadeEnabled)
                    return;

                if (!W.IsReady())
                    return;

                var wtarget =
                    TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsKillable(-1, true) && W.IsInRange(W.GetPrediction(TargetSelector.SelectedTarget).CastPosition)
                    ? TargetSelector.SelectedTarget 
                    : W.GetTarget().IsKillable(-1, true) && W.IsInRange(W.GetPrediction(W.GetTarget()).CastPosition)
                    ? W.GetTarget()
                    : caster;

                var castpos = wtarget.IsKillable(-1, true) && W.IsInRange(W.GetPrediction(wtarget).CastPosition)
                    ? (W.GetPrediction(wtarget).CastPosition + wtarget.ServerPosition) / 2
                    : castPos != new Vector2() && W.IsInRange(castPos.To3DWorld()) ? castPos.To3DWorld() : Game.CursorPos;
                
                W.Cast(castpos);
                _lastBlock = Core.GameTickCount;
                Logger.Info($"BLOCK {spellname}");
            }

            public static List<EnabledSpell> EnabledSpells = new List<EnabledSpell>();

            public class EnabledSpell
            {
                public EnabledSpell(string spellname)
                {
                    this.SpellName = spellname;
                }
                
                public string SpellName;
                public bool Enabled { get { return Config.spellblock.CheckBoxValue($"enable{SpellName}"); } }
                public bool FastEvade { get { return Config.spellblock.CheckBoxValue($"fast{this.SpellName}"); } }
                public int DangerLevel { get { return Config.spellblock.SliderValue($"danger{this.SpellName}"); } }
            }

            public static List<DetectedSpell> DetectedSpells = new List<DetectedSpell>();

            public class DetectedSpell
            {
                public Obj_AI_Base Caster;
                public object DetectedData;
                public string SpellName;
                public int DangerLevel;
                public bool FastEvade;
            }
        }

        public static class Config
        {
            public static Menu ComboMenu, spellblock, ksMenu, LMenu, JMenu, MiscMenu, HMenu, DMenu;

            public static bool validVitals => ComboMenu.CheckBoxValue("QValidvitals");
            public static bool useQShortVital => ComboMenu.CheckBoxValue("QShortvital");
            public static bool useQLongvital => ComboMenu.CheckBoxValue("QLongvital");
            public static bool orbwalk => ComboMenu.CheckBoxValue("orbVital");
            public static bool useEReset => ComboMenu.CheckBoxValue("EReset");
            public static bool useR => ComboMenu.CheckBoxValue("R");
            public static bool orbUltVital => ComboMenu.CheckBoxValue("orbUltVital");
            public static bool orbAAVital => ComboMenu.CheckBoxValue("aaVitl");
            public static bool focusRTarget => ComboMenu.CheckBoxValue("focusR");

            public static bool AutoHarass => HMenu.KeyBindValue("autoHarass");
            public static bool QHarass => HMenu.CheckBoxValue("qharass") && Player.Instance.HealthPercent > HMenu.SliderValue("qHarassHP") && Player.Instance.ManaPercent > HMenu.SliderValue("qHarassMana");
            public static bool QHarassTurrets => HMenu.CheckBoxValue("qhturret");

            public static bool evadeEnabled => spellblock.CheckBoxValue("enable");
            public static bool BlockExecute => spellblock.CheckBoxValue("executeBlock");
            public static bool BlockAA => spellblock.CheckBoxValue("AABlock");
            public static bool BlockBuff => spellblock.CheckBoxValue("buffBlock");
            public static bool BlockTargeted => spellblock.CheckBoxValue("targetedBlock");
            public static bool BlockSkillshots => spellblock.CheckBoxValue("skillshotBlock");
            public static bool BlockSpecial => spellblock.CheckBoxValue("specialBlock");

            public static bool useEUnk => CanLaneCelarE && LMenu.CheckBoxValue("Eunk");
            public static bool EResetTurrets => CanLaneCelarE && LMenu.CheckBoxValue("ETurrets");
            public static bool EWards => LMenu.CheckBoxValue("EWard");
            public static bool CanLaneCelarE => Player.Instance.ManaPercent > LMenu.SliderValue("ELaneMana");

            public static bool Ejungle => JMenu.CheckBoxValue("Ejung") && EjungleMana;
            public static bool EjungleMana => Player.Instance.ManaPercent > JMenu.SliderValue("Ejungmana");

            public static bool useQks => ksMenu.CheckBoxValue("Qks");
            public static bool useWks => ksMenu.CheckBoxValue("Wks");

            public static bool PlayAudio => MiscMenu.CheckBoxValue("audio");

            public static bool DrawQ => DMenu.CheckBoxValue("DrawQ");
            public static bool DrawVitals => DMenu.CheckBoxValue("DrawVitals");
            public static bool DrawDamage => DMenu.CheckBoxValue("DrawDamage");

            public static void Init()
            {
                #region combo

                ComboMenu = Program.GlobalMenu.AddSubMenu("Fiora: Combo");

                ComboMenu.AddGroupLabel("Vital Settings");
                ComboMenu.CreateCheckBox("QValidvitals", "Q Valid Vitals Only");
                ComboMenu.CreateCheckBox("QShortvital", "Q Vital in Range");
                ComboMenu.CreateCheckBox("QLongvital", "Q Vitals Long Range");
                ComboMenu.CreateCheckBox("orbVital", "Orbwalk To Vitals");
                ComboMenu.CreateCheckBox("orbUltVital", "Orbwalk to R Vitals Only", false);
                ComboMenu.CreateCheckBox("aaVitl", "Orbwalk to Vitals in AA Range Only");
                ComboMenu.CreateCheckBox("focusR", "Force Focus Target with R Mark");
                ComboMenu.AddGroupLabel("Extra Settings");
                ComboMenu.CreateCheckBox("EReset", "E Reset Auto Attack");
                ComboMenu.CreateCheckBox("R", "Auto use R");

                #endregion combo

                #region Harass

                HMenu = Program.GlobalMenu.AddSubMenu("Fiora: Harass");
                HMenu.CreateKeyBind("autoHarass", "Auto Harass", false, KeyBind.BindTypes.PressToggle);
                HMenu.CreateCheckBox("qharass", "Q Vitals");
                HMenu.CreateCheckBox("qhturret", "Dont Q Dive Enemy Turret");
                HMenu.CreateSlider("qHarassHP", "Q Harass Health limit", 40);
                HMenu.CreateSlider("qHarassMana", "Q Harass Mana limit", 60);

                #endregion

                #region Evade

                spellblock = Program.GlobalMenu.AddSubMenu("Fiora: SpellBlock");
                spellblock.CreateCheckBox("enable", "Enable SpellBlock");
                spellblock.CreateCheckBox("executeBlock", "Block Any Spell if it will Kill Player");

                #region AutoAttacks
                var validAttacks = EmpowerdAttackDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.Hero)));
                if (validAttacks.Any())
                {
                    spellblock.AddGroupLabel("Empowered Attacks");
                    spellblock.CreateCheckBox("AABlock", "Block Empowered Attacks");
                    foreach (var s in validAttacks.OrderBy(s => s.Hero))
                    {
                        var spellname = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(spellname)))
                        {
                            spellblock.AddLabel(spellname);
                            spellblock.CreateCheckBox("enable" + spellname, "Enable", s.DangerLevel > 1 || s.CrowdControl);
                            spellblock.CreateSlider("danger" + spellname, "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(spellname));
                            spellblock.AddSeparator(0);
                        }
                    }
                }
                #endregion AutoAttacks

                #region buffs
                var validBuffs = DangerBuffDataDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.Hero)));
                if (validBuffs.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("Danger Buffs");
                    spellblock.CreateCheckBox("buffBlock", "Block Danger Buffs");

                    foreach (var s in validBuffs.OrderBy(s => s.Hero))
                    {
                        var spellname = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(spellname)))
                        {
                            spellblock.AddLabel(spellname);
                            spellblock.CreateCheckBox("enable" + spellname, "Enable", s.DangerLevel > 1);
                            if (s.HasStackCount)
                            {
                                var stackCount = spellblock.CreateSlider("stackCount", "Block at Stack Count", s.StackCount, 1, s.MaxStackCount);
                                s.StackCountFromMenu = () => stackCount.CurrentValue;
                            }
                            spellblock.CreateSlider("danger" + spellname, "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(spellname));
                            spellblock.AddSeparator(0);
                        }
                    }
                }
                #endregion buffs

                #region Targeted
                var validTargeted = TargetedSpellDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.hero)));
                if (validTargeted.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("Targeted Spells");
                    spellblock.CreateCheckBox("targetedBlock", "Block Targeted Spells");
                    foreach (var s in validTargeted.OrderBy(s => s.hero))
                    {
                        var spellname = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(spellname)))
                        {
                            spellblock.AddLabel(spellname);
                            spellblock.CreateCheckBox("enable" + spellname, "Enable", s.DangerLevel > 1);
                            spellblock.CreateCheckBox("fast" + spellname, "Fast Block (Instant)", s.FastEvade);
                            spellblock.CreateSlider("danger" + spellname, "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(spellname));
                            spellblock.AddSeparator(0);
                        }
                    }
                }
                #endregion Targeted

                #region Speical spells
                var specialSpells = SpecialSpellsDatabase.Current.FindAll(s => EntityManager.Heroes.Enemies.Any(h => s.Hero.Equals(h.Hero)));
                if (specialSpells.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("Special Spells");
                    spellblock.CreateCheckBox("specialBlock", "Block Special Spells");
                    foreach (var s in specialSpells)
                    {
                        var display = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(display)))
                        {
                            spellblock.AddLabel(display);
                            spellblock.CreateCheckBox($"enable{display}", "Enable", s.DangerLevel > 1);
                            spellblock.CreateCheckBox($"fast{display}", "Fast Block (Instant)", s.DangerLevel > 2);
                            spellblock.CreateSlider($"danger{display}", "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(display));
                        }
                    }
                }
                #endregion Speical spells

                #region SkillShots
                var validskillshots =
                    SkillshotDatabase.Current.Where(s => (s.GameType.Equals(GameType.Normal) || s.GameType.Equals(Game.Type))
                    && EntityManager.Heroes.Enemies.Any(h => s.IsCasterName(Champion.Unknown) || s.IsCasterName(h.Hero))).OrderBy(s => s.CasterNames[0]);
                if (validskillshots.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("SkillShots");
                    spellblock.CreateCheckBox("skillshotBlock", "Block SkillShots");

                    foreach (var s in validskillshots)
                    {
                        var display = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(display)))
                        {
                            spellblock.AddLabel(display);
                            spellblock.CreateCheckBox($"enable{display}", "Enable", s.DangerLevel > 1);
                            spellblock.CreateCheckBox($"fast{display}", "Fast Block (Instant)", s.FastEvade);
                            spellblock.CreateSlider($"danger{display}", "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(display));
                        }
                    }
                }
                #endregion SkillShots

                #endregion evade

                #region laneclear

                LMenu = Program.GlobalMenu.AddSubMenu("Fiora: LaneClear");
                LMenu.CreateCheckBox("Eunk", "Use E On Unkillable Minions");
                LMenu.CreateCheckBox("ETurrets", "Use E Reset On Structures");
                LMenu.CreateCheckBox("EWard", "Use E Reset On Wards");
                LMenu.CreateSlider("ELaneMana", "E Mana Limit", 60);

                #endregion laneclear

                #region jungleclear

                JMenu = Program.GlobalMenu.AddSubMenu("Fiora: JungleClear");
                JMenu.CreateCheckBox("Ejung", "Use E");
                JMenu.CreateSlider("Ejungmana", "E Mana Limit", 60);

                #endregion

                #region Killsteal

                ksMenu = Program.GlobalMenu.AddSubMenu("Fiora: Killsteal");
                ksMenu.CreateCheckBox("Qks", "Use Q");
                ksMenu.CreateCheckBox("Wks", "Use W");

                #endregion Killsteal

                #region Misc

                MiscMenu = Program.GlobalMenu.AddSubMenu("Fiora: Misc");
                MiscMenu.CreateCheckBox("audio", "Play Audio");
                MiscMenu.AddGroupLabel("R Block list");
                foreach (var e in EntityManager.Heroes.Enemies)
                {
                    MiscMenu.CreateCheckBox(e.Name(), $"Dont R {e.Name()}", false);
                }

                #endregion Misc

                #region Drawing

                DMenu = Program.GlobalMenu.AddSubMenu("Fiora: Drawing");
                DMenu.CreateCheckBox("DrawQ", "Draw Q Range");
                DMenu.CreateCheckBox("DrawVitals", "Draw Vitals");
                DMenu.CreateCheckBox("DrawDamage", "Draw Damage On Enemies");

                #endregion Drawing
            }
        }
    }
}
