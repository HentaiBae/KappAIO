using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using KappAIO_Reborn.Common.Utility;
using SharpDX;

namespace KappAIO_Reborn.Common.CustomEvents
{
    public class TrackedTeleport
    {
        public TrackedTeleport(AIHeroClient caster, Teleport.TeleportEventArgs args)
        {
            this.Caster = caster;
            this.Args = args;
            this.StartTick = args.Start;
            this.EndTick = this.StartTick + args.Duration;
        }

        public AIHeroClient Caster;
        public GameObject TeleportTarget;
        public Teleport.TeleportEventArgs Args;
        private Vector3? _endPosition;
        public Vector3? EndPosition
        {
            get
            {
                Vector3? endResult = _endPosition;

                if (this.Args.Type == TeleportType.Recall)
                {
                    return this.Caster.GetSpawnPoint().Position;
                }

                var unitBase = ObjectManager.Get<Obj_HQ>().FirstOrDefault(o => o.Team == this.Caster.Team);
                if (this._endPosition.HasValue && this.Args.Type == TeleportType.Teleport && unitBase != null)
                {
                    var range = TeleportTarget != null ? TeleportTarget.Name.ToLower().Contains("turret") ? 225 
                        : Math.Min(175, this.TeleportTarget.BoundingRadius + this.Caster.BoundingRadius) 
                        : Math.Min(this.Caster.BoundingRadius, 100);

                    var fixedRange = Math.Min(275, range < 0 || range > 1000 ? 225
                        : range);

                    endResult = this._endPosition.Value.Extend(unitBase.Position, fixedRange).To3DWorld();
                }

                return endResult;
            }
            set
            {
                this._endPosition = value;
            }
        }
        public float StartTick;
        public float EndTick;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => this.Args.Start - Core.GameTickCount;
        public bool Ended => TicksLeft <= 0 || Aborted;
        public bool Aborted;
    }
    
    public class Teleports
    {
        public delegate void TeleportTracked(TrackedTeleport args);
        public static event TeleportTracked OnTrack;
        internal static void Invoke(TrackedTeleport args)
        {
            var invocationList = OnTrack?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);
        }
        public delegate void TeleportFinish(TrackedTeleport args);
        public static event TeleportFinish OnFinish;
        internal static bool InvokeFinish(TrackedTeleport args)
        {
            var invocationList = OnFinish?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            return true;
        }

        public static List<TrackedTeleport> TrackedTeleports = new List<TrackedTeleport>();

        private static string[] _teleportTargetBuffs = { "Teleport_Target", "teleport_turret" };
        private static string[] _alliedNames = { "_blue.troy", "_Green.troy" };
        private static Dictionary<string, Champion> _teleports = new Dictionary<string, Champion>
            {
            { "global_ss_teleport_target_", Champion.Unknown },
            { "global_ss_teleport_turret_", Champion.Unknown },
            { "TwistedFate_Base_R_Gatemarker_", Champion.TwistedFate },
            };

        static Teleports()
        {
            Teleport.OnTeleport += Teleport_OnTeleport;
            Game.OnTick += Game_OnTick;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
        }

        private static void Game_OnTick(EventArgs args)
        {
            TrackedTeleports.RemoveAll(t => t.Ended && InvokeFinish(t));

            foreach (var teleport in TrackedTeleports.Where(t => t.TeleportTarget == null))
            {
                var findBuffTarget = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => _teleportTargetBuffs.Any(o.HasBuff));
                if (findBuffTarget != null)
                {
                    teleport.EndPosition = findBuffTarget.ServerPosition;
                    teleport.TeleportTarget = findBuffTarget;
                    Invoke(teleport);
                }
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if(sender == null)
                return;

            if(!_teleportTargetBuffs.Any(b => args.Buff.DisplayName.Equals(b, StringComparison.CurrentCultureIgnoreCase) || args.Buff.Name.Equals(b, StringComparison.CurrentCultureIgnoreCase)))
                return;

            var tracked = TrackedTeleports.OrderByDescending(t => t.StartTick).FirstOrDefault(t => t.Caster.IdEquals(args.Buff.Caster));
            if (tracked != null)
            {
                tracked.EndPosition = sender.ServerPosition;
                tracked.TeleportTarget = sender;
                Invoke(tracked);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if(sender == null)
                return;

            var validTeleportTarget = sender.Name.EndsWith(".troy") && _teleports.Any(t => sender.Name.Contains(t.Key));
            if (validTeleportTarget)
            {
                var data = _teleports.FirstOrDefault(t => sender.Name.Contains(t.Key));
                var allied = _alliedNames.Any(sender.Name.EndsWith);

                var tracked = TrackedTeleports.OrderByDescending(t => t.StartTick).FirstOrDefault(t => t.EndPosition == null
                && (data.Value == Champion.Unknown || data.Value == t.Caster.Hero)
                && ((allied && t.Caster.IsAlly) || (!allied && t.Caster.IsEnemy)));

                if (tracked != null)
                {
                    tracked.EndPosition = sender.Position;
                    Invoke(tracked);
                }
            }
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if(hero == null)
                return;

            var tracked = TrackedTeleports.FirstOrDefault(t => t.Caster.IdEquals(hero));
            if (tracked != null)
            {
                if (args.Status == TeleportStatus.Abort || args.Status == TeleportStatus.Finish || args.Status == TeleportStatus.Unknown)
                    tracked.Aborted = true;
                else
                {
                    tracked.Args = args;
                }

                Invoke(tracked);
            }
            else
            {
                tracked = new TrackedTeleport(hero, args);
                TrackedTeleports.Add(tracked);
                Invoke(tracked);
            }
        }
    }
}
