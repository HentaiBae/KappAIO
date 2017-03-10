using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.CustomEvents;
using SharpDX;

namespace KappAIO_Reborn.Plugins.Utility.Tracker
{
    public static class TeleportsTracker
    {
        public static void Init()
        {
            Teleports.OnTrack += Teleports_OnTrack;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            end.DrawCircle(100, Color.AliceBlue);
        }

        private static Vector3 end;
        private static void Teleports_OnTrack(TrackedTeleport args)
        {
            if (args.EndPosition != null)
            {
                end = args.EndPosition.Value;
            }
        }
    }
}
