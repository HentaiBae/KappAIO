using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Common.Utility.TextureManager;
using SharpDX;
using Color = SharpDX.Color;

namespace KappAIO_Reborn.Plugins.Utility.HUD
{
    public static class HUDManager
    {
        private static Text CDText;

        public static void Init()
        {
            HUDConfig.Init();
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
                    if (sprite?.Icon.Rectangle != null)
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
                    if (sprite?.Icon.Rectangle != null)
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
                if (!DownloadTexture.Finished || sprite == null)
                    return;

                sprite.Offset = 0;

                var vector2 = new Vector2(x, y + Yoffset);
                sprite.CurrentIcon.Draw(vector2);

                var heroIconHeight = 0;
                if(sprite.Icon.Rectangle.HasValue)
                    heroIconHeight = sprite.Icon.Rectangle.Value.Height;

                var heroIconWidth = 0;
                if (sprite.Icon.Rectangle.HasValue)
                    heroIconWidth = sprite.Icon.Rectangle.Value.Width;

                var spellIconHeight = 0;
                var rndspell = sprite.SpellSprites.FirstOrDefault();
                if (rndspell?.Icon.Rectangle != null)
                    spellIconHeight = rndspell.Icon.Rectangle.Value.Height;

                var barIconHeight = 0;
                if (sprite.XPBar.Rectangle.HasValue)
                    barIconHeight = sprite.XPBar.Rectangle.Value.Height;

                var barVector = vector2;
                barVector.Y += heroIconHeight;
                sprite.Offset += heroIconHeight;

                if (HUDConfig.DrawXP)
                {
                    sprite.Offset += barIconHeight;
                    var xp = Math.Min(Math.Max(0, sprite.Champion.CurrentXPPercent()), 100);
                    sprite.XPBar.Scale = new Vector2(1 * (xp / 100), 1);
                    sprite.EmptyBar.Draw(barVector);
                    sprite.XPBar.Draw(barVector);
                }
                else
                {
                    barVector.Y -= barIconHeight;
                }

                if (HUDConfig.DrawHP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var hp = Math.Min(Math.Max(0, sprite.Champion.HealthPercent), 100);
                    sprite.HPBar.Scale = new Vector2(1 * (hp / 100), 1);
                    sprite.EmptyBar.Draw(barVector);
                    sprite.HPBar.Draw(barVector);
                }

                if (HUDConfig.DrawMP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var mp = Math.Min(Math.Max(0, sprite.Champion.ManaPercent), 100);
                    sprite.HPBar.Scale = new Vector2(1 * (mp / 100), 1);
                    sprite.EmptyBar.Draw(barVector);
                    sprite.MPBar.Draw(barVector);
                }

                vector2.X += heroIconWidth;
                foreach (var spell in sprite.SpellSprites.OrderBy(s => s.Slot))
                {
                    if (spell.IsOnCoolDown(sprite.Champion))
                    {
                        var cdtextsize = spell.Icon.Rectangle.Value.Height + spell.Icon.Rectangle.Value.Width;

                        if (CDText == null)
                            CDText = new Text("", new Font(FontFamily.GenericSerif, cdtextsize * 0.23f, FontStyle.Regular)) { Color = System.Drawing.Color.AliceBlue };

                        var cdPos = new Vector2(vector2.X + spell.Icon.Rectangle.Value.Width * 1.3f, vector2.Y);
                        CDText.Draw(spell.CurrentCD(sprite.Champion), System.Drawing.Color.AliceBlue, cdPos);
                    }

                    spell.CurrentSprite(sprite.Champion).Draw(vector2);
                    vector2.Y += spellIconHeight;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("D3DERR_INVALIDCALL"))
                {
                    TextureManager.Reload();
                }
            }
        }
    }
}
