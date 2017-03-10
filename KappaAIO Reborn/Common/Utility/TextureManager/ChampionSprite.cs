using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Plugins.Utility.HUD;
using SharpDX;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class ChampionSprite
    {
        public ChampionSprite(AIHeroClient champion, Sprite icon, Sprite grayIcon, Sprite hp, Sprite mp, Sprite xp, Sprite empty, params SpellSprite[] spells)
        {
            this.Champion = champion;
            this.Icon = icon;
            this.GrayIcon = grayIcon;
            this.HPBar = hp;
            this.MPBar = mp;
            this.XPBar = xp;
            this.EmptyBar = empty;
            this.SpellSprites = spells;

            var barWidth = this.Icon.Rectangle.Value.Width + spells[0].Icon.Rectangle.Value.Width;
            var barHeight = (int)(spells[0].Icon.Rectangle.Value.Height * (HUDConfig.BarSize * 0.01f));
            var rectangle = new Rectangle(0, 0, barWidth, barHeight / 3);

            hp.Rectangle = rectangle;
            mp.Rectangle = rectangle;
            xp.Rectangle = rectangle;
            empty.Rectangle = rectangle;

            this.Offset += barHeight;
            this.Offset += this.Icon.Rectangle.Value.Height;
        }

        public AIHeroClient Champion;
        public Sprite Icon;
        public Sprite GrayIcon;
        public Sprite CurrentIcon => !this.Champion.IsHPBarRendered || this.Champion.IsDead ? this.GrayIcon : this.Icon;
        public Sprite HPBar;
        public Sprite MPBar;
        public Sprite XPBar;
        public Sprite EmptyBar;
        public SpellSprite[] SpellSprites;

        public int Offset;

        public void Dispose()
        {
            this.Icon.Texture.Dispose();
            this.GrayIcon.Texture.Dispose();
            this.HPBar.Texture.Dispose();
            this.MPBar.Texture.Dispose();
            this.XPBar.Texture.Dispose();
            this.EmptyBar.Texture.Dispose();

            foreach (var sprite in this.SpellSprites)
                sprite.Dispose();
        }
    }

    public class SpellSprite
    {
        public SpellSprite(string name, SpellSlot slot, Sprite icon, Sprite grayicon)
        {
            this.SpellName = name;
            this.Slot = slot;
            this.Icon = icon;
            this.GrayIcon = grayicon;
        }

        public string SpellName;
        public SpellSlot Slot;
        public Sprite Icon;
        public Sprite GrayIcon;

        public Sprite CurrentSprite(AIHeroClient hero)
        {
            var spell = hero.Spellbook.GetSpell(this.Slot);
            var notReady = hero.IsOnCoolDown(this.Slot) || !spell.IsLearned;
            return notReady ? this.GrayIcon : this.Icon;
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
            this.Icon.Texture.Dispose();
            this.GrayIcon.Texture.Dispose();
        }
    }
}
