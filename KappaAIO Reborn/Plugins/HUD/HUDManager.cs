using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Common.Utility.TextureManager;
using SharpDX;

namespace KappAIO_Reborn.Plugins.HUD
{
    public static class HUDManager
    {
        public static void Init()
        {
            HUDConfig.Init();
            TextureManager.Init();
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static int allyYoffset;
        private static int enemyYoffset;
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (HUDConfig.DrawAlly)
            {
                allyYoffset = 0;
                foreach (var hero in EntityManager.Heroes.Allies.OrderBy(h => h.Hero))
                {
                    var sprite = hero.GetSprite();
                    if (sprite?.HeroIcon.Rectangle != null)
                    {
                        var xmod = HUDConfig.AllyX * 0.01f;
                        var ymod = HUDConfig.AllyY * 0.01f;
                        Draw(sprite, allyYoffset, (int)(Drawing.Width * xmod), (int)(Drawing.Height * ymod));
                        allyYoffset += sprite.Offset;
                        allyYoffset += HUDConfig.Spacing;
                    }
                }
            }

            if (HUDConfig.DrawEnemy)
            {
                enemyYoffset = 0;
                foreach (var hero in EntityManager.Heroes.Enemies.OrderBy(h => h.Hero))
                {
                    var sprite = hero.GetSprite();
                    if (sprite?.HeroIcon.Rectangle != null)
                    {
                        var xmod = HUDConfig.EnemyX * 0.01f;
                        var ymod = HUDConfig.EnemyY * 0.01f;
                        Draw(sprite, enemyYoffset, (int)(Drawing.Width * xmod), (int)(Drawing.Height * ymod));
                        enemyYoffset += sprite.Offset;
                        enemyYoffset += HUDConfig.Spacing;
                    }
                }
            }
        }

        private static void Draw(ChampionSprite sprite, int Yoffset, int x, int y)
        {
            try
            {
                if (!TextureDownload.Finished || sprite == null)
                    return;

                sprite.Offset = 0;

                var vector2 = new Vector2(x, y + Yoffset);
                sprite.HeroIcon.Draw(vector2);
                var heroIconHeight = 0;
                if(sprite.HeroIcon.Rectangle.HasValue)
                    heroIconHeight = sprite.HeroIcon.Rectangle.Value.Height;

                var heroIconWidth = 0;
                if (sprite.HeroIcon.Rectangle.HasValue)
                    heroIconWidth = sprite.HeroIcon.Rectangle.Value.Width;

                var spellIconHeight = 0;
                var rndspell = sprite.SpellSprites.FirstOrDefault();
                if (rndspell?.Ready.Rectangle != null)
                    spellIconHeight = rndspell.Ready.Rectangle.Value.Height;

                var barIconHeight = 0;
                if (sprite.XP.Rectangle.HasValue)
                    barIconHeight = sprite.XP.Rectangle.Value.Height;

                var barVector = vector2;
                barVector.Y += heroIconHeight;
                sprite.Offset += heroIconHeight;

                if (HUDConfig.DrawXP)
                {
                    sprite.Offset += barIconHeight;
                    var xp = Math.Min(Math.Max(0, sprite.Hero.CurrentXPPercent()), 100);
                    sprite.XP.Scale = new Vector2(1 * (xp / 100), 1);
                    sprite.Empty.Draw(barVector);
                    sprite.XP.Draw(barVector);
                }
                else
                {
                    barVector.Y -= barIconHeight;
                }

                if (HUDConfig.DrawHP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var hp = Math.Min(Math.Max(0, sprite.Hero.HealthPercent), 100);
                    sprite.HP.Scale = new Vector2(1 * (hp / 100), 1);
                    sprite.Empty.Draw(barVector);
                    sprite.HP.Draw(barVector);
                }

                if (HUDConfig.DrawMP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var mp = Math.Min(Math.Max(0, sprite.Hero.ManaPercent), 100);
                    sprite.MP.Scale = new Vector2(1 * (mp / 100), 1);
                    sprite.Empty.Draw(barVector);
                    sprite.MP.Draw(barVector);
                }

                vector2.X += heroIconWidth;
                foreach (var spell in sprite.SpellSprites.OrderBy(s => s.Slot))
                {
                    spell.Ready.Draw(vector2);
                    vector2.Y += spellIconHeight;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("D3DERR_INVALIDCALL"))
                {
                    LoadTexture.Init();
                }
            }
        }
    }
}
