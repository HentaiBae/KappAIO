using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Utility;
using SharpDX;

namespace KappAIO_Reborn.Common.CustomEvents
{
    public static class Unit
    {
        public class OnPositionUpdate
        {
            public class UpdatedPosition
            {
                public UpdatedPosition(Obj_AI_Base x, Vector3 current, Vector3 before, Vector3 after, bool fromfow)
                {
                    this.unit = x;
                    this.Current = current;
                    this.Before = before;
                    this.After = after;
                    this.FromFOW = fromfow;
                }

                public Obj_AI_Base unit;
                public Vector3 Current;
                public Vector3 Before;
                public Vector3 After;
                public bool FromFOW;
            }

            public delegate void PositionUpdate(UpdatedPosition args);
            public static event PositionUpdate OnUpdate;
            internal static void Invoke(UpdatedPosition args)
            {
                var invocationList = OnUpdate?.GetInvocationList();
                if (invocationList != null)
                    foreach (var m in invocationList)
                        m?.DynamicInvoke(args);
            }

            private static List<UpdatedPosition> trackedUnits = new List<UpdatedPosition>();

            static OnPositionUpdate()
            {
                trackedUnits.AddRange(EntityManager.Heroes.AllHeroes.Select(h => new UpdatedPosition(h, h.ServerPosition, h.ServerPosition, h.ServerPosition, !h.IsHPBarRendered)));
                Game.OnTick += Game_OnTick;
                GameObject.OnCreate += GameObject_OnCreate;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
                Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
                GameObject.OnDelete += GameObject_OnDelete;
            }

            private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
            {
                var caster = args.Buff.Caster as AIHeroClient;
                if (caster != null)
                {
                    var tracked = trackedUnits.FirstOrDefault(u => u.unit.IdEquals(caster));
                    if (tracked != null)
                    {
                        Console.WriteLine($"{caster.ChampionName}: BuffLose {args.Buff.DisplayName}");
                        Invoke(new UpdatedPosition(caster, caster.ServerPosition, tracked.Current, sender.ServerPosition, !caster.IsHPBarRendered));
                    }
                }
            }

            private static void GameObject_OnDelete(GameObject sender, EventArgs args)
            {
                if (sender != null)
                {
                    var tracked = trackedUnits.FirstOrDefault(u => sender.Name.ToLower().Contains(u.unit.Model.ToLower()));
                    if (tracked != null)
                    {
                        Console.WriteLine($"{tracked.unit.BaseSkinName}: OnDelete {sender.Name}");
                        Invoke(new UpdatedPosition(tracked.unit, sender.Position, sender.Position, sender.Position, !tracked.unit.IsHPBarRendered && !EntityManager.Allies.Any(a => a.IsValidTarget() && a.IsInRange(sender, 600))));
                    }
                }
            }

            private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
            {
                var caster = args.Buff.Caster as AIHeroClient;
                if (caster != null)
                {
                    var tracked = trackedUnits.FirstOrDefault(u => u.unit.IdEquals(caster));
                    if (tracked != null)
                    {
                        Console.WriteLine($"{caster.ChampionName}: BuffGain {args.Buff.DisplayName}");
                        Invoke(new UpdatedPosition(caster, caster.ServerPosition, tracked.Current, sender.ServerPosition, !caster.IsHPBarRendered));
                    }
                }
            }

            private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                var caster = sender as AIHeroClient;
                if (caster == null)
                    return;

                var tracked = trackedUnits.FirstOrDefault(u => u.unit.IdEquals(caster));
                if (tracked != null)
                {
                    Console.WriteLine($"{caster.ChampionName}: ProcessSpellCast {args.Slot}");
                    Invoke(new UpdatedPosition(caster, caster.ServerPosition, tracked.Current, args.Start, !caster.IsHPBarRendered));
                }
            }

            private static void GameObject_OnCreate(GameObject sender, System.EventArgs args)
            {
                if (sender != null)
                {
                    var xtracked = trackedUnits.FirstOrDefault(u => sender.Name.ToLower().Contains(u.unit.Model.ToLower()));
                    if (xtracked != null)
                    {
                        Console.WriteLine($"{xtracked.unit.BaseSkinName}: OnDelete {sender.Name}");
                        Invoke(new UpdatedPosition(xtracked.unit, sender.Position, sender.Position, sender.Position, !xtracked.unit.IsHPBarRendered));
                    }
                }

                var missile = sender as MissileClient;
                var caster = missile?.SpellCaster as AIHeroClient;
                if(caster == null)
                    return;

                var tracked = trackedUnits.FirstOrDefault(u => u.unit.IdEquals(caster));
                if (tracked != null)
                {
                    Console.WriteLine($"{tracked.unit.BaseSkinName}: missileOnCreate {missile.SData.Name}");
                    Invoke(new UpdatedPosition(caster, caster.ServerPosition, tracked.Current, missile.StartPosition, !caster.IsHPBarRendered));
                }
            }

            private static void Game_OnTick(System.EventArgs args)
            {
                foreach (var unit in trackedUnits)
                {
                    if (!unit.Current.IsInRange(unit.unit.ServerPosition, unit.unit.BoundingRadius))
                    {
                        unit.Before = unit.Current;
                        unit.Current = unit.unit.ServerPosition;
                        unit.After = unit.unit.ServerPosition;
                        unit.FromFOW = !unit.unit.IsHPBarRendered;
                        Console.WriteLine($"{unit.unit.BaseSkinName}: OnTick");
                        Invoke(unit);
                    }
                }
            }
        }

        public class OnDeathArgs
        {
            public GameObject Sender;
            public float StartTick;
            public float EndTick;
            public float Duration;
        }

        public class DeathEvent
        {
            public delegate void Death(OnDeathArgs args);

            public static event Death OnDeath;
            private static readonly Dictionary<int, OnDeathArgs> invoked = new Dictionary<int, OnDeathArgs>();
            private static readonly bool loaded;

            static DeathEvent()
            {
                if (loaded)
                    return;
                loaded = true;
                Game.OnTick += delegate
                    {
                        foreach (var obj in ObjectManager.Get<GameObject>().Where(o => o.IsValid))
                        {
                            if (obj != null)
                            {
                                var netid = obj.NetworkId;
                                if (obj.IsDead)
                                {
                                    if (!invoked.ContainsKey(netid))
                                    {
                                        var args = new OnDeathArgs
                                            {
                                                Sender = obj, StartTick = Core.GameTickCount,
                                                EndTick = obj.IsChampion() ? ((Game.Time + ((AIHeroClient)obj).DeathDuration()) * 1000f) : int.MaxValue,
                                                Duration = obj.IsChampion() ? (((AIHeroClient)obj).DeathDuration() * 1000f) : int.MaxValue,
                                            };
                                        invoked.Add(netid, args);
                                        OnDeath?.Invoke(args);
                                    }
                                }
                                else
                                {
                                    if (invoked.ContainsKey(netid))
                                        invoked.Remove(netid);
                                }
                            }
                        }
                    };
            }
        }
    }
}
