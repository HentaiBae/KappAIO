using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class TextureManager
    {
        public static void Init()
        {
            LoadTexture.Init();
        }

        public static ChampionSprite GetSprite(this AIHeroClient champion)
        {
            return LoadTexture.ChampionSprites.FirstOrDefault(s => s.Hero.IdEquals(champion));
        }
    }
}
