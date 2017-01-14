using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;

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
                Loaded = true;
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
            foreach (var skill in SkillshotsDetected)
                OnSkillShotDetected.Invoke(skill);

            SkillshotsDetected.RemoveAll(s => s.Ended);
        }

        public static List<DetectedSkillshotData> SkillshotsDetected = new List<DetectedSkillshotData>();

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || missile.IsAutoAttack() || !missile.IsValid)
                return;

            if (SkillshotsDetected.Any(s => s.Caster != null && s.Missile != null && s.Missile.IdEquals(missile) && caster.IdEquals(s.Caster)))
            {
                SkillshotsDetected.RemoveAll(s => s.Caster != null && s.Missile != null && s.Missile.IdEquals(missile) && caster.IdEquals(s.Caster));
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var partical = sender as Obj_GeneralParticleEmitter;
            if (partical != null)
            {
                var pdata = SkillshotDatabase.List.FirstOrDefault(s => !string.IsNullOrEmpty(s.ParticalName) && s.ParticalName.Equals(partical.Name));
                if (pdata != null)
                {
                    var correctObject = ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.Distance(partical)).FirstOrDefault(o => !string.IsNullOrEmpty(pdata.ParticalObjectName) && o.Name.Equals(pdata.ParticalObjectName));
                    if (correctObject != null)
                    {
                        var pcaster = EntityManager.Heroes.AllHeroes.OrderBy(e => e.Distance(correctObject)).FirstOrDefault(h => h.Hero.Equals(pdata.hero));

                        var pdetected = new DetectedSkillshotData
                        {
                            Caster = pcaster,
                            Start = correctObject.Position,
                            End = correctObject.Position,
                            Data = pdata,
                            StartTick = Core.GameTickCount
                        };

                        Add(pdetected);
                    }
                }
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || missile.IsAutoAttack() || !missile.IsValid)
                return;

            var missilename = missile.SData.Name;
            //Console.WriteLine($"{missilename} - mis - {missile.Slot}");

            var data = GetData(caster, null, missile);

            if (data == null)
            {
                //Console.WriteLine($"OnCreateSkillshots: {caster.ChampionName} - [{missile.Slot} | {missilename}] - Not Detected");
                return;
            }

            var detected = new DetectedSkillshotData
            {
                Caster = caster,
                Missile = missile,
                Start = missile.StartPosition,
                End = missile.EndPosition,
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

            var spellname = args.SData.Name;
            //Console.WriteLine(spellname);

            var data = GetData(caster, args, null);

            if (data == null)
            {
                //Console.WriteLine($"OnProcessSkillshots: {caster.ChampionName} - [{args.Slot} | {spellname}] - Not Detected");
                return;
            }

            var start = args.Start;
            var end = args.End;
            if (data.StartsFromTarget && target != null)
            {
                start = target.ServerPosition;
                end = start.Extend(caster, -data.Range).To3D();
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

        private static void Add(DetectedSkillshotData data)
        {
            if (data == null)
            {
                Console.WriteLine("Invalid DetectedSkillshotData");
                return;
            }

            if (SkillshotsDetected.Any(s => s.Missile != null && data.Missile == null && s.Caster != null && data.Caster != null && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
            {
                // Already Detected by Missile
                return;
            }

            var replaceByMissile =
                SkillshotsDetected.FirstOrDefault(s => s.Missile == null && data.Missile != null && s.Caster != null && data.Caster != null && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data));
            if (replaceByMissile != null)
            {
                // Add the Missile
                replaceByMissile.Missile = data.Missile;
                replaceByMissile.End = data.Missile.EndPosition;
                replaceByMissile.Start = data.Missile.StartPosition;
                return;
            }

            if (SkillshotsDetected.Any(s => s.Caster != null && !s.DetectedMissile && s.Caster.IdEquals(data.Caster) && s.Data.Equals(data.Data)))
                return;

            OnSkillShotDetected.Invoke(data);
            SkillshotsDetected.Add(data);
        }

        internal static SkillshotData GetData(AIHeroClient caster, GameObjectProcessSpellCastEventArgs args, MissileClient missile)
        {
            SkillshotData result = null;
            var AllData = SkillshotDatabase.List.FindAll(s => s.hero.Equals(caster.Hero));
            if (AllData == null || !AllData.Any())
                return result;

            if (missile == null)
            {
                var slotData = AllData.FindAll(s => s.slot.Equals(args.Slot));
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
                        s.slot.Equals(missile.Slot) || !string.IsNullOrEmpty(s.MissileName) && s.MissileName.Equals(missilename, StringComparison.CurrentCultureIgnoreCase)
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
            Game.OnTick += Game_OnPreTick;
        }

        private static void Game_OnPreTick(EventArgs args)
        {
            foreach (var skill in SkillshotDetector.SkillshotsDetected)
            {
                var collideobjects = new List<Obj_AI_Base>();

                var collisionData = skill.Data.Collisions;

                var polygon = skill.OriginalPolygon;

                if (collisionData == null)
                {
                    collideobjects.AddRange(EntityManager.Allies.Where(h => h.Team != skill.Caster.Team && h.IsValidTarget(1500) && polygon != null && polygon.IsInside(h)));
                    collideobjects.AddRange(EntityManager.Enemies.Where(h => h.Team != skill.Caster.Team && h.IsValidTarget(1500) && polygon != null && polygon.IsInside(h)));

                    skill.CollidePoint = null;
                    skill.CollideTarget = null;
                    skill.CollideTargets = collideobjects.ToArray();
                    return;
                }

                collideobjects.Clear();

                if (collisionData.Contains(Databases.SpellData.Collision.Caster) && skill.Caster != null && polygon.IsInside(skill.Caster))
                    collideobjects.Add(skill.Caster);

                if (collisionData.Contains(Databases.SpellData.Collision.Heros))
                    collideobjects.AddRange(EntityManager.Heroes.AllHeroes.Where(h => h.Team != skill.Caster.Team && !h.IsMe && h.IsValidTarget(1500) && polygon != null && polygon.IsInside(h)));

                if (collisionData.Contains(Databases.SpellData.Collision.Minions))
                    collideobjects.AddRange(EntityManager.MinionsAndMonsters.CombinedAttackable.Where(h => h.Team != skill.Caster.Team && !h.IsWard() && h.IsValidTarget(1500) && polygon != null && polygon.IsInside(h)));

                if (!collideobjects.Any())
                {
                    skill.CollideTarget = null;
                    skill.CollidePoint = null;
                    skill.CollideTargets = null;
                    return;
                }

                Obj_AI_Base collide = null;

                if (skill.Data.CollideCount != 0)
                {
                    if (collideobjects.Count > skill.Data.CollideCount)
                    {
                        if (collideobjects.Count == 1)
                            collide = collideobjects.FirstOrDefault();
                        else if (collideobjects.Count == skill.Data.CollideCount + 1)
                            collide = collideobjects.OrderBy(o => o.Distance(skill.CurrentPosition)).ToArray().LastOrDefault();
                        else
                            collide = collideobjects.OrderBy(o => o.Distance(skill.CurrentPosition)).ToArray()[skill.Data.CollideCount];
                    }
                }
                else
                {
                    collide = collideobjects.OrderBy(o => o.Distance(skill.CurrentPosition, true)).FirstOrDefault();
                }

                if (collide == null)
                {
                    skill.CollideTarget = null;
                    skill.CollidePoint = null;
                    skill.CollideTargets = collideobjects.ToArray();
                    return;
                }

                var range = skill.Start.Distance(collide.ServerPosition);
                skill.CollidePoint = skill.Start.Extend(skill.End, range).To3D();
                skill.CollideTarget = collide;
                skill.CollideTargets = collideobjects.ToArray();
            }
        }
    }
}
