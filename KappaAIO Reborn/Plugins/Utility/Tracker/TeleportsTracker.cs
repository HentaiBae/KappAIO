using System;
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
using SharpDX;
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
            menu.CreateCheckBox("draw", "Draw Teleport End Point");
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        
        private static Dictionary<Vector2, List<TrackedTeleport>> toDraw = new Dictionary<Vector2, List<TrackedTeleport>>();

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if(!draw || !TextureManager.CanBeUsed)
                return;

            toDraw.Clear();
            foreach (var tp in TrackedTeleports.Where(t => t.Value.Position != null && !t.Value.Ended).Select(t => t.Value))
            {
                var sprite = tp.Caster.GetSprite().CircleIcon;
                var end = tp.Position.Value.WorldToScreen();
                var x = Math.Min(Drawing.Width - sprite.Texture.Bitmap.Width, Math.Max(Drawing.Width * 0.05f, end.X)) * 0.95f;
                var y = Math.Min(Drawing.Height - sprite.Texture.Bitmap.Height, Math.Max(Drawing.Height * 0.05f, end.Y)) * 0.95f;
                
                var modpos = new Vector2(x, y);

                if (modpos.IsOnScreen())
                {
                    if (!toDraw.ContainsKey(modpos))
                    {
                        toDraw.Add(modpos, new List<TrackedTeleport> { tp });
                    }
                    //Console.WriteLine(modpos);
                    //sprite.Draw(modpos);
                }
            }

            foreach (var pos in toDraw)
            {
                if (pos.Key.IsOnScreen())
                {
                    var offset = 0;
                    foreach (var tp in pos.Value)
                    {
                        var sprite = tp.Caster.GetSprite().CircleIcon;
                        sprite.Draw(new Vector2(pos.Key.X, pos.Key.Y + offset));
                        offset += sprite.Texture.Bitmap.Width;
                    }
                }
            }
        }
    }
}
