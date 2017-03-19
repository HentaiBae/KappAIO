using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.CustomEvents;
using SharpDX;

namespace KappAIO_Reborn.Plugins.Utility.Tracker
{
    public static class TeleportsTracker
    {
        public static List<TrackedTeleport> TrackedTeleports = new List<TrackedTeleport>();

        private static bool loaded;
        static TeleportsTracker()
        {
            if(loaded)
                return;
            
            loaded = true;
            Teleports.OnTrack += Teleports_OnTrack;
            Teleports.OnFinish += Teleports_OnFinish;
        }

        private static void Teleports_OnFinish(TrackedTeleport args)
        {
            if (TrackedTeleports.Contains(args))
            {
                TrackedTeleports.Remove(args);
            }
        }

        private static void Teleports_OnTrack(TrackedTeleport args)
        {
            if (args.EndPosition != null && !TrackedTeleports.Contains(args))
            {
                TrackedTeleports.Add(args);
            }
        }

        public static TrackedTeleport GetTeleportInfo(this AIHeroClient target)
        {
            return TrackedTeleports.FirstOrDefault(t => t.Caster.IdEquals(target));
        }
    }
}
