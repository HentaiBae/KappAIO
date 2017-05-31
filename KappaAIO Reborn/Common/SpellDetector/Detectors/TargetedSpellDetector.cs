using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class TargetedSpellDetector
    {
        private static bool Loaded;
        static TargetedSpellDetector()
        {
            if (!Loaded)
            {
                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Loaded = true;
            }
        }

        public static List<DetectedTargetedSpellData> DetectedTargetedSpells = new List<DetectedTargetedSpellData>();

        private static void Game_OnTick(EventArgs args)
        {
            DetectedTargetedSpells.RemoveAll(s => s.Ended);

            foreach (var spell in DetectedTargetedSpells)
                OnTargetedSpellUpdate.Invoke(spell);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null)
                return;

            DetectedTargetedSpells.RemoveAll(a => a.Caster != null && a.Caster.IdEquals(caster) && a.Missile != null && a.Missile.Equals(missile) && (a.Ended = true));
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                var xdata = TargetedSpellDatabase.Current.FirstOrDefault(s => s.MissileNames != null && s.MissileNames.Any(m => m.Equals(particle.Name)));
                if (xdata != null)
                {
                    var xcaster = EntityManager.Heroes.AllHeroes.OrderBy(o => o.Distance(particle)).FirstOrDefault(h => h.Hero.Equals(xdata.hero) && h.IsInRange(particle, 1000));
                    if (xcaster != null)
                    {
                        var xtarget = EntityManager.Heroes.AllHeroes.OrderBy(o => o.Distance(particle)).FirstOrDefault(h => h.Team != xcaster.Team && h.IsInRange(particle, 1000) && !h.IsDead && h.IsValid);
                        if (xtarget != null)
                        {
                            if (xdata.hero == Champion.Kled)
                            {
                                xdata.Speed = xcaster.MoveSpeed;
                            }

                            Add(new DetectedTargetedSpellData
                            {
                                Caster = xcaster,
                                Target = xtarget,
                                Data = xdata,
                                Start = xcaster.ServerPosition,
                                StartTick = Core.GameTickCount
                            });
                            return;
                        }
                    }
                }
            }

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null)
                return;

            var target = missile.Target as Obj_AI_Base;
            if (target == null)
                return;

            var data =
                TargetedSpellDatabase.Current.FirstOrDefault(
                    s => s.MissileNames != null && s.MissileNames.Any(m => m.Equals(missile.SData.Name, StringComparison.CurrentCultureIgnoreCase)) && s.hero.Equals(caster.Hero));

            if (data == null)
                return;

            var detected = new DetectedTargetedSpellData
            {
                Caster = caster,
                Target = target,
                Data = data,
                Start = missile.StartPosition,
                StartTick = Core.GameTickCount,
                Missile = missile
            };

            Add(detected);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as Obj_AI_Base;

            if (caster == null || target == null || !caster.IsValid || !target.IsValid)
                return;

            var data = TargetedSpellDatabase.Current.FirstOrDefault(s => s.hero.Equals(caster.Hero) && s.slot.Equals(args.Slot));

            if (data == null)
            {
                //Console.WriteLine($"OnProcessTargetedSpell: {caster.ChampionName} - [{args.Slot}] - Not Detected");
                return;
            }

            var detected = new DetectedTargetedSpellData
            {
                Caster = caster,
                Target = target,
                Data = data,
                Start = args.Start,
                StartTick = Core.GameTickCount
            };

            Add(detected);
        }

        private static void Add(DetectedTargetedSpellData args)
        {
            if (args == null || DetectedTargetedSpells.Contains(args))
            {
                Console.WriteLine("Invalid DetectedTargetedSpellData");
                return;
            }

            if (args.Missile != null)
            {
                var detect = DetectedTargetedSpells.FirstOrDefault(s => s.Caster.IdEquals(args.Caster) && s.Missile == null && s.Data.Equals(args.Data));
                if (detect != null)
                {
                    DetectedTargetedSpells.Remove(detect);
                }
            }

            if (args.Caster.Hero == Champion.Nautilus && args.Data.slot == SpellSlot.R && args.Target != null)
            {
                args.Start = args.Caster.ServerPosition.Extend(args.Target, args.Caster.GetAutoAttackRange(args.Target)).To3DWorld();
                if (args.Caster.Distance(args.Target) < args.Caster.GetAutoAttackRange(args.Target))
                    args.Speed = int.MaxValue;
            }

            OnTargetedSpellDetected.Invoke(args);
            DetectedTargetedSpells.Add(args);
        }
    }
}
