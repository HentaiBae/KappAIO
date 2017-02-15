using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Plugins.HUD;
using Rectangle = SharpDX.Rectangle;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class LoadTexture
    {
        private static SpellSlot[] wantedSlots = { SpellSlot.R, SpellSlot.Summoner1, SpellSlot.Summoner2 };
        private static string _textureName;
        private static TextureLoader _textureLoader = new TextureLoader();
        private static List<LoadChampionSprite> loadedChampionSprites = new List<LoadChampionSprite>();
        private static List<LoadSpellSprite> loadedSpellSprites = new List<LoadSpellSprite>();
        public static List<ChampionSprite> ChampionSprites = new List<ChampionSprite>();

        public static void Init()
        {
            TextureDownload.Init();
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(System.EventArgs args)
        {
            if (TextureDownload.Finished)
            {
                startLoading();
                Game.OnTick -= Game_OnTick;
            }
        }

        private static void startLoading()
        {
            _textureLoader.Dispose();
            _textureLoader = new TextureLoader();

            foreach (var sprite in loadedChampionSprites)
                sprite.Dispose();

            foreach (var sprite in loadedSpellSprites)
                sprite.Dispose();

            foreach (var sprite in ChampionSprites)
                sprite.Dispose();

            loadedChampionSprites.Clear();
            loadedSpellSprites.Clear();
            ChampionSprites.Clear();

            var hp = new Sprite(_textureLoader.Load("HP", Properties.Resources.hp));
            var empty = new Sprite(_textureLoader.Load("Empty", Properties.Resources.empty));
            var xp = new Sprite(_textureLoader.Load("XP", Properties.Resources.xp));
            var mp = new Sprite(_textureLoader.Load("MP", Properties.Resources.mp));
            var AllHeroes = EntityManager.Heroes.AllHeroes;
            foreach (var hero in EntityManager.Heroes.AllHeroes)
            {
                foreach (var slot in wantedSlots)
                {
                    var spellName = hero.Spellbook.GetSpell(slot).SData.Name.Contains("SummonerSmite") ? "SummonerSmite" : hero.Spellbook.GetSpell(slot).SData.Name;
                    if (!loadedSpellSprites.Any(s => s.Name.Equals(spellName)))
                    {
                        var sprite = loadSprite(hero.Hero, slot, spellName);
                        var newsprite = new LoadSpellSprite(spellName, sprite, sprite);
                        loadedSpellSprites.Add(newsprite);
                    }
                }

                if (!loadedChampionSprites.Any(s => s.Champion.Equals(hero.Hero)))
                {
                    var loadChampSprite = loadSprite(hero.Hero);
                    var newsprite =  new LoadChampionSprite(hero.Hero, loadChampSprite, loadChampSprite);
                    loadedChampionSprites.Add(newsprite);
                }
            }

            foreach (var hero in AllHeroes)
            {
                var allSpells = loadedSpellSprites.FindAll(s => hero.Spellbook.Spells.Any(x => s.Name.Equals(x.SData.Name) || (s.Name.Contains("SummonerSmite") && x.Name.Contains("SummonerSmite"))));
                var convertSpells = new List<SpellSprite>();
                foreach (var spell in allSpells)
                {
                    var correctSpell = hero.Spellbook.Spells.FirstOrDefault(s => s.Name.Equals(spell.Name) || (s.Name.Contains("SummonerSmite") && spell.Name.Contains("SummonerSmite")));
                    if (correctSpell != null)
                    {
                        var spellSprite = new SpellSprite(correctSpell.Slot, spell);
                        if(!convertSpells.Contains(spellSprite))
                            convertSpells.Add(spellSprite);
                    }
                }

                var icon = loadedChampionSprites.FirstOrDefault(s => s.Champion.Equals(hero.Hero));
                if (icon != null)
                {
                    var championSprite = new ChampionSprite(hero, xp, empty, hp, mp, icon, convertSpells.ToArray());
                    if (!ChampionSprites.Contains(championSprite))
                        ChampionSprites.Add(championSprite);
                }
            }

            Logger.Info("KappAIO: Loaded all texture");
        }

        private static Sprite loadSprite(Champion champ, SpellSlot slot, string name)
        {
            var folder = slot.IsSummonerSpell() ? FileManager.SummonerSpellsFolder : FileManager.ChampionFolder(champ);
            var filePath = $"{folder}/{(slot.IsSummonerSpell() ? name : slot.ToString())}.png";
            var image = Image.FromFile(filePath);
            var bitmap = resizeImage(image, true);
            var texture = _textureLoader.Load(bitmap, out _textureName);
            var sprite = new Sprite(texture);
            sprite.Rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            return sprite;
        }
        private static Sprite loadSprite(Champion champ)
        {
            var folder = FileManager.ChampionFolder(champ);
            var filePath = $"{folder}/{champ}.png";
            var image = Image.FromFile(filePath);
            var bitmap = resizeImage(image);
            string textureName;
            var texture = _textureLoader.Load(bitmap, out textureName);
            var sprite = new Sprite(texture);
            sprite.Rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            return sprite;
        }

        private static Bitmap resizeImage(Image img, bool spell = false)
        {
            var mod = HUDConfig.IconsSize * 0.01f;
            var mod2 = spell ? 0.625f : 1;
            var size = new Size((int)(img.Width * mod * mod2), (int)(img.Height * mod * mod2));
            return new Bitmap(img, size);
        }
    }
}
