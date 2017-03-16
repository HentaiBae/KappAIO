using EloBuddy;
using KappAIO_Reborn.Plugins.Utility.HUD;
using SharpDX;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class ChampionSprite
    {
        public ChampionSprite(AIHeroClient champion, CustomSprite icon, CustomSprite grayIcon, CustomSprite hp, CustomSprite mp, CustomSprite xp, CustomSprite empty, params SpellSprite[] spells)
        {
            this.Champion = champion;
            this.Icon = icon;
            this.GrayIcon = grayIcon;
            this.HPBar = hp;
            this.MPBar = mp;
            this.XPBar = xp;
            this.EmptyBar = empty;
            this.SpellSprites = spells;

            var barWidth = this.Icon.Sprite.Rectangle.Value.Width + spells[0].Icon.Sprite.Rectangle.Value.Width;
            var barHeight = (int)(spells[0].Icon.Sprite.Rectangle.Value.Height * (HUDConfig.BarSize * 0.01f));
            var rectangle = new Rectangle(0, 0, barWidth, barHeight / 3);

            hp.Sprite.Rectangle = rectangle;
            mp.Sprite.Rectangle = rectangle;
            xp.Sprite.Rectangle = rectangle;
            empty.Sprite.Rectangle = rectangle;

            this.Offset += barHeight;
            this.Offset += this.Icon.Sprite.Rectangle.Value.Height;
        }
        
        public AIHeroClient Champion;
        public CustomSprite Icon;
        public CustomSprite GrayIcon;
        public CustomSprite CurrentIcon => !this.Champion.IsHPBarRendered || this.Champion.IsDead ? this.GrayIcon : this.Icon;
        public CustomSprite HPBar;
        public CustomSprite MPBar;
        public CustomSprite XPBar;
        public CustomSprite EmptyBar;
        public SpellSprite[] SpellSprites;

        public int Offset;

        public void Dispose()
        {
            this.Icon.Texture.Texture.Dispose();
            this.GrayIcon.Texture.Texture.Dispose();
            this.HPBar.Texture.Texture.Dispose();
            this.MPBar.Texture.Texture.Dispose();
            this.XPBar.Texture.Texture.Dispose();
            this.EmptyBar.Texture.Texture.Dispose();

            foreach (var sprite in this.SpellSprites)
                sprite.Dispose();
        }
    }

    public class SpellSprite
    {
        public SpellSprite(string name, SpellSlot slot, CustomSprite icon, CustomSprite grayicon)
        {
            this.SpellName = name;
            this.Slot = slot;
            this.Icon = icon;
            this.GrayIcon = grayicon;
        }

        public string SpellName;
        public SpellSlot Slot;
        public CustomSprite Icon;
        public CustomSprite GrayIcon;

        public Sprite CurrentSprite(AIHeroClient hero)
        {
            var spell = hero.Spellbook.GetSpell(this.Slot);
            var notReady = hero.IsOnCoolDown(this.Slot) || !spell.IsLearned;
            return notReady ? this.GrayIcon.Sprite : this.Icon.Sprite;
        }

        public bool IsOnCoolDown(AIHeroClient target)
        {
            return target.IsOnCoolDown(this.Slot);
        }

        public string CurrentCD(AIHeroClient hero)
        {
            return hero.Spellbook.GetSpell(this.Slot).CoolDown();
        }

        public void Dispose()
        {
            this.Icon.Texture.Texture.Dispose();
            this.GrayIcon.Texture.Texture.Dispose();
        }
    }

    public class CustomSprite
    {
        public CustomSprite(Sprite sprite, CachedTexture texture)
        {
            this.Sprite = sprite;
            this.Texture = texture;
        }
        public Sprite Sprite;
        public CachedTexture Texture;
    }
}
