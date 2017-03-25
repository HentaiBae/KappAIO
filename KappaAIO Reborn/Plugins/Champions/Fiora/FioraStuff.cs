using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
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
                        var passive = new FioraVital(emitter) { startTick = Core.GameTickCount };
                        if (!StoredPassives.Contains(passive))
                            StoredPassives.Add(passive);
                        StoredPassives.RemoveAll(v => v.Vital != null && (v.Vital.IsDead || !v.Vital.IsValid) || v.Caster != null && (!v.Caster.IsValid || v.Caster.IsDead) || Core.GameTickCount - v.startTick > 15000);
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
                        if (StoredPassives.Any(p => p.Vital.Name.Equals(emitter.Name)))
                            StoredPassives.RemoveAll(v => v.Vital != null && (v.Vital.IdEquals(emitter) || v.Vital.IsDead || !v.Vital.IsValid) || v.Caster != null && (!v.Caster.IsValid || v.Caster.IsDead) || Core.GameTickCount - v.startTick > 15000);
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
                public AIHeroClient Caster { get { return EntityManager.Heroes.AllHeroes.OrderBy(e => e.Distance(this.Vital)).FirstOrDefault(h => HasFioraPassiveBuff(h) && h.Distance(this.Vital) <= 300); } }
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
                        var range = 175;
                        var travelTime = Player.Instance.Distance(CorrectPos) / Player.Instance.MoveSpeed * 1000f;
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
                        var range = 175;
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

                var center = v.QpredVitalsector.CenterOfPolygon();
                var qpos = v.QPredVitalPos;

                if (shortVital)
                {
                    var close = Q1.IsInRange(qpos);
                    if (close)
                    {
                        return qpos;
                    }
                }

                if (longVital)
                {
                    var vitdis = v.QPredVitalPos.Distance(Player.Instance, true);
                    var tardis = target.Distance(Player.Instance, true);
                    var farvit = tardis > vitdis || Q1.IsInRange(center.To3D());
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
                if (!Q1.IsLearned)
                    return 0;
                var index = Q1.Level - 1;
                var dmg = new[] { 65f, 75f, 85f, 95f, 105f }[index];
                var bonus = new[] { 0.95f, 1f, 1.05f, 1.1f, 1.15f }[index] * Player.Instance.FlatPhysicalDamageMod;
                return target == null ? dmg + bonus : Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, dmg + bonus);
            }
            public static float WDamage(Obj_AI_Base target)
            {
                if (!W.IsLearned)
                    return 0;
                var index = W.Level - 1;
                var dmg = new[] { 90f, 130f, 170f, 210f, 250f }[index];
                var bonus = Player.Instance.TotalMagicalDamage;
                return target == null ? dmg + bonus : Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg + bonus);
            }
            public static float EDamage(Obj_AI_Base target)
            {
                if (!E.IsLearned)
                    return 0;
                var index = E.Level - 1;
                var mod = new[] { 1.4f, 1.55f, 1.7f, 1.85f, 2f }[index];
                var dmg = Player.Instance.TotalAttackDamage;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, dmg * mod);
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
                Drawing.OnDraw += Drawing_OnDraw;
            }

            private static float _lastBlock;
            private static void Game_OnTick(EventArgs args)
            {
                if(delay > Core.GameTickCount - _lastBlock)
                    return;

                var BlockSpells = DetectedSpells.OrderByDescending(s => s.DangerLevel);
                if (BlockSpells != null && BlockSpells.Any())
                {
                    foreach (var BlockSpell in BlockSpells)
                    {
                        #region skillshot
                        var skillshot = BlockSpell.DetectedData as DetectedSkillshotData;
                        if (skillshot != null)
                        {
                            if (skillshot.Ended || !SkillshotDetector.SkillshotsDetected.Contains(skillshot))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (skillshot.WillHit(Player.Instance))
                            {
                                if(skillshot.TravelTime(Player.Instance) <= delay || BlockSpell.FastEvade)
                                    CastW(skillshot.Caster, BlockSpell.SpellName);
                            }

                            break;
                        }
                        #endregion

                        #region buff
                        var buff = BlockSpell.DetectedData as DetectedDangerBuffData;
                        if (buff != null)
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
                        if (targted != null)
                        {
                            if (targted.Ended || !TargetedSpellDetector.DetectedTargetedSpells.Contains(targted))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (targted.WillHit(Player.Instance))
                            {
                                if (targted.TicksLeft <= delay || BlockSpell.FastEvade)
                                    CastW(targted.Caster, BlockSpell.SpellName);
                            }

                            break;
                        }
                        #endregion

                        #region autoattack
                        var autoattack = BlockSpell.DetectedData as DetectedEmpoweredAttackData;
                        if (autoattack != null)
                        {
                            if (autoattack.Ended || !EmpoweredAttackDetector.DetectedEmpoweredAttacks.Contains(autoattack))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (autoattack.WillHit(Player.Instance))
                            {
                                if (autoattack.TicksLeft <= delay)
                                    CastW(autoattack.Caster, BlockSpell.SpellName);
                            }

                            break;
                        }
                        #endregion

                        #region special
                        var special = BlockSpell.DetectedData as DetectedSpecialSpellData;
                        if (special != null)
                        {
                            if (special.Ended || !SpecialSpellDetector.DetectedSpecialSpells.Contains(special))
                            {
                                DetectedSpells.Remove(BlockSpell);
                                continue;
                            }

                            if (special.WillHit(Player.Instance))
                            {
                                if (special.TicksLeft <= delay || BlockSpell.FastEvade)
                                    CastW(special.Caster, BlockSpell.SpellName);
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
                if (spell == null || !spell.Enabled)
                {
                    Console.WriteLine($"{spellname} Not Blocked");
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

                var kill = args.Caster.GetAutoAttackDamage(args.Target, true) >= args.Target.Health;

                if ((spell == null || !spell.Enabled) && !kill)
                {
                    Console.WriteLine($"{spellname} Not Blocked");
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

                var kill = args.Caster.GetSpellDamage(args.Target, args.Data.Slot) >= args.Target.Health;

                if ((spell == null || !spell.Enabled) && !kill)
                {
                    Console.WriteLine($"{spellname} Not Blocked");
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

                var kill = args.Caster.GetSpellDamage(args.Target, args.Data.slot) >= args.Target.Health;

                if ((spell == null || !spell.Enabled) && !kill)
                {
                    Console.WriteLine($"{spellname} Not Blocked");
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
                //if (!Config.DrawMenu.CheckBoxValue("draw"))
                    return;

                foreach (var s in SkillshotDetector.SkillshotsDetected.Where(s=> s.Caster.IsEnemy))
                {
                    s.Polygon?.Draw(System.Drawing.Color.AliceBlue, 2);
                }
                foreach (var s in SpecialSpellDetector.DetectedSpecialSpells.Where(s => s.IsEnemy))
                {
                    s.Position.DrawCircle((int)s.Data.Range, Color.AliceBlue);
                }
            }

            private static void OnSkillShotDetected_OnDetect(DetectedSkillshotData args)
            {
                if (args.Caster == null || !args.Caster.IsEnemy || !args.WillHit(Player.Instance))
                    return;

                var spellname = args.Data.MenuItemName;
                var spell = EnabledSpells.FirstOrDefault(s => s.SpellName.Equals(spellname));

                bool kill = false;
                var hero = args.Caster as AIHeroClient;
                if (hero != null)
                {
                    kill = hero.GetSpellDamage(Player.Instance, args.Data.Slots[0]) >= Player.Instance.Health;
                }

                if (spell == null)
                    return;

                if (!spell.Enabled && !kill)
                {
                    Console.WriteLine($"{spellname} Not Blocked");
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

            private static void CastW(Obj_AI_Base caster, string spellname = "")
            {
                if(!Config.evadeEnabled)
                    return;

                if (!W.IsReady())
                    return;

                var wtarget =
                    TargetSelector.SelectedTarget != null && W.IsInRange(TargetSelector.SelectedTarget) ? TargetSelector.SelectedTarget
                    : W.GetTarget() != null && W.IsInRange(W.GetTarget()) ? W.GetTarget() : caster;

                var castpos = wtarget != null && W.IsInRange(wtarget) ? wtarget.ServerPosition : Game.CursorPos;
                
                W.Cast(castpos);
                _lastBlock = Core.GameTickCount;
                Console.WriteLine($"BLOCK {spellname}");
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
            public static Menu ComboMenu, spellblock, ksMenu, LMenu, MiscMenu;
            private static CheckBox QShortvital, QLongvital, QValidvitals, orbVital, EReset, Hydra, R, spellblockEnable, Qks, Wks, Eunk, orbRvit, aaVitl, focusR;

            public static bool validVitals => QValidvitals.CurrentValue;
            public static bool useQShortVital => QShortvital.CurrentValue;
            public static bool useQLongvital => QLongvital.CurrentValue;
            public static bool orbwalk => orbVital.CurrentValue;
            public static bool useEReset => EReset.CurrentValue;
            public static bool useHydra => Hydra.CurrentValue;
            public static bool useR => R.CurrentValue;
            public static bool evadeEnabled => spellblockEnable.CurrentValue;
            public static bool useQks => Qks.CurrentValue;
            public static bool useWks => Wks.CurrentValue;
            public static bool useEUnk => Eunk.CurrentValue;
            public static bool orbUltVital => orbRvit.CurrentValue;
            public static bool orbAAVital => aaVitl.CurrentValue;
            public static bool focusRTarget => focusR.CurrentValue;

            public static void Init()
            {
                #region combo

                ComboMenu = Program.GlobalMenu.AddSubMenu("Fiora: Combo");

                ComboMenu.AddGroupLabel("Vital Settings");
                QValidvitals = ComboMenu.CreateCheckBox("QValidvitals", "Q Valid Vitals Only");
                QShortvital = ComboMenu.CreateCheckBox("QShortvital", "Q Vital in Range");
                QLongvital = ComboMenu.CreateCheckBox("QLongvital", "Q Vitals Long Range");
                orbVital = ComboMenu.CreateCheckBox("orbVital", "Orbwalk To Vitals");
                orbRvit = ComboMenu.CreateCheckBox("orbUltVital", "Orbwalk to R Vitals Only", false);
                aaVitl = ComboMenu.CreateCheckBox("aaVitl", "Orbwalk to Vitals in AA Range Only");
                focusR = ComboMenu.CreateCheckBox("focusR", "Force Focus Target with R Mark");
                ComboMenu.AddGroupLabel("Extra Settings");
                EReset = ComboMenu.CreateCheckBox("EReset", "E Reset Auto Attack");
                Hydra = ComboMenu.CreateCheckBox("Hydra", "Use Hydra");
                R = ComboMenu.CreateCheckBox("R", "Auto use R");

                #endregion combo

                #region Evade

                spellblock = Program.GlobalMenu.AddSubMenu("Fiora: SpellBlock");
                spellblockEnable = spellblock.CreateCheckBox("enable", "Enable SpellBlock");

                var validAttacks = EmpowerdAttackDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.Hero)));
                if (validAttacks.Any())
                {
                    spellblock.AddGroupLabel("Empowered Attacks");
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

                var validBuffs = DangerBuffDataDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.Hero)));
                if (validBuffs.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("Danger Buffs");

                    foreach (var s in validBuffs.OrderBy(s => s.Hero))
                    {
                        var spellname = s.MenuItemName;
                        if (!SpellBlocker.EnabledSpells.Any(x => x.SpellName.Equals(spellname)))
                        {
                            spellblock.AddLabel(spellname);
                            spellblock.CreateCheckBox("enable" + spellname, "Enable", s.DangerLevel > 1);
                            spellblock.CreateSlider("danger" + spellname, "Danger Level", s.DangerLevel, 1, 5);
                            SpellBlocker.EnabledSpells.Add(new SpellBlocker.EnabledSpell(spellname));
                            spellblock.AddSeparator(0);
                        }
                    }
                }


                var validTargeted = TargetedSpellDatabase.Current.FindAll(x => EntityManager.Heroes.Enemies.Any(h => h.Hero.Equals(x.hero)));
                if (validTargeted.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("Targeted Spells");
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

                var specialSpells = SpecialSpellsDatabase.Current.FindAll(s => EntityManager.Heroes.Enemies.Any(h => s.Hero.Equals(h.Hero)));
                if (specialSpells.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("SpecialSpells");
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

                var validskillshots =
                    SkillshotDatabase.Current.Where(s => (s.GameType.Equals(GameType.Normal) || s.GameType.Equals(Game.Type))
                    && EntityManager.Heroes.Enemies.Any(h => s.IsCasterName(Champion.Unknown) || s.IsCasterName(h.Hero))).OrderBy(s => s.CasterNames[0]);
                if (validskillshots.Any())
                {
                    spellblock.AddSeparator(5);
                    spellblock.AddGroupLabel("SkillShots");

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

                #endregion evade

                #region laneclear

                LMenu = Program.GlobalMenu.AddSubMenu("Fiora: LaneClear");
                Eunk = LMenu.CreateCheckBox("Eunk", "Use E On Unkillable Minions");

                #endregion laneclear

                #region Killsteal

                ksMenu = Program.GlobalMenu.AddSubMenu("Fiora: Killsteal");
                Qks = ksMenu.CreateCheckBox("Qks", "Use Q");
                Wks = ksMenu.CreateCheckBox("Wks", "Use W");

                #endregion Killsteal

                #region Misc

                MiscMenu = Program.GlobalMenu.AddSubMenu("Fiora: Misc");
                MiscMenu.AddGroupLabel("R Block list");
                foreach (var e in EntityManager.Heroes.Enemies)
                {
                    MiscMenu.CreateCheckBox(e.Name(), $"Dont R {e.Name()}", false);
                }

                #endregion Misc
            }
        }
    }
}
