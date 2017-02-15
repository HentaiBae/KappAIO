using EloBuddy;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Plugins.HUD;
using SharpDX;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class ChampionSprite
    {
        public ChampionSprite(AIHeroClient hero, Sprite xp, Sprite empty, Sprite hp, Sprite mp, LoadChampionSprite sprite, SpellSprite[] spells)
        {
            this.Hero = hero;
            this.HeroIcon = sprite.Icon;
            this.GrayIcon = sprite.Gray;
            this.SpellSprites = spells;
            this.Empty = empty;
            this.XP = xp;
            this.HP = hp;
            this.MP = mp;

            var barWidth = this.HeroIcon.Rectangle.Value.Width + spells[0].Ready.Rectangle.Value.Width;
            var barHeight = (int)(spells[0].Ready.Rectangle.Value.Height * (HUDConfig.BarSize * 0.01f));
            var rectangle = new Rectangle(0, 0, barWidth, barHeight / 3);

            empty.Rectangle = rectangle;
            xp.Rectangle = rectangle;
            hp.Rectangle = rectangle;
            mp.Rectangle = rectangle;

            this.Offset += barHeight;
            this.Offset += this.HeroIcon.Rectangle.Value.Height;
        }
        public AIHeroClient Hero;
        public Sprite HeroIcon;
        public Sprite GrayIcon;
        public Sprite Empty;
        public Sprite XP;
        public Sprite HP;
        public Sprite MP;
        public SpellSprite[] SpellSprites;
        public int Offset;

        public void Dispose()
        {
            this.HeroIcon.Texture.Dispose();
            this.GrayIcon.Texture.Dispose();
            this.Empty.Texture.Dispose();
            this.Empty.Texture.Dispose();
            this.MP.Texture.Dispose();
            foreach (var sprite in SpellSprites)
            {
                sprite.Ready.Texture.Dispose();
                sprite.NotReady.Texture.Dispose();
            }
        }
    }

    public class SpellSprite
    {
        public SpellSprite(SpellSlot slot, LoadSpellSprite spellSprite)
        {
            this.Slot = slot;
            this.Ready = spellSprite.Ready;
            this.NotReady = spellSprite.NotReady;
        }
        public SpellSlot Slot;
        public Sprite Ready;
        public Sprite NotReady;
        public void Dispose()
        {
            this.Ready.Texture.Dispose();
            this.NotReady.Texture.Dispose();
        }
    }

    public class LoadChampionSprite
    {
        public LoadChampionSprite(Champion champ, params Sprite[] sprites)
        {
            this.Champion = champ;
            this.Icon = sprites[0];
            this.Gray = sprites[1];
        }
        public Champion Champion;
        public Sprite Icon;
        public Sprite Gray;

        public void Dispose()
        {
            this.Icon.Texture.Dispose();
            this.Gray.Texture.Dispose();
        }
    }

    public class LoadSpellSprite
    {
        public LoadSpellSprite(string name, params Sprite[] sprites)
        {
            this.Name = name;
            this.Ready = sprites[0];
            this.NotReady = sprites[1];
        }
        public string Name;
        public Sprite Ready;
        public Sprite NotReady;
        public void Dispose()
        {
            this.Ready.Texture.Dispose();
            this.NotReady.Texture.Dispose();
        }
    }
}
