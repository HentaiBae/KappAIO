using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Common.CustomEvents;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Common.Utility.TextureManager;
using KappAIO_Reborn.Plugins.Utility.HUD;
using Color = SharpDX.Color;

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

        private static Text drawText;
        private static Menu menu;
        private static bool draw => menu.CheckBoxValue("draw");
        public static void Init()
        {
            HUDConfig.Init();
            if (!TextureManager.Started)
            {
                TextureManager.StartLoading();
            }
            drawText = new Text("Lato", new Font(FontFamily.GenericSerif, 14f, FontStyle.Bold));
            menu = Program.UtilityMenu.AddSubMenu("Teleports Tracker");
            menu.CreateCheckBox("draw", "Draw Teleprot End Point");
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(System.EventArgs args)
        {
            if(!draw || !TextureManager.CanBeUsed)
                return;

            foreach (var tp in TrackedTeleports.Where(t => t.Value.Position != null && !t.Value.Ended).Select(t => t.Value))
            {
                var end = tp.Position.Value;
                //end.DrawCircle(100, Color.AliceBlue);
                var c = tp.Caster.IsEnemy ? System.Drawing.Color.Red : System.Drawing.Color.GreenYellow;
                tp.Caster.GetSprite().CircleIcon.Draw(end.WorldToScreen());
                //drawText.Draw($"{tp.Caster.BaseSkinName} ({tp.Name}): {(tp.TicksLeft / 1000f).ToTimeSpan()}", c, end.WorldToScreen());
            }
        }
    }
}
