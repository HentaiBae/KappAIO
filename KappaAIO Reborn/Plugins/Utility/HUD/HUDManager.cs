using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Common.CustomEvents;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Common.Utility.TextureManager;
using KappAIO_Reborn.Plugins.Utility.Tracker;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappAIO_Reborn.Plugins.Utility.HUD
{
    public static class HUDManager
    {
        private static Text CDText, RecallText, DeadText, LevelText;
        public static bool loadHud;

        public static void Init()
        {
            TextureManager.StartLoading();
            HUDConfig.Init();
            Drawing.OnEndScene += Drawing_OnEndScene;
            Unit.DeathEvent.OnDeath += DeathEvent_OnDeath;
        }

        private static Dictionary<int, Unit.OnDeathArgs> deadHeros = new Dictionary<int, Unit.OnDeathArgs>();
        private static void DeathEvent_OnDeath(Unit.OnDeathArgs args)
        {
            var hero = args.Sender as AIHeroClient;
            if(hero == null)
                return;

            var netid = hero.NetworkId;
            if (deadHeros.ContainsKey(netid))
            {
                deadHeros[netid] = args;
                return;
            }

            deadHeros.Add(netid, args);
        }

        public static float GetDeathDuration(this AIHeroClient hero)
        {
            var netid = hero.NetworkId;
            if (!deadHeros.ContainsKey(netid))
                return 0;

            return (deadHeros[hero.NetworkId].EndTick - Core.GameTickCount) / 1000f;
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
                    if (sprite?.Icon.Sprite.Rectangle != null)
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
                    if (sprite?.Icon.Sprite.Rectangle != null)
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
                var iconPos = vector2;
                sprite.CurrentIcon.Sprite.Draw(vector2);

                var heroIconHeight = 0;
                if(sprite.Icon.Sprite.Rectangle.HasValue)
                    heroIconHeight = sprite.Icon.Sprite.Rectangle.Value.Height;

                var heroIconWidth = 0;
                if (sprite.Icon.Sprite.Rectangle.HasValue)
                    heroIconWidth = sprite.Icon.Sprite.Rectangle.Value.Width;

                var spellIconHeight = 0;
                var rndspell = sprite.SpellSprites.FirstOrDefault();
                if (rndspell?.Icon.Sprite.Rectangle != null)
                    spellIconHeight = rndspell.Icon.Sprite.Rectangle.Value.Height;

                var barIconHeight = 0;
                if (sprite.XPBar.Sprite.Rectangle.HasValue)
                    barIconHeight = sprite.XPBar.Sprite.Rectangle.Value.Height;

                var barVector = vector2;
                barVector.Y += heroIconHeight;
                sprite.Offset += heroIconHeight;

                if (HUDConfig.DrawXP)
                {
                    sprite.Offset += barIconHeight;
                    var xp = Math.Min(Math.Max(0, sprite.Champion.CurrentXPPercent()), 100);
                    sprite.XPBar.Sprite.Scale = new Vector2(1 * (xp / 100), 1);
                    sprite.EmptyBar.Sprite.Draw(barVector);
                    sprite.XPBar.Sprite.Draw(barVector);
                }
                else
                {
                    barVector.Y -= barIconHeight;
                }

                if (HUDConfig.DrawHP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var hp = sprite.Champion.IsDead ? 0 : Math.Min(Math.Max(0, sprite.Champion.HealthPercent), 100);
                    sprite.HPBar.Sprite.Scale = new Vector2(1 * (hp / 100), 1);
                    sprite.EmptyBar.Sprite.Draw(barVector);
                    sprite.HPBar.Sprite.Draw(barVector);
                }

                if (HUDConfig.DrawMP)
                {
                    sprite.Offset += barIconHeight;
                    barVector.Y += barIconHeight;
                    var mp = sprite.Champion.IsDead ? 0 : Math.Min(Math.Max(0, sprite.Champion.ManaPercent), 100);
                    sprite.MPBar.Sprite.Scale = new Vector2(1 * (mp / 100), 1);
                    sprite.EmptyBar.Sprite.Draw(barVector);
                    sprite.MPBar.Sprite.Draw(barVector);
                }

                try
                {
                    if (HUDConfig.DrawRecall)
                    {
                        var tpInfo = sprite.Champion.GetTeleportInfo();
                        if (tpInfo != null && (tpInfo.Started || ((tpInfo.Aborted || tpInfo.Finished) && tpInfo.Duration > Core.GameTickCount - tpInfo.StartTick)))
                        {
                            if (RecallText == null)
                            {
                                var SIZE = sprite.Icon.Sprite.Rectangle.Value.Height + sprite.Icon.Sprite.Rectangle.Value.Width;
                                RecallText = new Text("Lato", new Font(FontFamily.GenericSerif, SIZE * 0.067f, FontStyle.Bold));
                            }

                            if (tpInfo.Args.Type == TeleportType.Unknown)
                                Logger.Warn($"KappAIO: Unknown TP type {tpInfo.Args.TeleportName} - Caster: {tpInfo.Caster.BaseSkinName} Duration: {tpInfo.Args.Duration}");

                            var c = new Color();
                            var color = ScaleColors.FirstOrDefault(s => s.Sprite.Equals(sprite));
                            if (color != null)
                            {
                                c = color.CurrentColor;
                            }
                            else
                            {
                                var scaleColor = new ScaleColor(sprite, Color.White, Color.Gold) { Duration = tpInfo.Duration };
                                ScaleColors.Add(scaleColor);
                                c = scaleColor.CurrentColor;
                            }

                            var text = tpInfo.Finished || tpInfo.Aborted ? tpInfo.Args.Status.ToString() : tpInfo.Name;
                            var modx = vector2.X * 1.01f;
                            var modY = (vector2.Y * 2 + sprite.Icon.Sprite.Rectangle.Value.Height) / 2;
                            var recallPos = new Vector2(modx, modY);
                            RecallText?.Draw(text.ToUpper(), c, recallPos);
                        }
                        else
                        {
                            ScaleColors.RemoveAll(s => s.Sprite.Equals(sprite));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                vector2.X += heroIconWidth;
                foreach (var spell in sprite.SpellSprites.OrderBy(s => s.Slot))
                {
                    if (spell.IsOnCoolDown(sprite.Champion))
                    {
                        if (CDText == null)
                        {
                            var cdtextsize = spell.Icon.Sprite.Rectangle.Value.Height + spell.Icon.Sprite.Rectangle.Value.Width;
                            CDText = new Text("", new Font(FontFamily.GenericSerif, cdtextsize * 0.22f, FontStyle.Regular)) { Color = Color.AliceBlue };
                        }

                        var cdPos = new Vector2(vector2.X + spell.Icon.Sprite.Rectangle.Value.Width * 1.3f, vector2.Y);
                        CDText.Draw(spell.CurrentCD(sprite.Champion), Color.AliceBlue, cdPos);
                    }

                    spell.CurrentSprite(sprite.Champion).Draw(vector2);
                    vector2.Y += spellIconHeight;
                }

                if (LevelText == null)
                {
                    var size = (heroIconHeight + heroIconWidth) * 0.1f;
                    LevelText = new Text("", new Font(FontFamily.GenericSerif, size, FontStyle.Bold));
                }
                var lvlpos = new Vector2(iconPos.X + heroIconWidth * 0.69f, iconPos.Y + heroIconHeight * 0.69f);
                LevelText.Draw(sprite.Champion.Level.ToString(), Color.Red, lvlpos);

                if (sprite.Champion.IsDead)
                {
                    if (DeadText == null)
                    {
                        var size = (heroIconHeight + heroIconWidth) * 0.125f;
                        DeadText = new Text("Arial", new Font(FontFamily.GenericSerif, size, FontStyle.Bold));
                    }
                    DeadText.Draw(sprite.Champion.GetDeathDuration().ToTimeSpan(), Color.Red, new Vector2(iconPos.X + heroIconWidth * 0.225f, iconPos.Y + heroIconHeight * 0.375f));
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

        private static List<ScaleColor> ScaleColors = new List<ScaleColor>();

        public class ScaleColor
        {
            public ScaleColor(ChampionSprite sprite, Color start, Color end)
            {
                this.Sprite = sprite;
                this.StartColor = start;
                this.EndColor = end;
                this.A = end.A - start.A;
                this.R = end.R - start.R;
                this.G = end.G - start.G;
                this.B = end.B - start.B;
            }
            public ChampionSprite Sprite;
            private Color StartColor;
            private Color EndColor;
            public Color CurrentColor
            {
                get
                {
                    var mod = this.TicksPassed/this.Duration;
                    var color = new SharpDX.Color
                    {
                        A = (byte)(Math.Max(0, Math.Min(255, this.StartColor.A + this.A * mod))),
                        R = (byte)(Math.Max(0, Math.Min(255, this.StartColor.R + this.R * mod))),
                        G = (byte)(Math.Max(0, Math.Min(255, this.StartColor.G + this.G * mod))),
                        B = (byte)(Math.Max(0, Math.Min(255, this.StartColor.B + this.B * mod))),
                    };
                    return color.ToSystem();
                }
            }
            private int A;
            private int R;
            private int G;
            private int B;
            private float StartTick = Core.GameTickCount;
            public float Duration;
            private float EndTick => this.StartTick + this.Duration;
            private float TicksPassed => Core.GameTickCount - this.StartTick;
        }
    }
}
