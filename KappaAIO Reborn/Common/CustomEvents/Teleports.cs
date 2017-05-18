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
        public Vector3? Position => this._endPosition;
        public string Name
        {
            get
            {
                switch (this.Args.Type)
                {
                    case TeleportType.TwistedFate:
                        return "Gate R";
                    case TeleportType.Shen:
                        return "Shen R";
                    case TeleportType.Teleport:
                        return "Teleport";
                    case TeleportType.Recall:
                        return "Recall";
                    default:
                        return "Unknown";
                }
            }
        }
        public float Duration;
        public float StartTick;
        public float EndTick => this.StartTick + this.Duration;
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => this.Args.Start - Core.GameTickCount;
        public bool Ended => TicksLeft <= 0 || Aborted || Finished;
        public bool Recalling => this.Args.Type == TeleportType.Recall && this.Started;
        public bool Teleporting => this.Args.Type == TeleportType.Teleport && this.Started;
        public bool Started => this.Args.Status == TeleportStatus.Start;
        public bool Finished => this.Args.Status == TeleportStatus.Finish;
        public bool Aborted => this.Args.Status == TeleportStatus.Abort;
        public bool IsTargeted => this.Args.Type == TeleportType.Shen || this.Args.Type == TeleportType.Teleport;
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

        public static Dictionary<int, TrackedTeleport> TrackedTeleports = new Dictionary<int, TrackedTeleport>();

        private static string[] _teleportTargetBuffs = { "Teleport_Target", "teleport_turret" };
        private static string[] _alliedNames = { "_blue.troy", "_Green.troy" };
        private static Dictionary<string, Champion> _teleports = new Dictionary<string, Champion>
            {
            { "global_ss_teleport_target_", Champion.Unknown }, // Unknown = all
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
            if (TrackedTeleports.Any())
            {
                foreach (var tp in TrackedTeleports.Where(t => t.Value.Ended && InvokeFinish(t.Value)))
                {
                    TrackedTeleports.Remove(tp.Key);
                    return;
                }

                foreach (var teleport in TrackedTeleports.Where(t => t.Value.TeleportTarget == null && t.Value.IsTargeted))
                {
                    var findBuffTarget = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => _teleportTargetBuffs.Any(o.HasBuff));
                    if (findBuffTarget != null)
                    {
                        teleport.Value.EndPosition = findBuffTarget.ServerPosition;
                        teleport.Value.TeleportTarget = findBuffTarget;
                        Invoke(teleport.Value);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender == null)
                return;

            if (!_teleportTargetBuffs.Any(b => args.Buff.DisplayName.Equals(b, StringComparison.CurrentCultureIgnoreCase) || args.Buff.Name.Equals(b, StringComparison.CurrentCultureIgnoreCase)))
                return;

            var tracked = TrackedTeleports.OrderByDescending(t => t.Value.StartTick).FirstOrDefault(t => t.Value.IsTargeted && t.Value.Caster.IdEquals(args.Buff.Caster));
            if (TrackedTeleports.Contains(tracked))
            {
                tracked.Value.EndPosition = sender.ServerPosition;
                tracked.Value.TeleportTarget = sender;
                Invoke(tracked.Value);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var validTeleportTarget = sender.Name.EndsWith(".troy") && _teleports.Any(t => sender.Name.Contains(t.Key));
            if (validTeleportTarget)
            {
                var data = _teleports.FirstOrDefault(t => sender.Name.Contains(t.Key));
                var allied = _alliedNames.Any(sender.Name.EndsWith);

                var tracked = TrackedTeleports.OrderByDescending(t => t.Value.StartTick).FirstOrDefault(t => t.Value.EndPosition == null
                && t.Value.Args.Type == TeleportType.Teleport
                && (data.Value == Champion.Unknown || data.Value == t.Value.Caster.Hero)
                && ((allied && t.Value.Caster.IsAlly) || (!allied && t.Value.Caster.IsEnemy)));

                if (TrackedTeleports.Contains(tracked))
                {
                    tracked.Value.EndPosition = sender.Position;
                    Invoke(tracked.Value);
                }
            }
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if(caster == null)
                return;

            var tpargs = new TrackedTeleport(caster, args);
            tpargs.Duration = args.Status == TeleportStatus.Finish || args.Status == TeleportStatus.Abort ? 2000 : args.Duration;
            tpargs.StartTick = Core.GameTickCount;
            if (args.Type == TeleportType.Recall)
                tpargs.EndPosition = caster.GetSpawnPoint().Position;
            if (TrackedTeleports.ContainsKey(caster.NetworkId))
            {
                TrackedTeleports[caster.NetworkId] = tpargs;
                Invoke(tpargs);
                return;
            }
            
            TrackedTeleports.Add(caster.NetworkId, tpargs);
            Invoke(tpargs);
        }
    }
}
