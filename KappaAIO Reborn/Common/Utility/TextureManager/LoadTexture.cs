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
        public CachedTexture(string name, Texture texture, Bitmap bitmap)
        {
            this.TextureName = name;
            this.Texture = texture;
            this.Bitmap = bitmap;
        }
        public string TextureName;
        public Texture Texture;
        public Bitmap Bitmap;
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
            var hp = new Sprite(_textureLoader.Load("HP", Properties.Resources.hp));
            var empty = new Sprite(_textureLoader.Load("Empty", Properties.Resources.empty));
            var xp = new Sprite(_textureLoader.Load("XP", Properties.Resources.xp));
            var mp = new Sprite(_textureLoader.Load("MP", Properties.Resources.mp));

            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(h => h.ChampionName != "PracticeTool_TargetDummy"))
            {
                var heroIcon = loadSprite(hero.GetChampionName());
                var heroIconGray = loadSprite(hero.GetChampionName(), true);
                var spells = new List<SpellSprite>();
                foreach (var slot in wantedSlots)
                {
                    var spell = hero.Spellbook.GetSpell(slot);
                    var sprite = loadSprite(hero.GetChampionName(), slot, spell.GetSpellName());
                    var spriteGray = loadSprite(hero.GetChampionName(), slot, spell.GetSpellName(), true);
                    var spellSprite = new SpellSprite(spell.GetSpellName(), spell.Slot, sprite, spriteGray);
                    spells.Add(spellSprite);
                }

                var championSprite = new ChampionSprite(hero, heroIcon, heroIconGray, hp, mp, xp, empty, spells.ToArray());
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
                var cacheName = $"{filePath}{(gray ? "gray" : "")}";
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
                    _cacheTexture.Add(new CachedTexture(cacheName, texture, bitmap));
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

        private static Sprite loadSprite(string champ, bool gray = false)
        {
            try
            {
                var folder = FileManager.ChampionFolder(champ);
                var filePath = $"{folder}/{champ}.png";
                var cacheName = $"{filePath}{(gray ? "gray" : "")}";
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
                    bitmap = resizeImage(image);

                    if (gray)
                        bitmap = ReColor(bitmap);

                    texture = _textureLoader.Load(bitmap, out _textureName);
                    _cacheTexture.Add(new CachedTexture(cacheName, texture, bitmap));
                }

                var sprite = new Sprite(texture);
                sprite.Rectangle = new SharpDX.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                return sprite;
            }
            catch (Exception ex)
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

        /*
        public static Bitmap ColorEdge(Bitmap bi, Color c)
        {
            using (var grf = Graphics.FromImage(bi))
            {
                using (Brush border = new SolidBrush(c))
                {
                    grf.FillRectangle(border, 0, 0, 2.5f, bi.Height);
                }
            }

            return bi;
        }*/

        private static Bitmap resizeImage(Image img, bool spell = false)
        {
            var mod = HUDConfig.IconsSize * 0.01f;
            var mod2 = spell ? 0.63f : 1;
            var size = new Size((int)(img.Width * mod * mod2), (int)(img.Height * mod * mod2));
            return new Bitmap(img, size);
        }
    }
}
