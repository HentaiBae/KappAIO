using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class TextureManager
    {
        public static bool Started;
        public static bool CanBeUsed => Started && DownloadTexture.Finished;
        public static void StartLoading()
        {
            if(Started)
                return;

            Started = true;
            DownloadTexture.Start();
            Game.OnTick += Game_OnTick;
        }

        public static void Reload()
        {
            Started = true;
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
