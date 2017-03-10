using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class TextureManager
    {
        public static void StartLoading()
        {
            DownloadTexture.Start();
            Game.OnTick += Game_OnTick;
        }

        public static void Reload()
        {
            LoadTexture.Dispose();
            StartLoading();
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (DownloadTexture.Finished)
            {
                Game.OnTick -= Game_OnTick;
                LoadTexture.Start();
            }
        }

        public static ChampionSprite GetSprite(this AIHeroClient target)
        {
            return LoadTexture.ChampionSprites.FirstOrDefault(t => t.Champion.IdEquals(target));
        }
    }
}
