using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Plugins.HUD;
using Newtonsoft.Json.Linq;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class TextureDownload
    {
        private static List<InGameSpells> inGameSpells = new List<InGameSpells>();
        private static System.Version currentVersion = GameVersion.CurrentPatch();
        private static Stopwatch stopWatch;
        private static int i;
        public static bool Finished;
        private static bool _downloadingAll;

        public static void Init()
        {
            var allHeros = EntityManager.Heroes.AllHeroes;
            inGameSpells.Clear();
            Finished = false;
            stopWatch = new Stopwatch();
            stopWatch.Start();
            
            if (HUDConfig.DownloadAllTexture)
            {
                _downloadingAll = true;

                foreach (var hero in allHeros) // download summ
                {
                    foreach (var slot in Extensions.KnownSpellSlots.Where(s => s.IsSummonerSpell()))
                    {
                        var spell = hero.Spellbook.GetSpell(slot);
                        var spellName = spell.SData.Name.Contains("SummonerSmite") ? "SummonerSmite" : spell.SData.Name;
                        var newSpell = new InGameSpell(slot, spellName);
                        Download(newSpell, hero.Hero);
                    }
                }

                bool skipped = true;
                var listAllGameChampions = typeof(Champion).GetEnumValues();
                foreach (var champion in listAllGameChampions)
                {
                    var championName = champion.ToString();
                    
                    if (championName.Contains("Unknown"))
                        continue;

                    if (Directory.Exists(FileManager.ChampionFolder((Champion)champion)))
                    {
                        if (Extensions.KnownSpellSlots.Where(s => !s.IsSummonerSpell()).All(s => File.Exists($"{FileManager.ChampionFolder((Champion)champion)}/{s}.png")))
                        {
                            continue;
                        }
                    }

                    skipped = false;

                    var spells = new List<InGameSpell>();
                    var webclient = new WebClient();
                    webclient.DownloadStringTaskAsync(new Uri($"http://ddragon.leagueoflegends.com/cdn/{GameVersion.CurrentPatch()}/data/en_US/champion/{championName}.json"));

                    if (championName.Equals("FiddleSticks")) // rito pls
                        championName = "Fiddlesticks";
                    
                    webclient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
                    {
                        if (args.Result.Contains(championName))
                        {
                            var jObject = JObject.Parse(args.Result);
                            var data = jObject.SelectToken("data");
                            var hero = data.SelectToken(championName);
                            dynamic heroSpells = hero.SelectToken("spells");
                            for (int j = 0; j <= 3; j++)
                            {
                                var spellSlot = Extensions.KnownSpellSlots[j];
                                var spellName = heroSpells[j].id.ToObject<string>();
                                var ingamespell = new InGameSpell(spellSlot, spellName);
                                spells.Add(ingamespell);
                                Console.WriteLine($"{ingamespell.Slot} {ingamespell.Name}");
                            }

                            inGameSpells.Add(new InGameSpells((Champion)champion, spells.ToArray()));
                            if (inGameSpells.Count >= listAllGameChampions.Length - 1)
                            {
                                foreach (var spell in inGameSpells)
                                {
                                    Download(spell);
                                }

                                Game.OnTick += Game_OnTick;
                            }
                        }

                        webclient.Dispose();
                    };
                }

                if (skipped) // all icons already updated
                {
                    Game.OnTick += Game_OnTick;
                }
            }
            else
            {
                foreach (var hero in allHeros)
                {
                    var spells = new List<InGameSpell>();
                    foreach (var slot in Extensions.KnownSpellSlots)
                    {
                        var spell = hero.Spellbook.GetSpell(slot);
                        var spellName = spell.SData.Name.Contains("SummonerSmite") ? "SummonerSmite" : spell.SData.Name;
                        var newSpell = new InGameSpell(slot, spellName);
                        if (!spells.Contains(newSpell))
                            spells.Add(newSpell);
                    }

                    var groupSpells = new InGameSpells(hero.Hero, spells.ToArray());
                    if (!inGameSpells.Contains(groupSpells))
                        inGameSpells.Add(groupSpells);
                }

                foreach (var spell in inGameSpells)
                {
                    Download(spell);
                }

                Game.OnTick += Game_OnTick;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (i == 0 && !Finished)
            {
                stopWatch.Stop();
                Finished = true;
                Logger.Info($"KappAIO: Finished download texture in {(stopWatch.ElapsedMilliseconds / 1000f).ToString("F1")} Seconds");
                Game.OnTick -= Game_OnTick;
            }
        }

        private static bool IsSummonerSpell(InGameSpell spell)
        {
            return spell.Slot.IsSummonerSpell();
        }

        private static void Download(InGameSpells spells)
        {
            Download(spells.Champion);
            foreach (var spell in spells.Spells)
            {
                Download(spell, spells.Champion);
            }
        }

        private static void Download(InGameSpell spell, Champion champ)
        {
            var abillityIconsURL = $"http://ddragon.leagueoflegends.com/cdn/{currentVersion}/img/spell";
            var currentAbillityURI = new Uri($"{abillityIconsURL}/{spell.Name}.png");

            if (IsSummonerSpell(spell))
            {
                Download($"{spell.Name}.png", FileManager.SummonerSpellsFolder, currentAbillityURI);
                return;
            }

            Download($"{spell.Slot}.png", FileManager.ChampionFolder(champ), currentAbillityURI);
        }

        private static void Download(Champion champion)
        {
            var championIconsURL = $"http://ddragon.leagueoflegends.com/cdn/{currentVersion}/img/champion";
            var currentChampionURI = new Uri($"{championIconsURL}/{champion}.png");

            Download($"{champion}.png", FileManager.ChampionFolder(champion), currentChampionURI);
        }

        private static void Download(string fileName, string folder, Uri uri)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (File.Exists($"{folder}/{fileName}"))
            {
                return;
            }

            var webClient = new WebClient();
            i++;
            webClient.DownloadFileAsync(uri, $"{folder}/{fileName}");
            Logger.Info($"Downloading {uri}");
            webClient.DownloadFileCompleted += (sender, args) =>
            {
                webClient.Dispose();
                i--;
            };
        }
    }
}
