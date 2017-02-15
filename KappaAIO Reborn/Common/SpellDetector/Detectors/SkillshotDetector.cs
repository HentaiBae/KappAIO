using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Events;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;
using KappAIO_Reborn.Common.Utility;
using SharpDX;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class SkillshotDetector
    {
        private static bool Loaded;
        static SkillshotDetector()
        {
            if (!Loaded)
            {
                Collision.Init();
                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Spellbook.OnStopCast += Spellbook_OnStopCast;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
                Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
                Loaded = true;
            }
        }

        public class LuxRPartical
        {
            public SkillshotData Data;
            public float StartTick = Core.GameTickCount;
            public AIHeroClient caster;
            public Vector2? Start;
            public Vector2? Mid;
            public Vector2? End;
            public bool FullyDetected => this.caster != null && Start != null && this.Mid != null && this.End != null;
        }
        public class IllaoiTentacle
        {
            public AIHeroClient caster;
            public Obj_AI_Minion Tentacle;
            public Vector2 endpos;
            public float StartTick = Core.GameTickCount;
            public bool Attacked;
        }
        public class SkillshotMinion
        {
            public AIHeroClient caster;
            public List<Obj_AI_Minion> Minion;
            public SkillshotData Data;
            public int ID;
            public float StartTick = Core.GameTickCount;
            public bool Remove;
        }

        public static List<SkillshotMinion> SkillshotMinions = new List<SkillshotMinion>();
        public static List<IllaoiTentacle> IllaoiTentacles = new List<IllaoiTentacle>();
        public static List<LuxRPartical> DetectedLuxRParticals = new List<LuxRPartical>();

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender == null)
                return;

            if (!sender.BaseSkinName.Equals("IllaoiMinion") || !args.Animation.Contains("Attack"))
                return;

            var tentacles = IllaoiTentacles.FindAll(x => x.Tentacle.IdEquals(sender));
            if (tentacles.Any())
            {
                foreach (var t in IllaoiTentacles.Where(x => x.Tentacle.IdEquals(sender)))
                {
                    var detected = new DetectedSkillshotData
                    {
                        Caster = t.caster,
                        Data = SkillshotDatabase.List.FirstOrDefault(s => s.hero == Champion.Illaoi && s.slot.Equals(SpellSlot.W)),
                        Start = sender.ServerPosition.To2D(),
                        End = t.endpos,
                        StartTick = Core.GameTickCount
                    };

                    Add(detected);
                    t.Attacked = true;
                }
            }
            else
            {
                var buff = sender.Buffs.FirstOrDefault(b => b.DisplayName.Equals("illaoitentacleactive"));
                var caster = buff?.Caster as AIHeroClient;
                if (caster != null)
                {
                    var illaoiESpirit = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.Buffs.Any(b => b.Caster.IdEquals(caster) && b.DisplayName.Equals("IllaoiESpirit")));
                    var target = EntityManager.Heroes.AllHeroes.FindAll(h => h.Buffs.Any(b => b.Caster.IdEquals(caster))).OrderBy(h => h.Distance(sender)).FirstOrDefault();
                    var correct = illaoiESpirit ?? target;
                    if (correct != null)
                    {
                        var detected = new DetectedSkillshotData
                        {
                            Caster = caster,
                            Data = SkillshotDatabase.List.FirstOrDefault(s => s.hero == Champion.Illaoi && s.slot.Equals(SpellSlot.W)),
                            Start = sender.ServerPosition.To2D(),
                            End = correct.ServerPosition.To2D(),
                            StartTick = Core.GameTickCount
                        };

                        Add(detected);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as Obj_AI_Base;
            if (caster == null || target == null)
                return;

            if (!caster.Hero.Equals(Champion.Illaoi))
                return;

            var IllaoiWAttack = args.SData.Name.Equals("IllaoiWAttack");
            if (!IllaoiWAttack)
                return;

            var illaoiMinions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValid && !o.IsDead && o.BaseSkinName.Equals("IllaoiMinion") && o.Buffs.Any(b => b.Caster.IdEquals(caster))).ToList();
            if (!illaoiMinions.Any())
                return;

            var illaoiw = SkillshotDatabase.List.FirstOrDefault(s => s.hero.Equals(Champion.Illaoi) && s.slot.Equals(SpellSlot.W));
            if (illaoiw == null)
                return;

            foreach (var tentacle in illaoiMinions.Where(m => m.IsInRange(target.PrediectPosition(), illaoiw.Range * 1.1f)))
            {
                IllaoiTentacles.Add(new IllaoiTentacle { caster = caster, endpos = target.PrediectPosition().To2D(), Tentacle = tentacle });
            }
        }

        private static void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.ChampionName.Equals("Sion"))
                return;

            SkillshotsDetected.RemoveAll(s => s.Caster.IdEquals(caster)); // Clear all sion Skills when he stops casting
        }

        private static void Game_OnTick(EventArgs args)
        {
            foreach (var LuxR in DetectedLuxRParticals)
            {
                var data = SkillshotDatabase.List.FirstOrDefault(s => s.hero.Equals(Champion.Lux) && s.slot.Equals(SpellSlot.R));
                if (data == null)
                {
                    continue;
                }

                if (SkillshotsDetected.Any(s => s.Caster.IdEquals(LuxR.caster) && s.Data.Equals(data)))
                {
                    continue;
                }

                Vector2? start = null;
                Vector2? end = null;

                if (LuxR.Start.HasValue)
                {
                    start = LuxR.Start.Value;
                }
                else
                {
                    if (LuxR.Mid.HasValue && LuxR.End.HasValue)
                    {
                        start = LuxR.End.Value.Extend(LuxR.Mid.Value, data.Range);
                    }
                }

                if (LuxR.End.HasValue)
                {
                    end = LuxR.End.Value;
                }
                else
                {
                    if (LuxR.caster != null && LuxR.caster.IsHPBarRendered)
                    {
                        if (LuxR.End.HasValue)
                        {
                            end = LuxR.End.Value;
                        }
                        if (LuxR.Mid.HasValue)
                        {
                            end = LuxR.caster.ServerPosition.Extend(LuxR.Mid.Value, data.Range);
                        }
                        else
                        {
                            if (LuxR.Start.HasValue)
                            {
                                end = LuxR.caster.ServerPosition.Extend(LuxR.Start.Value, data.Range);
                            }
                        }
                    }
                    else
                    {
                        if (LuxR.Start.HasValue && LuxR.Mid.HasValue)
                        {
                            end = LuxR.Start.Value.Extend(LuxR.Mid.Value, data.Range);
                        }
                    }
                }

                if (start.HasValue && end.HasValue)
                {
                    if (!SkillshotsDetected.Any(s => s.Caster.IdEquals(LuxR.caster) && s.Data.Equals(data)))
                    {
                        var detected = new DetectedSkillshotData
                        {
                            Caster = LuxR.caster,
                            StartTick = Core.GameTickCount,
                            Start = start.Value,
                            End = end.Value,
                            Data = data
                        };

                        Add(detected);
                    }
                }
            }

            foreach (var minion in SkillshotMinions.OrderBy(s => s.caster.NetworkId).Where(s => s.Minion != null && s.Minion.Any()))
            {
                if (minion.Minion.Count >= 5)
                {
                    var start = minion.Minion.FirstOrDefault();
                    var end = minion.Minion.LastOrDefault();
                    if (start != null && end != null && !start.IdEquals(end))
                    {
                        var newDetect = new DetectedSkillshotData
                        {
                            Caster = minion.caster,
                            Data = minion.Data,
                            Start = start.ServerPosition.To2D(),
                            End = end.ServerPosition.To2D(),
                            StartTick = Core.GameTickCount,
                        };

                        Add(newDetect);
                        minion.Remove = true;
                    }
                }
            }
            
            SkillshotMinions.RemoveAll(s => Core.GameTickCount - s.StartTick > s.Data.CastDelay || s.Remove);
            IllaoiTentacles.RemoveAll(s => Core.GameTickCount - s.StartTick > 1250 || s.Attacked);
            DetectedLuxRParticals.RemoveAll(s => Core.GameTickCount - s.StartTick > s.Data.CastDelay);
            SkillshotsDetected.RemoveAll(s => s.Ended);
        }

        public static List<DetectedSkillshotData> SkillshotsDetected = new List<DetectedSkillshotData>();

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var partical = sender as Obj_GeneralParticleEmitter;
            if (partical != null)
            {
                SkillshotsDetected.RemoveAll(s => s.Particle != null && s.Particle.IdEquals(partical));
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || !missile.IsValid)
                return;

            if (SkillshotsDetected.Any(s => s.Caster != null && s.Missile != null && s.Missile.IdEquals(missile) && caster.IdEquals(s.Caster)))
            {
                SkillshotsDetected.RemoveAll(s => s.Caster != null && s.Missile != null && !s.Data.DontRemoveWithMissile && s.Missile.IdEquals(missile) && caster.IdEquals(s.Caster) && s.TicksPassed > Game.Ping);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var partical = sender as Obj_GeneralParticleEmitter;
            if (partical != null)
            {
                var pdata = SkillshotDatabase.List.FirstOrDefault(s => partical.Name.EndsWith(".troy") && ((!string.IsNullOrEmpty(s.ParticalName) && partical.Name.StartsWith(s.ParticalName))
                || (!string.IsNullOrEmpty(s.MissileName) && partical.Name.StartsWith(s.MissileName))
                || (s.ExtraMissileName != null && s.ExtraMissileName.Any(x => partical.Name.StartsWith(x)))));

                if (pdata != null)
                {
                    var testCaster = EntityManager.Heroes.AllHeroes.OrderByDescending(x => x.Distance(partical)).FirstOrDefault(h => h.Hero.Equals(pdata.hero)
                    && ((h.IsAlly && (h.Spellbook.IsChanneling || h.Spellbook.IsCastingSpell || h.Spellbook.IsCharging)) || h.IsEnemy));

                    var pcaster = EntityManager.Heroes.AllHeroes.OrderByDescending(x => x.Distance(partical)).FirstOrDefault(h => h.Hero.Equals(pdata.hero));
                    if (testCaster != null)
                    {
                        pcaster = testCaster;
                    }

                    var correctObject = ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.Distance(partical)).FirstOrDefault(o => !string.IsNullOrEmpty(pdata.ParticalObjectName) && o.Name.Equals(pdata.ParticalObjectName));
                    if (correctObject != null)
                    {
                        var pdetected = new DetectedSkillshotData
                        {
                            Caster = pcaster,
                            Start = correctObject.Position.To2D(),
                            End = correctObject.Position.To2D(),
                            Data = pdata,
                            StartTick = Core.GameTickCount,
                            Particle = partical
                        };

                        Add(pdetected);
                    }
                    else
                    {
                        var start = partical.Position.To2D();
                        var end = partical.Position.To2D();
                        if (pcaster != null)
                        {
                            //start = pcaster.ServerPosition.To2D();
                            switch (pcaster.ChampionName)
                            {
                                case "Yasuo":
                                    {
                                        var alreadydetected = SkillshotsDetected.FirstOrDefault(s => s.Caster.IdEquals(pcaster));
                                        if (alreadydetected != null)
                                        {
                                            var qcircle = SkillshotDatabase.List.FirstOrDefault(s => s.hero.Equals(Champion.Yasuo) && !string.IsNullOrEmpty(s.SpellName) && s.SpellName.Equals(pcaster.HasBuff("YasuoQ3W") ? "E Q3" : "E Q"));
                                            if (qcircle != null)
                                            {
                                                alreadydetected.Data = qcircle;
                                                alreadydetected.End = alreadydetected.Start.Extend(alreadydetected.Caster.ServerPosition.To2D(), qcircle.Range);
                                            }
                                        }
                                        return;
                                    }
                                case "Zac":
                                    if (start.Distance(end) > pdata.Range)
                                    {
                                        start = end.Extend(start, pdata.Range);
                                    }
                                    break;
                            }
                        }

                        var pdetected = new DetectedSkillshotData
                        {
                            Caster = pcaster,
                            Start = start,
                            End = end,
                            Data = pdata,
                            StartTick = Core.GameTickCount,
                            Particle = partical
                        };

                        Add(pdetected);
                    }
                }
                else
                {
                    var StartluxR =
                        SkillshotDatabase.List.FirstOrDefault(p => !string.IsNullOrEmpty(p.StartParticalName) && partical.Name.StartsWith(p.StartParticalName) && partical.Name.EndsWith(".troy"));
                    var MidluxR =
                        SkillshotDatabase.List.FirstOrDefault(p => !string.IsNullOrEmpty(p.StartParticalName) && partical.Name.StartsWith(p.MidParticalName) && partical.Name.EndsWith(".troy"));
                    var EndluxR =
                        SkillshotDatabase.List.FirstOrDefault(p => !string.IsNullOrEmpty(p.StartParticalName) && partical.Name.StartsWith(p.MidParticalName) && partical.Name.EndsWith(".troy"));

                    AIHeroClient pcaster;
                    if (StartluxR != null || MidluxR != null || EndluxR != null)
                    {
                        pdata = SkillshotDatabase.List.FirstOrDefault(s => s.hero.Equals(Champion.Lux) && s.slot.Equals(SpellSlot.R));
                        var luxInStart = EntityManager.Heroes.AllHeroes.FirstOrDefault(h => h.Hero.Equals(Champion.Lux) && h.IsValidTarget() && h.IsInRange(partical, 275));
                        pcaster = luxInStart ?? EntityManager.Heroes.AllHeroes.OrderBy(h => h.Distance(partical)).FirstOrDefault(h => h.ChampionName.Equals("Lux"));

                        if (pcaster != null)
                        {
                            var alreadyDetected = DetectedLuxRParticals.FirstOrDefault(p => p.caster != null && !p.FullyDetected && p.caster.IdEquals(pcaster));
                            if (alreadyDetected != null)
                            {
                                if (alreadyDetected.Start == null && StartluxR != null)
                                {
                                    alreadyDetected.Start = partical.Position.To2D();
                                }
                                if (alreadyDetected.Mid == null && MidluxR != null)
                                {
                                    alreadyDetected.Mid = partical.Position.To2D();
                                }
                                if (alreadyDetected.End == null && EndluxR != null)
                                {
                                    alreadyDetected.End = partical.Position.To2D();
                                }
                            }
                            else
                            {
                                var addnew = new LuxRPartical
                                {
                                    caster = pcaster,
                                    Start = StartluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                    Mid = MidluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                    End = EndluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                    Data = pdata
                                };

                                DetectedLuxRParticals.Add(addnew);
                            }
                        }
                    }
                }
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                var mdata =
                    SkillshotDatabase.List.FirstOrDefault(
                        s => !string.IsNullOrEmpty(s.MinionName) && minion.Name.Equals(s.MinionName) || !string.IsNullOrEmpty(s.MinionBaseSkinName) && minion.BaseSkinName.Equals(s.MinionBaseSkinName));
                if (mdata != null)
                {
                    var mcaster = EntityManager.Heroes.AllHeroes.FirstOrDefault(h => h.Team.Equals(minion.Team) && h.Hero.Equals(mdata.hero) && !h.IsDead);
                    if (mcaster != null)
                    {
                        var alreadyDetected = SkillshotMinions.FirstOrDefault(s => s.caster != null && s.caster.IdEquals(mcaster) && s.Data.Equals(mdata));
                        if (alreadyDetected?.Minion != null && alreadyDetected.Minion.Count < 5 && Core.GameTickCount - alreadyDetected.StartTick < mdata.CastDelay)
                        {
                            alreadyDetected.Minion.Add(minion);
                            alreadyDetected.ID++;
                        }
                        else if (alreadyDetected == null)
                        {
                            var newDetect = new SkillshotMinion
                            {
                                caster = mcaster,
                                Minion = new List<Obj_AI_Minion> { minion },
                                StartTick = Core.GameTickCount,
                                Data = mdata,
                                ID = 1
                            };

                            SkillshotMinions.Add(newDetect);
                        }
                    }
                }
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster;
            if (missile == null || caster == null || (missile.IsAutoAttack() && !caster.BaseSkinName.Equals("Twitch")) || !missile.IsValid)
                return;

            var missilename = missile.SData.Name;
            //Console.WriteLine($"{missilename} - mis - {missile.Slot}");

            var data = GetData(caster, null, missile);

            if (data == null)
            {
                //Console.WriteLine($"OnCreateSkillshots: {caster.ChampionName} - [{missile.Slot} | {missilename}] - Not Detected");
                return;
            }

            var Misstart = missile.StartPosition.To2D();
            var Misend = missile.EndPosition.To2D();

            if (data.hero.Equals(Champion.Shen) && data.slot.Equals(SpellSlot.Q))
            {
                Misstart = missile.Position.To2D();
                Misend = caster.ServerPosition.To2D();
                data.Range = Misstart.Distance(Misend);
            }

            var detected = new DetectedSkillshotData
            {
                Caster = caster,
                Missile = missile,
                Start = Misstart,
                End = Misend,
                Data = data,
                StartTick = Core.GameTickCount
            };

            Add(detected);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as AIHeroClient;
            if (caster == null || !caster.IsValid || args.IsAutoAttack())
                return;

            var spellname = args.SData.Name;
            //Console.WriteLine(spellname);

            var data = GetData(caster, args, null);

            if (data == null)
            {
                //Console.WriteLine($"OnProcessSkillshots: {caster.ChampionName} - [{args.Slot} | {spellname}] - Not Detected");
                return;
            }

            var start = args.Start.To2D();
            var end = args.End.To2D();
            if (data.StartsFromTarget && target != null)
            {
                if (data.hero == Champion.LeeSin && data.slot == SpellSlot.R)
                {
                    start = target.ServerPosition.To2D();
                    end = start.Extend(caster, -data.Range);
                }

                if (data.hero == Champion.Lissandra && data.slot == SpellSlot.R)
                {
                    start = target.ServerPosition.To2D();
                    end = target.ServerPosition.To2D();
                }
            }

            if (data.RangeScaleWithMoveSpeed)
            {
                data.Range = caster.MoveSpeed * data.MoveSpeedScaleMod;

                if (data.hero == Champion.Warwick && data.slot == SpellSlot.R)
                {
                    data.Range = Math.Max(275, caster.MoveSpeed * data.MoveSpeedScaleMod);
                }

                end = start.Extend(end, data.Range);
            }

            if (data.hero.Equals(Champion.Taric) && data.slot.Equals(SpellSlot.E))
            {
                var taricAllies = EntityManager.Heroes.AllHeroes.Where(h => h.IsValidTarget() && h.Team.Equals(caster.Team) && !h.IdEquals(caster) && h.HasBuff("taricwleashactive"));
                foreach (var ally in taricAllies)
                {
                    var taricAllyE = new DetectedSkillshotData
                    {
                        Caster = ally,
                        Target = target,
                        Start = ally.ServerPosition.To2D(),
                        End = ally.ServerPosition.To2D().Extend(end, data.Range),
                        Data = data,
                        StartTick = Core.GameTickCount
                    };

                    Add(taricAllyE);
                }
            }

            if (data.SpellName.Equals("SiegeLaserAffixShot"))
            {
                var turret = EntityManager.Turrets.AllTurrets.FirstOrDefault(t => t.Team.Equals(caster.Team) && t.Buffs.Any(b => b.Caster.IdEquals(caster)));
                if (turret == null)
                {
                    return;
                }

                start = turret.ServerPosition.To2D();
                end = start.Extend(args.End.To2D(), data.Range);
            }

            var detected = new DetectedSkillshotData
            {
                Caster = caster,
                Target = target,
                Start = start,
                End = end,
                Data = data,
                StartTick = Core.GameTickCount
            };

            Add(detected);
        }

        internal static void Add(DetectedSkillshotData data)
        {
            if (data == null)
            {
                Console.WriteLine("Invalid DetectedSkillshotData");
                return;
            }

            if (!data.Data.AllowDuplicates)
            {
                if (SkillshotsDetected.Any(s => s.Missile != null && data.Missile == null && s.Caster != null && data.Caster != null && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
                {
                    // Already Detected by Missile
                    return;
                }

                var replaceByMissile =
                    SkillshotsDetected.FirstOrDefault(s => s.Missile == null && data.Missile != null && s.Caster != null && data.Caster != null && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data));
                if (replaceByMissile != null && !(data.Data.StaticStart && data.Data.StaticEnd))
                {
                    // Add the Missile
                    replaceByMissile.Missile = data.Missile;
                    replaceByMissile.End = data.Missile.EndPosition.To2D();
                    replaceByMissile.Start = data.Missile.StartPosition.To2D();
                    return;
                }

                if (SkillshotsDetected.Any(s => s.Caster != null && !s.DetectedMissile && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
                    return;
            }

            if (data.Data.StaticStart && data.Data.StaticEnd)
            {
                var start = data.Start;
                var end = data.End;
                data.Start = end - (end - start).Normalized().Perpendicular() * (data.Data.Range / 2);
                data.End = end + (end - start).Normalized().Perpendicular() * (data.Data.Range / 2);
            }

            if (data.Data.SpellName.Equals("YorickE"))
            {
                var start = data.End.Extend(data.Start, 200);
                var end = data.End.Extend(data.Start, -450);
                data.Start = start;
                data.End = end;
            }

            SkillshotsDetected.Add(data);
            Collision.Check(data);
            OnSkillShotDetected.Invoke(data);
        }

        internal static SkillshotData GetData(Obj_AI_Base caster, GameObjectProcessSpellCastEventArgs args, MissileClient missile)
        {
            SkillshotData result = null;
            var hero = caster as AIHeroClient;

            List<SkillshotData> AllData = SkillshotDatabase.List.FindAll(s => hero != null ? s.hero.Equals(Champion.Unknown) || s.hero.Equals(hero.Hero) : !string.IsNullOrEmpty(s.CasterName) && caster.BaseSkinName.StartsWith(s.CasterName));
            if (AllData == null || !AllData.Any())
                return result;

            if (missile == null)
            {
                var slotData = AllData.FindAll(s => s.slot.Equals(args.Slot) || s.slot.Equals(SpellSlot.Unknown) || (s.ExtraSlot != SpellSlot.Unknown && s.ExtraSlot.Equals(args.Slot)));
                if (slotData != null && slotData.Any())
                {
                    var spellname = args.SData.Name;
                    var data =
                        slotData.FirstOrDefault(
                            s =>
                            !string.IsNullOrEmpty(s.SpellName) && s.SpellName.Equals(spellname, StringComparison.CurrentCultureIgnoreCase)
                            || s.ExtraSpellName != null && s.ExtraSpellName.Any(x => x.Equals(spellname, StringComparison.CurrentCultureIgnoreCase)));

                    if (data != null && !data.DetectByMissile && (string.IsNullOrEmpty(data.RequireBuff) || caster.GetBuffCount(data.RequireBuff) >= data.RequireBuffCount) && (data.StartsFromTarget && args.Target != null || !data.StartsFromTarget))
                    {
                        result = data;
                    }
                }
            }
            else
            {
                var missilename = missile.SData.Name;
                var data =
                    AllData.FirstOrDefault(
                        s =>
                        !string.IsNullOrEmpty(s.MissileName) && s.MissileName.Equals(missilename, StringComparison.CurrentCultureIgnoreCase)
                        || s.ExtraMissileName != null && s.ExtraMissileName.Any(x => x.Equals(missilename, StringComparison.CurrentCultureIgnoreCase)));

                if (data != null && (string.IsNullOrEmpty(data.RequireBuff) || caster.GetBuffCount(data.RequireBuff) >= data.RequireBuffCount) && !data.StartsFromTarget)
                {
                    result = data;
                }
            }
            return result;
        }
    }

    internal static class Collision
    {
        internal static void Init()
        {
            YasuoWalls.Init();
            Game.OnTick += Game_OnPreTick;
        }

        private static void Game_OnPreTick(EventArgs args)
        {
            foreach (var skill in SkillshotDetector.SkillshotsDetected.Where(s => s.IsVisible))
            {
                Check(skill);
            }
        }
        
        public static void Check(DetectedSkillshotData skill)
        {
            var collideobjects = new List<Obj_AI_Base>();
            var collidePoints = new List<Vector2>();
            collideobjects.Clear();
            collidePoints.Clear();

            var collisionData = skill.Data.Collisions;

            skill.CollidePoint = null;
            skill.CollideTarget = null;
            skill.CollideTargets = null;

            if (collisionData == null || !collisionData.Any())
                return;

            var polygon = skill.OriginalPolygon;

            if (polygon == null)
                return;

            bool CollideWithWals = false;
            if (collisionData.Contains(Databases.SpellData.Collision.Heros))
            {
                var heros = EntityManager.Heroes.AllHeroes.Where(h => h.Team != skill.Caster.Team && !h.IsMe && h.IsValidTarget() && skill.IsInside(h)).ToList();
                if (heros != null && heros.Any())
                {
                    collideobjects.AddRange(heros);
                    collidePoints.AddRange(heros.Select(h => h.ServerPosition.To2D()));
                }
            }

            if (collisionData.Contains(Databases.SpellData.Collision.Caster))
            {
                if (skill.IsInside(skill.Caster))
                {
                    collideobjects.Add(skill.Caster);
                    collidePoints.Add(skill.Caster.ServerPosition.To2D());
                }
            }

            if (collisionData.Contains(Databases.SpellData.Collision.Minions))
            {
                var minions = EntityManager.MinionsAndMonsters.Combined.Where(h => h.Team != skill.Caster.Team && !h.IsWard() && h.IsValidTarget() && skill.IsInside(h)).ToList();
                if (minions != null && minions.Any())
                {
                    collideobjects.AddRange(minions);
                    collidePoints.AddRange(minions.Select(h => h.ServerPosition.To2D()));
                }
            }

            if (collisionData.Contains(Databases.SpellData.Collision.Walls))
            {
                if (collisionData.Contains(Databases.SpellData.Collision.Walls))
                {
                    var wallsDetected = CellsAnalyze(skill.Start, polygon).Select(c => c.WorldPosition.To2D()).ToList();
                    if (wallsDetected != null && wallsDetected.Any())
                    {
                        collidePoints.AddRange(wallsDetected);
                        CollideWithWals = true;
                    }
                }
            }

            if (collisionData.Contains(Databases.SpellData.Collision.YasuoWall))
            {
                foreach (var wall in YasuoWalls.DetectedWalls.Where(w => w.Polygon != null && w.Caster.Team != skill.Caster.Team))
                {
                    wall.ExtraWidth = skill.Data.Width / 2;
                    var points = wall.Polygon.Points.ToArray();
                    for (int i = 0; i < points.Length; i++)
                    {
                        var intersection = points[i].Intersection(points[i >= 3 ? 0 : i + 1], skill.CurrentPosition, skill.EndPosition);
                        if (intersection.Intersects)
                        {
                            collidePoints.Add(intersection.Point);
                            break;
                        }
                    }
                }
            }

            var orderedPoints = collidePoints.OrderBy(vector2 => vector2.Distance(skill.Start)).ToArray();
            var orderedObjects = collideobjects.OrderBy(obj => obj.Distance(skill.Start)).ToArray();

            if (CollideWithWals)
            {
                if (orderedPoints.Any())
                {
                    var range = Math.Min(skill.Data.Range, orderedPoints[0].Distance(skill.Start));
                    skill.CollidePoint = skill.Start.Extend(skill.EndPosition, range);
                }
            }
            else
            {
                if (orderedPoints.Any())
                {
                    var range = orderedPoints[0].Distance(skill.Start);
                    if (skill.Data.CanCollide)
                    {
                        if (orderedPoints.Length > skill.Data.CollideCount)
                        {
                            var point = orderedPoints[skill.Data.CollideCount];
                            range = point.Distance(skill.Start);
                            var obj = orderedObjects.FirstOrDefault(o => o.ServerPosition.To2D().Equals(point));
                            if (obj != null)
                            {
                                range -= obj.BoundingRadius;
                            }

                            skill.CollidePoint = skill.Start.Extend(skill.EndPosition, range);
                        }
                    }
                    else
                    {
                        skill.CollidePoint = skill.Start.Extend(skill.EndPosition, range);
                    }
                }
            }

            if (orderedObjects.Any())
            {
                skill.CollideTargets = orderedObjects;
                skill.CollideTarget = orderedObjects[0];
            }

            if (orderedObjects.Any())
            {
                skill.CollideTargets = orderedObjects;
                skill.CollideTarget = orderedObjects[0];
            }
        }

        private static IEnumerable<NavMeshCell> CellsAnalyze(Vector2 pos, EloBuddy.SDK.Geometry.Polygon poly)
        {
            var sourceGrid = pos.ToNavMeshCell();
            var GridSize = 50f;
            var startPos = new NavMeshCell(sourceGrid.GridX - (short)Math.Floor(GridSize / 2f), sourceGrid.GridY - (short)Math.Floor(GridSize / 2f));

            var cells = new List<NavMeshCell> { startPos };
            for (var y = startPos.GridY; y < startPos.GridY + GridSize; y++)
            {
                for (var x = startPos.GridX; x < startPos.GridX + GridSize; x++)
                {
                    if (x == startPos.GridX && y == startPos.GridY)
                    {
                        continue;
                    }
                    if (x == sourceGrid.GridX && y == sourceGrid.GridY)
                    {
                        cells.Add(sourceGrid);
                    }
                    else
                    {
                        cells.Add(new NavMeshCell(x, y));
                    }
                }
            }

            return cells.Where(c => poly.IsInside(c.WorldPosition) && (c.WorldPosition.IsBuilding() || c.WorldPosition.IsWall()));
        }
    }

    public static class YasuoWalls
    {
        public class YasuoWall
        {
            public AIHeroClient Caster;
            public MissileClient Left;
            public MissileClient Mid;
            public MissileClient Right;
            public float ExtraWidth = 30;
            public float StartTick = Core.GameTickCount;
            public EloBuddy.SDK.Geometry.Polygon.Rectangle Polygon
            {
                get
                {
                    var extra = ExtraWidth;
                    var width = 120;

                    if (this.Left != null && this.Right != null)
                        return new EloBuddy.SDK.Geometry.Polygon.Rectangle(Left.Position.Extend(Right.Position, -extra), Right.Position.Extend(Left.Position, -extra), width);

                    if (this.Left != null && this.Mid != null)
                    {
                        var dis = Left.Distance(Mid) * 2;
                        return new EloBuddy.SDK.Geometry.Polygon.Rectangle(Left.Position.Extend(Mid.Position, -extra), Left.Position.Extend(this.Mid.Position, dis + extra), width);
                    }

                    if (this.Right != null && this.Mid != null)
                    {
                        var dis = Right.Distance(Mid) * 2;
                        return new EloBuddy.SDK.Geometry.Polygon.Rectangle(Right.Position.Extend(Mid.Position, -extra), Right.Position.Extend(this.Mid.Position, dis + extra), width);
                    }
                    return null;
                }
            }
        }

        public static List<YasuoWall> DetectedWalls = new List<YasuoWall>();

        private const string _leftMissile = "YasuoWMovingWallMisL";
        private const string _rightMissile = "YasuoWMovingWallMisR";
        private const string _midMissile = "YasuoWMovingWallMisVis";

        public static void Init()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            //GameObject.OnDelete += GameObject_OnDelete;
            Game.OnTick += GameOnOnTick;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            return;

            foreach (var wall in DetectedWalls.Where(w => w.Polygon != null))
            {
                wall.Polygon.Draw(System.Drawing.Color.AliceBlue, 2);
            }
        }

        private static void GameOnOnTick(EventArgs args)
        {
            DetectedWalls.RemoveAll(w => Core.GameTickCount - w.StartTick > 4000);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null)
                return;

            if (!caster.BaseSkinName.Equals("Yasuo"))
                return;

            var alreadyDetected = DetectedWalls.FirstOrDefault(w => w.Caster.IdEquals(caster));

            if (alreadyDetected == null)
                return;

            var missilename = missile.SData.Name;
            var left = missilename.Equals(_leftMissile);
            var mid = missilename.Equals(_midMissile);
            var right = missilename.Equals(_rightMissile);

            if (left || mid || right)
                DetectedWalls.Remove(alreadyDetected);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null)
                return;

            if (!caster.BaseSkinName.Equals("Yasuo"))
                return;

            var alreadyDetected = DetectedWalls.FirstOrDefault(w => w.Caster.IdEquals(caster));

            var missilename = missile.SData.Name;
            var left = missilename.Equals(_leftMissile);
            var mid = missilename.Equals(_midMissile);
            var right = missilename.Equals(_rightMissile);

            if (alreadyDetected == null)
            {
                var newDetected = new YasuoWall { Caster = caster };
                if (left)
                    newDetected.Left = missile;
                if (mid)
                    newDetected.Mid = missile;
                if (right)
                    newDetected.Right = missile;

                DetectedWalls.Add(newDetected);
                return;
            }

            if (left)
                alreadyDetected.Left = missile;
            if (mid)
                alreadyDetected.Mid = missile;
            if (right)
                alreadyDetected.Right = missile;
        }
    }
}
