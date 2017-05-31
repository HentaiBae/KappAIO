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
using Collision = KappAIO_Reborn.Common.Databases.SpellData.Collision;
using Type = KappAIO_Reborn.Common.Databases.SpellData.Type;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class SkillshotDetector
    {
        private static bool Loaded;
        static SkillshotDetector()
        {
            if (!Loaded)
            {
                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Spellbook.OnStopCast += Spellbook_OnStopCast;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
                Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
                Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
                Loaded = true;
                OnSkillShotDelete.OnDelete += OnSkillShotDelete_OnDelete;
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
            public bool Added;
            public bool FullyDetected => this.caster != null && ((this.Start != null && (this.Mid != null || this.End != null)) || (this.End != null && (this.Mid != null || this.Start != null)));
        }
        public class IllaoiTentacle
        {
            public AIHeroClient caster;
            public Obj_AI_Minion Tentacle;
            public Obj_AI_Base Target;
            public float StartTick = Core.GameTickCount;
            public bool Attacked;
        }

        public static List<IllaoiTentacle> IllaoiTentacles = new List<IllaoiTentacle>();
        public static List<LuxRPartical> DetectedLuxRParticals = new List<LuxRPartical>();
        public static List<DetectedSkillshotData> SkillshotsDetected = new List<DetectedSkillshotData>();

        private static float lastTrapsCheck;
        private static void FindAndAddTraps()
        {
            if (Core.GameTickCount - lastTrapsCheck > 200)
            {
                lastTrapsCheck = Core.GameTickCount;
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValid && !x.IsDead))
                {
                    var data = SkillshotDatabase.Current.FirstOrDefault(s => s.IsTrap && s.IsTrapBaseSkinName(minion.BaseSkinName));
                    if (data != null)
                    {
                        var buff = minion.Buffs.FirstOrDefault(b =>
                        {
                            var buffCaster = b.Caster as Obj_AI_Base;
                            return buffCaster != null && b.IsValid && b.IsActive && data.IsCasterName(buffCaster.BaseSkinName)
                            && (data.IsTrapBuff(b.DisplayName) || data.IsTrapBuff(b.Name));
                        });
                        var caster = buff?.Caster as Obj_AI_Base;
                        if (caster != null)
                        {
                            Add(new DetectedSkillshotData
                            {
                                Caster = caster,
                                Trap = minion,
                                Start = minion.ServerPosition.To2D(),
                                End = minion.ServerPosition.To2D(),
                                StartTick = buff.StartTime * 1000f,
                                FixedEndTick = buff.EndTime * 1000f,
                                Data = data,
                            });
                        }
                    }
                }
            }
        }

        private static void OnSkillShotDelete_OnDelete(DetectedSkillshotData args)
        {
            if (args.Data.OnDeleteAdd != null)
            {
                var newDetect = args;
                newDetect.Data = args.Data.OnDeleteAdd;
                newDetect.StartTick = Core.GameTickCount;
                Add(newDetect);
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                SkillshotsDetected.RemoveAll(s => s.Caster.IdEquals(hero) && !string.IsNullOrEmpty(s.Data.RemoveOnBuffLose)
                && args.Buff.Caster.IdEquals(hero)
                && (args.Buff.DisplayName.Equals(s.Data.RemoveOnBuffLose, StringComparison.CurrentCultureIgnoreCase)
                || args.Buff.Name.Equals(s.Data.RemoveOnBuffLose, StringComparison.CurrentCultureIgnoreCase)));
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            FindAndAddTraps();
            IllaoiTentacles.RemoveAll(s => Core.GameTickCount - s.StartTick > 1250 || s.Attacked);
            DetectedLuxRParticals.RemoveAll(s => Core.GameTickCount - s.StartTick > s.Data.CastDelay || SkillshotsDetected.Any(x => s.caster.IdEquals(x.Caster) && s.Data.Equals(x.Data)));
            SkillshotsDetected.RemoveAll(s => s.Ended && OnSkillShotDelete.Invoke(s));

            foreach (var luxR in DetectedLuxRParticals.Where(r => r.FullyDetected && !r.Added && !SkillshotsDetected.Any(s => s.Caster.IdEquals(r.caster) && s.Data.Equals(r.Data))))
            {
                Vector2? start = luxR.Start;
                Vector2? end = luxR.End;
                if (!start.HasValue)
                {
                    if (end.HasValue && luxR.Mid.HasValue)
                        start = end.Value.Extend(luxR.Mid.Value, luxR.Data.Range);
                }
                if (!end.HasValue)
                {
                    if (start.HasValue && luxR.Mid.HasValue)
                        end = start.Value.Extend(luxR.Mid.Value, luxR.Data.Range);
                }

                if (start.HasValue && end.HasValue)
                {
                    start = start.Value.Extend(end.Value, -150);
                    end = end.Value.Extend(start.Value, 150);
                    var detectd = new DetectedSkillshotData
                    {
                        Caster = luxR.caster,
                        StartTick = Core.GameTickCount,
                        Start = start.Value,
                        End = end.Value,
                        Data = luxR.Data
                    };

                    Add(detectd);
                    luxR.Added = true;
                }
            }

            foreach (var skill in SkillshotsDetected.Where(s => s != null && !s.Ended))
            {
                skill.Update();
            }
        }

        private static string[] AlliedNames = { "greenground.troy", "green.troy", "green_team.troy", "green_ring.troy", "_ally.troy", "blue.troy", "_ally_green.troy", "_ground_ally.troy", "_cas_green.troy" };
        private static string[] ExcludeEnds = { "_explosion.troy", "_end.troy", "_mis.troy", "_aoe_explosion.troy", "_impact.troy", "_charge.troy", "_tar.troy", "_end.troy", "_sound.troy" };

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender == null)
                return;

            if (!sender.BaseSkinName.Equals("IllaoiMinion") || !args.Animation.Contains("Attack"))
                return;

            var tentacles = IllaoiTentacles.FindAll(x => x.Tentacle.IdEquals(sender));
            var illaoiw = SkillshotDatabase.Current.FirstOrDefault(s => s.IsCasterName("Illaoi") && s.IsSlot(SpellSlot.W));
            if (tentacles.Any())
            {
                foreach (var t in IllaoiTentacles.Where(x => x.Tentacle.IdEquals(sender) && x.Target != null))
                {
                    var detected = new DetectedSkillshotData
                    {
                        Caster = t.caster,
                        Data = illaoiw,
                        Start = sender.ServerPosition.To2D(),
                        End = t.Target.ServerPosition.To2D(),
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
                    var target = EntityManager.Heroes.AllHeroes.FindAll(h => h.Buffs.Any(b => b.Caster.IdEquals(caster))).OrderBy(h => h.Distance(sender)).FirstOrDefault(t => !t.IdEquals(caster));
                    var correct = illaoiESpirit ?? target;
                    if (correct != null)
                    {
                        var detected = new DetectedSkillshotData
                        {
                            Caster = caster,
                            Data = illaoiw,
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

            var illaoiw = SkillshotDatabase.Current.FirstOrDefault(s => s.IsCasterName("Illaoi") && s.IsSlot(SpellSlot.W));
            if (illaoiw == null)
                return;

            foreach (var tentacle in illaoiMinions.Where(m => m.IsInRange(target.PrediectPosition(), illaoiw.Range * 1.1f)))
            {
                IllaoiTentacles.Add(new IllaoiTentacle { caster = caster, Target = target, Tentacle = tentacle });
            }
        }

        private static void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.ChampionName.Equals("Sion"))
                return;

            SkillshotsDetected.RemoveAll(s => s.Caster.IdEquals(caster) && OnSkillShotDelete.Invoke(s)); // Clear all sion Skills when he stops casting
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var partical = sender as Obj_GeneralParticleEmitter;
            if (partical != null)
            {
                SkillshotsDetected.RemoveAll(s => s.Particle != null && !s.Data.DontRemoveWithMissile && s.Particle.IdEquals(partical));
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || !missile.IsValid)
                return;

            var name = missile.SData.Name;
            if (SkillshotsDetected.Any(s => s.Caster != null && s.Missile != null && s.Missile.IdEquals(missile) && caster.IdEquals(s.Caster)))
            {
                SkillshotsDetected.RemoveAll(s => s.Caster != null && s.Missile != null && !s.Data.DontRemoveWithMissile && s.Missile.IdEquals(missile)
                && s.Data.IsMissileName(name) && caster.IdEquals(s.Caster) && s.TicksPassed > 15 && OnSkillShotDelete.Invoke(s));
            }

            var endExplode = SkillshotDatabase.Current.FirstOrDefault(s => s.AddEndExplode && s.IsMissileName(name));
            if (endExplode != null)
            {
                var end = missile.Position.To2D();
                if (endExplode.IsDisplayName("Karma Q Explode"))
                {
                    var tar = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget() && !o.IsStructure() && o.Team != missile.SpellCaster.Team && o.IsInRange(missile, 100 + o.BoundingRadius)).OrderBy(o => o.Distance(missile)).FirstOrDefault();
                    if (tar != null)
                    {
                        end = tar.Position.To2D();
                    }
                }

                Add(new DetectedSkillshotData
                {
                    Caster = missile.SpellCaster,
                    Data = endExplode,
                    Start = end,
                    End = end,
                    StartTick = Core.GameTickCount,
                });
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var partical = sender as Obj_GeneralParticleEmitter;
            if (partical != null)
            {
                var particleName = partical.Name.ToLower();
                var pdata = SkillshotDatabase.Current.FirstOrDefault(s => !ExcludeEnds.Any(particleName.EndsWith) && s.IsParticleName(particleName) && ObjectManager.Get<Obj_AI_Base>().Any(s.HasBuff));
                if (pdata != null)
                {
                    var isAlly = AlliedNames.Any(particleName.EndsWith);
                    var yasuoEQ = partical.Name.StartsWith("Yasuo_Base_EQ_SwordGlow");
                    var jungleMob = pdata.IsCasterName("jungle");

                    Obj_AI_Base pcaster =
                        jungleMob ? ObjectManager.Get<Obj_AI_Base>().Where(m => pdata.IsCasterName(m.BaseSkinName) && m.IsValid && !m.IsDead).OrderBy(m => m.Distance(partical)).FirstOrDefault()
                        : yasuoEQ ? EntityManager.Heroes.AllHeroes.OrderBy(m => m.Distance(partical)).FirstOrDefault(h => h.ChampionName == "Yasuo" && h.IsValid && !h.IsDead)
                        : EntityManager.Heroes.AllHeroes.OrderByDescending(h => h.Distance(partical)).FirstOrDefault(h => h.IsInRange(partical, 3000) && pdata.IsCasterName(h.BaseSkinName)
                        && (isAlly ? h.IsAlly : h.IsEnemy) && h.IsValid && !h.IsDead && pdata.HasBuff(h));

                    var correctObject = ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.Distance(partical)).FirstOrDefault(o => !string.IsNullOrEmpty(pdata.ParticalObjectName)
                    && (o.Name.Equals(pdata.ParticalObjectName) || o.BaseSkinName.Equals(pdata.ParticalObjectName) || o.Model.Equals(pdata.ParticalObjectName) || pdata.HasBuff(o)));
                    if (correctObject != null)
                    {
                        var buff = correctObject.Buffs.FirstOrDefault(b => b.Caster is Obj_AI_Base);
                        var correctCaster = buff?.Caster as Obj_AI_Base;
                        if (correctCaster != null)
                            pcaster = correctCaster;

                        if (pcaster != null)
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
                    }
                    else
                    {
                        var start = partical.Position.To2D();
                        var end = partical.Position.To2D();
                        if (pcaster != null)
                        {
                            //start = pcaster.ServerPosition.To2D();
                            switch (pcaster.BaseSkinName)
                            {
                                case "Yasuo":
                                    {
                                        var alreadydetected = SkillshotsDetected.FirstOrDefault(s => s.Caster.IdEquals(pcaster));
                                        if (alreadydetected != null)
                                        {
                                            var qcircle = SkillshotDatabase.Current.FirstOrDefault(s => s.IsCasterName(Champion.Yasuo) && s.IsDisplayName(pcaster.HasBuff("YasuoQ3W") ? "E Q3" : "E Q"));
                                            if (qcircle != null)
                                            {
                                                alreadydetected.Data = qcircle;
                                                alreadydetected.End = pcaster.GetDashInfo() != null ? pcaster.GetDashInfo().EndPos.To2D() : pcaster.ServerPosition.To2D();
                                                alreadydetected.Update();
                                            }
                                        }
                                        return;
                                    }
                                case "Zac":
                                    start = pcaster.IsHPBarRendered ? pcaster.ServerPosition.To2D() : end.Extend(pcaster, pdata.Range);
                                    break;
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
                }
                else
                {
                    var StartluxR =
                        SkillshotDatabase.Current.FirstOrDefault(p => !string.IsNullOrEmpty(p.StartParticalName) && partical.Name.StartsWith(p.StartParticalName) && partical.Name.EndsWith(".troy"));
                    var MidluxR =
                        SkillshotDatabase.Current.FirstOrDefault(p => !string.IsNullOrEmpty(p.MidParticalName) && partical.Name.StartsWith(p.MidParticalName) && partical.Name.EndsWith(".troy"));
                    var EndluxR =
                        SkillshotDatabase.Current.FirstOrDefault(p => !string.IsNullOrEmpty(p.EndParticalName) && partical.Name.StartsWith(p.EndParticalName) && partical.Name.EndsWith(".troy"));

                    AIHeroClient pcaster;
                    if (StartluxR != null || MidluxR != null || EndluxR != null)
                    {
                        pdata = SkillshotDatabase.Current.FirstOrDefault(s => s.IsCasterName(Champion.Lux) && s.IsSlot(SpellSlot.R));
                        var luxInStart = EntityManager.Heroes.AllHeroes.FirstOrDefault(h => h.Hero.Equals(Champion.Lux) && h.IsValidTarget() && h.IsInRange(partical, 275));
                        pcaster = luxInStart ?? EntityManager.Heroes.AllHeroes.OrderBy(h => h.Distance(partical)).FirstOrDefault(h => h.BaseSkinName.Equals("Lux") && (h.Spellbook.IsChanneling || h.Spellbook.IsCharging || h.Spellbook.IsCastingSpell || !h.IsHPBarRendered));

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
                                return;
                            }
                            var addnew = new LuxRPartical
                            {
                                caster = pcaster,
                                Start = StartluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                Mid = MidluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                End = EndluxR != null ? partical.Position.To2D() : (Vector2?)null,
                                Data = pdata
                            };

                            DetectedLuxRParticals.Add(addnew);
                            return;
                        }
                    }
                }
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster;
            if (missile == null || caster == null || !missile.IsValid)
                return;

            //var missilename = missile.SData.Name;
            //Console.WriteLine($"{missilename} - mis - {missile.Slot}");

            var data = GetData(caster, null, missile);

            if (data == null)
            {
                //Console.WriteLine($"OnCreateSkillshots: {caster.ChampionName} - [{missile.Slot} | {missilename}] - Not Detected");
                return;
            }

            var Misstart = missile.StartPosition.To2D();
            var Misend = missile.EndPosition.To2D();

            if (data.IsCasterName(Champion.Shen) && data.IsSlot(SpellSlot.Q))
            {
                Misstart = missile.Position.To2D();
                Misend = caster.ServerPosition.To2D();
                data.Range = Misstart.Distance(Misend);
            }

            var detected = new DetectedSkillshotData
            {
                Caster = caster,
                Target = missile.Target as AIHeroClient,
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
            if (caster == null || !caster.IsValid)
                return;

            //var spellname = args.SData.Name;
            //Console.WriteLine(spellname);

            var data = GetData(caster, args, null);

            if (data == null)
            {
                //Console.WriteLine($"OnProcessSkillshots: {caster.ChampionName} - [{args.Slot} | {spellname}] - Not Detected");
                return;
            }

            var start = args.Start.To2D();
            var end = args.End.To2D();
            if (target != null && (!target.IsMe || caster.IsAlly))
            {
                if (data.StartsFromTarget)
                {
                    if (data.IsCasterName("LeeSin") && data.IsSlot(SpellSlot.R))
                    {
                        start = target.ServerPosition.To2D();
                        end = start.Extend(caster, -data.Range);
                    }

                    if (data.IsCasterName(Champion.Lissandra) && data.IsSlot(SpellSlot.R))
                    {
                        start = target.ServerPosition.To2D();
                        end = target.ServerPosition.To2D();
                    }
                }

                if (data.EndSticksToTarget)
                {
                    end = target.ServerPosition.To2D();
                    start = caster.ServerPosition.To2D();

                    if (data.IsCasterName(Champion.Nautilus))
                    {
                        start = caster.ServerPosition.To2D().Extend(end, caster.GetAutoAttackRange());
                    }
                }
            }

            if (data.RangeScaleWithMoveSpeed)
            {
                data.Range = caster.MoveSpeed * data.MoveSpeedScaleMod;

                if (data.IsCasterName(Champion.Warwick) && data.IsSlot(SpellSlot.R))
                {
                    data.Range = Math.Max(275, caster.MoveSpeed * data.MoveSpeedScaleMod);
                }

                end = start.Extend(end, data.Range);
            }

            if (data.IsCasterName(Champion.Taric) && data.IsSlot(SpellSlot.E))
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

            if (data.IsSpellName("SiegeLaserAffixShot"))
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
                Console.WriteLine("Invalid DetectedSkillshot");
                return;
            }
            
            if (data.Data.IsTrap && SkillshotsDetected.Any(x => x.Trap != null && data.Trap.Equals(x.Trap)))
                return;

            if (data.Missile == null && data.Data.DetectByMissile)
            {
                return;
            }
            
            if (data.Data.type == Type.Cone)
            {
                if (SkillshotsDetected.Any(s => s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
                {
                    return;
                }
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
                if (replaceByMissile != null && data.Missile != null && !(data.Data.StaticStart && data.Data.StaticEnd))
                {
                    // Add the Missile
                    replaceByMissile.Missile = data.Missile;
                    replaceByMissile.End = data.Missile.EndPosition.To2D();
                    replaceByMissile.Start = data.Missile.StartPosition.To2D();
                    return;
                }
                
                if (SkillshotsDetected.Any(s => s.Caster != null && !s.DetectedByMissile && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
                    return;
                
            }

            if (data.Data.StaticStart && data.Data.StaticEnd && data.Missile == null)
            {
                var start = data.Start;
                var end = data.End;
                data.Start = end - (end - start).Normalized().Perpendicular() * (data.Range / 2);
                data.End = end + (end - start).Normalized().Perpendicular() * (data.Range / 2);
            }

            if (data.Data.IsSpellName("YorickE"))
            {
                var start = data.End.Extend(data.Start, 200);
                var end = data.End.Extend(data.Start, -450);
                data.Start = start;
                data.End = end;
            }

            if (data.Data.CasterNames.Length == 3 && data.Data.type == Type.CircleMissile)
            {
                var obj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsValid && !o.IsDead && o.Team == data.Caster.Team && data.Data.CasterNames[1].Equals(o.BaseSkinName) && data.Data.CasterNames[2].Equals(o.Model));
                if (obj != null)
                {
                    data.Start = obj.ServerPosition.To2D();
                    data.End = obj.ServerPosition.To2D();
                }
            }

            if (data.Data.IsSpellName("JarvanIVDragonStrike"))
            {
                var jarvanFlag = ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.Distance(data.EndPosition)).FirstOrDefault(o => o.BaseSkinName.Equals("JarvanIVStandard") && o.Team == data.Caster.Team && o.IsValid && data.IsInside(o));
                if (jarvanFlag != null)
                {
                    var jarvanEQData = SkillshotDatabase.Current.FirstOrDefault(s => s.IsSpellName("JarvanIVEQ"));
                    if (jarvanEQData != null)
                    {
                        var jarvanEQ = new DetectedSkillshotData
                        {
                            Data = jarvanEQData,
                            Start = data.Start,
                            End = jarvanFlag.ServerPosition.To2D(),
                            Caster = data.Caster,
                            StartTick = Core.GameTickCount,
                        };

                        Add(jarvanEQ);
                        return;
                    }
                }
            }

            if (data.Data.DecaySpeedWithLessRange)
            {
                data.Speed = data.Data.Speed * (data.Start.Distance(data.EndPosition) / data.Data.Range);
            }

            data.DetectedByMissile = data.Missile != null;
            data.FromFOW = !data.Caster.IsHPBarRendered;

            data.Update();
            SkillshotsDetected.Add(data);
            OnSkillShotDetected.Invoke(data);

            if (data.Data.IsSpellName("SyndraE"))
            {
                var qeData = SkillshotDatabase.Current.FirstOrDefault(s => s.IsSpellName("SyndraEQ"));
                if (qeData != null)
                {
                    var syndraBalls = ObjectManager.Get<Obj_AI_Base>().Where(o => o.BaseSkinName.Equals("SyndraSphere") && o.Team == data.Caster.Team && data.IsInside(o) && o.IsValid && o.Mana > 17.5);
                    foreach (var ball in syndraBalls)
                    {
                        var newDetect = new DetectedSkillshotData
                        {
                            Caster = data.Caster,
                            Start = ball.ServerPosition.To2D(),
                            End = data.Start.Extend(ball.ServerPosition.To2D(), qeData.Range),
                            Data = qeData,
                            StartTick = Core.GameTickCount,
                            extraDelay = data.TravelTime(ball)
                        };

                        newDetect.Update();
                        SkillshotsDetected.Add(newDetect);
                        OnSkillShotDetected.Invoke(newDetect);
                    }
                }
            }
        }

        private static SkillshotData GetData(Obj_AI_Base caster, GameObjectProcessSpellCastEventArgs args, MissileClient missile = null)
        {
            var spellName = args?.SData.Name ?? missile?.SData.Name;
            var slot = args?.Slot ?? missile?.Slot;
            var target = (args?.Target ?? missile?.Target) as Obj_AI_Base;
            var isAutoAttack = (args?.IsAutoAttack() ?? missile?.IsAutoAttack()).GetValueOrDefault();

            SkillshotData result = SkillshotDatabase.Current.FirstOrDefault(s =>
            s.IsCasterName(caster.BaseSkinName) && (missile != null || s.IsSlot(slot)) && s.HasBuff(caster)
            && ((missile == null && s.IsSpellName(spellName)) || (args == null && s.IsMissileName(spellName)))
            && (!s.DetectByMissile || missile != null)
            && (!(s.EndSticksToTarget || s.StartsFromTarget || s.IsAutoAttack) || (target != null && !target.IsMe))
            && (!isAutoAttack || s.IsAutoAttack));

            return result;
        }
    }

    public class CollisionResult
    {
        public CollisionResult(DetectedSkillshotData skillshot)
        {
            this.Check(skillshot);
        }

        public Vector2? CorrectCollidePoint = null;
        public Vector2? CollidePoint = null;

        private void Check(DetectedSkillshotData skillshot)
        {
            if (skillshot == null)
                return;

            var coll = skillshot.Data.Collisions;
            var targets = new List<Vector2>();

            if (coll.Contains(Collision.Caster) && skillshot.IsInside(skillshot.Caster) && skillshot.GetHealthPrediction(skillshot.Caster) > 0)
                targets.Add(skillshot.Caster.ServerPosition.To2D().Extend(skillshot.Start, skillshot.Caster.BoundingRadius - 35));
            
            if (coll.Contains(Collision.Heros))
                targets.AddRange(EntityManager.Heroes.AllHeroes.FindAll(h => !h.IsMe && h.IsValidTarget()
                && h.Team != skillshot.Caster.Team && skillshot.IsInside(h) && skillshot.GetHealthPrediction(h) > 0)
                .Select(h => h.ServerPosition.To2D().Extend(skillshot.Start, h.BoundingRadius - 35)));
            
            if (coll.Contains(Collision.Minions))
                targets.AddRange(EntityManager.MinionsAndMonsters.Combined.Where(h => !h.IsMe && h.IsValidTarget()
                && h.Team != skillshot.Caster.Team && skillshot.IsInside(h) && skillshot.GetHealthPrediction(h) > 1).
                Select(h => h.ServerPosition.To2D().Extend(skillshot.Start, h.BoundingRadius - 35)));
            
            if (coll.Contains(Collision.YasuoWall))
            {
                var collidePoint = skillshot.CurrentPosition.GetYasuoWallCollision(skillshot.EndPosition, false, true);
                if (collidePoint.IsValid())
                    targets.Add(collidePoint.To2D());
            }

            if (coll.Contains(Collision.Walls))
                targets.AddRange(this.CellsAnalyze(skillshot.CurrentPosition, skillshot.generatePolygon2()).Select(c => c.WorldPosition.To2D()));

            if (!targets.Any())
                return;

            var orderPoints = targets.OrderBy(t => t.Distance(skillshot.Start)).ToArray();
            var multiCollide = skillshot.Data.CollideCount > 0;
            Vector2 collisionPoint = orderPoints[multiCollide && targets.Count > skillshot.Data.CollideCount + 1 ? skillshot.Data.CollideCount : 0];
            Vector2 extendToCollidePoint = skillshot.Start.Extend(skillshot.EndPosition, skillshot.Start.Distance(collisionPoint));

            this.CorrectCollidePoint = collisionPoint;
            this.CollidePoint = extendToCollidePoint;
        }

        private IEnumerable<NavMeshCell> CellsAnalyze(Vector2 pos, Geometry.Polygon poly) // Credits to Hellsing
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
