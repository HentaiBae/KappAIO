using System.Collections.Generic;
using EloBuddy;
using KappAIO_Reborn.Common.CustomEvents;

namespace KappAIO_Reborn.Plugins.Utility.Tracker
{
    public static class TeleportsTracker
    {
        public static Dictionary<int, TrackedTeleport> TrackedTeleports = new Dictionary<int, TrackedTeleport>();

        private static bool loaded;
        static TeleportsTracker()
        {
            if(loaded)
                return;
            
            loaded = true;
            Teleports.OnTrack += Teleports_OnTrack;
        }
        private static void Teleports_OnTrack(TrackedTeleport args)
        {
            if (TrackedTeleports.ContainsKey(args.Caster.NetworkId))
            {
                TrackedTeleports[args.Caster.NetworkId] = args;
                return;
            }

            TrackedTeleports.Add(args.Caster.NetworkId, args);
        }

        public static TrackedTeleport GetTeleportInfo(this AIHeroClient target)
        {
            if (!TrackedTeleports.ContainsKey(target.NetworkId))
                return null;

            return TrackedTeleports[target.NetworkId];
        }
    }
}
