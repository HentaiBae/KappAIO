using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Plugins.Utility.HUD;
using SharpDX.Direct3D9;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class CachedTexture
    {
        public CachedTexture(string name, Texture texture, Bitmap bitmap, Image image)
        {
            this.TextureName = name;
            this.Texture = texture;
            this.Bitmap = bitmap;
            this.Image = image;
        }
        public string TextureName;
        public Texture Texture;
        public Bitmap Bitmap;
        public Image Image;
    }

    public static class LoadTexture
    {
        private static SpellSlot[] wantedSlots = { SpellSlot.R, SpellSlot.Summoner1, SpellSlot.Summoner2 };
        private static string _textureName;
        private static TextureLoader _textureLoader = new TextureLoader();
        public static List<ChampionSprite> ChampionSprites = new List<ChampionSprite>();
        private static List<CachedTexture> _cacheTexture = new List<CachedTexture>();

        public static void Start()
        {
            if(!DownloadTexture.Finished)
                return;

            _textureLoader.Dispose();
            _textureLoader = new TextureLoader();
            
            ChampionSprites = loadChampionSprite();
        }

        public static void Dispose()
        {
            _textureLoader.Dispose();
            _textureLoader = new TextureLoader();
            foreach (var sprite in ChampionSprites)
            {
                sprite.Dispose();
            }

            foreach (var cache in _cacheTexture)
            {
                cache.Bitmap.Dispose();
                cache.Texture.Dispose();
            }

            ChampionSprites.Clear();
            _cacheTexture.Clear();
        }

        private static List<ChampionSprite> loadChampionSprite()
        {
            var result = new List<ChampionSprite>();
            var hpTexture = _textureLoader.Load("HP", Properties.Resources.hp);
            var hp = new Sprite(hpTexture);
            var mpTexture = _textureLoader.Load("MP", Properties.Resources.mp);
            var mp = new Sprite(mpTexture);
            var xpTexture = _textureLoader.Load("XP", Properties.Resources.xp);
            var xp = new Sprite(xpTexture);
            var emptyTexture = _textureLoader.Load("Empty", Properties.Resources.empty);
            var empty = new Sprite(emptyTexture);

            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(h => h.ChampionName != "PracticeTool_TargetDummy"))
            {
                var heroIcon = loadSprite(hero.GetChampionName());
                var heroIconGray = loadSprite(hero.GetChampionName(), true);
                var CircleheroIcon = loadSprite(hero.GetChampionName(), false, true, hero.IsAlly ? Color.GreenYellow : Color.Red);
                var spells = new List<SpellSprite>();
                foreach (var slot in wantedSlots)
                {
                    var spell = hero.Spellbook.GetSpell(slot);
                    var sprite = loadSprite(hero.GetChampionName(), slot, spell.GetSpellName());
                    var spriteGray = loadSprite(hero.GetChampionName(), slot, spell.GetSpellName(), true);
                    var spellSprite = new SpellSprite(spell.GetSpellName(), spell.Slot,
                        new CustomSprite(sprite, _cacheTexture.FirstOrDefault(t => t.TextureName.Equals($"{hero.GetChampionName()}{slot}"))),
                        new CustomSprite(spriteGray, _cacheTexture.FirstOrDefault(t => t.TextureName.Equals($"{hero.GetChampionName()}{slot}gray"))));
                    spells.Add(spellSprite);
                }

                var championSprite = new ChampionSprite(hero,
                    new CustomSprite(heroIcon, _cacheTexture.FirstOrDefault(t => t.TextureName.Equals($"{hero.GetChampionName()}"))),
                    new CustomSprite(heroIconGray, _cacheTexture.FirstOrDefault(t => t.TextureName.Equals($"{hero.GetChampionName()}gray"))),
                    new CustomSprite(CircleheroIcon, _cacheTexture.FirstOrDefault(t => t.TextureName.Equals($"{hero.GetChampionName()}circle"))),
                    new CustomSprite(hp, new CachedTexture("hp", hpTexture, new Bitmap(Properties.Resources.hp), Properties.Resources.hp)),
                    new CustomSprite(mp, new CachedTexture("mp", mpTexture, new Bitmap(Properties.Resources.mp), Properties.Resources.mp)),
                    new CustomSprite(xp, new CachedTexture("xp", xpTexture, new Bitmap(Properties.Resources.xp), Properties.Resources.xp)),
                    new CustomSprite(empty, new CachedTexture("empty", emptyTexture, new Bitmap(Properties.Resources.empty), Properties.Resources.empty)), spells.ToArray());

                result.Add(championSprite);
            }

            return result;
        }
        
        private static Sprite loadSprite(string champ, SpellSlot slot, string name, bool gray = false)
        {
            try
            {
                var folder = slot.IsSummonerSpell() ? FileManager.SummonerSpellsFolder : FileManager.ChampionFolder(champ);
                var filePath = $"{folder}/{(slot.IsSummonerSpell() ? name : slot.ToString())}.png";
                var cacheName = $"{champ}{slot}{(gray ? "gray" : "")}";
                Texture texture = null;
                Image image = null;
                Bitmap bitmap = null;
                var cached = _cacheTexture.FirstOrDefault(t => t.TextureName.Equals(cacheName));

                if (cached != null)
                {
                    texture = cached.Texture;
                    bitmap = cached.Bitmap;
                }
                else
                {
                    image = Image.FromFile(filePath);
                    bitmap = resizeImage(image, true);

                    if (gray)
                        bitmap = ReColor(bitmap);

                    texture = _textureLoader.Load(bitmap, out _textureName);
                    _cacheTexture.Add(new CachedTexture(cacheName, texture, bitmap, image));
                }

                var sprite = new Sprite(texture);
                sprite.Rectangle = new SharpDX.Rectangle(0, 0, bitmap.Width, bitmap.Height);

                return sprite;
            }
            catch (Exception)
            {
                Logger.Error($"KappAIO: Failed to load {champ} {slot} ({name}) Sprite");
            }

            return null;
        }

        private static Sprite loadSprite(string champ, bool gray = false, bool circle = false, Color criclecolor = new Color())
        {
            try
            {
                var folder = FileManager.ChampionFolder(champ);
                var filePath = $"{folder}/{champ}.png";
                var cacheName = $"{champ}{(gray ? "gray" : circle ? "circle" : "")}";
                Texture texture = null;
                Image image = null;
                Bitmap bitmap = null;
                var cached = _cacheTexture.FirstOrDefault(t => t.TextureName.Equals(cacheName));

                if (cached != null)
                {
                    texture = cached.Texture;
                    bitmap = cached.Bitmap;
                }
                else
                {
                    image = Image.FromFile(filePath);
                    var maxSize = Drawing.Width + Drawing.Height;
                    var customSize = (int)(maxSize * 0.0225);
                    bitmap = circle ? ellipse(resizeImage(image, false, customSize), true, criclecolor) : resizeImage(image);

                    if (gray)
                        bitmap = ReColor(bitmap);

                    texture = _textureLoader.Load(bitmap, out _textureName);
                    _cacheTexture.Add(new CachedTexture(cacheName, texture, bitmap, image));
                }

                var sprite = new Sprite(texture);
                sprite.Rectangle = new SharpDX.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                return sprite;
            }
            catch (Exception)
            {
                Logger.Error($"KappAIO: Failed to load {champ} Sprite");
            }

            return null;
        }

        public static Bitmap ReColor(Bitmap bi)
        {
            using (var grf = Graphics.FromImage(bi))
            {
                using (Brush brsh = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                {
                    grf.FillRectangle(brsh, new Rectangle(0, 0, bi.Width, bi.Height));
                }
            }

            using (var grf = Graphics.FromImage(bi))
            {
                using (Brush brsh = new TextureBrush(bi))
                {
                    grf.FillRectangle(brsh, 6, 6, bi.Width, bi.Height);
                }
            }

            using (var grf = Graphics.FromImage(bi))
            {
                grf.InterpolationMode = InterpolationMode.High;
                grf.CompositingQuality = CompositingQuality.HighQuality;
                grf.SmoothingMode = SmoothingMode.AntiAlias;
                grf.DrawImage(bi, new Rectangle(0, 0, bi.Width, bi.Height));
            }

            return bi;
        }
        
        public static Bitmap ellipse(Bitmap bi, bool c = false, Color color = new Color())
        {
            var btm = new Bitmap(bi.Width, bi.Height);
            var btm2 = new Bitmap(bi.Width + 5, bi.Height + 5);

            if (c)
            {
                using (var grf = Graphics.FromImage(btm))
                {
                    using (Brush brsh = new SolidBrush(color))
                    {
                        grf.FillEllipse(brsh, new Rectangle(0, 0, btm.Width, btm.Height));
                    }
                }
            }
            else
            {
                using (var grf = Graphics.FromImage(bi))
                {
                    using (Brush brsh = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
                    {
                        grf.FillRectangle(brsh, new Rectangle(0, 0, bi.Width, bi.Height));
                    }
                }
            }
            
            using (var grf = Graphics.FromImage(btm))
            {
                using (Brush brsh = new TextureBrush(bi))
                {
                    grf.FillEllipse(brsh, 5, 5, bi.Width - 10, bi.Height - 10);
                }
            }

            using (var grf = Graphics.FromImage(btm2))
            {
                grf.InterpolationMode = InterpolationMode.High;
                grf.CompositingQuality = CompositingQuality.HighQuality;
                grf.SmoothingMode = SmoothingMode.HighQuality;
                grf.DrawImage(btm, new Rectangle(0, 0, bi.Width, bi.Height));
            }

            return btm2;
        }

        private static Bitmap resizeImage(Image img, bool spell = false, int sizex = -1)
        {
            sizex = sizex == -1 ? HUDConfig.IconsSize : sizex;
            var mod = sizex * 0.01f;
            var mod2 = spell ? 0.627f : 1;
            var size = new Size((int)(img.Width * mod * mod2), (int)(img.Height * mod * mod2));
            return new Bitmap(img, size);
        }
    }
}
