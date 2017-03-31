using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Utils;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public class ChampionSpell
    {
        public ChampionSpell(string champ, string spell, SpellSlot slot)
        {
            this.ChampionName = champ;
            this.SpellName = spell;
            this.Slot = slot;
        }
        public string ChampionName;
        public string SpellName;
        public SpellSlot Slot;
    }

    public static class DownloadTexture
    {
        public static bool Finished;

        public static List<ChampionSpell> ChampionSpells = new List<ChampionSpell>();
        private static List<string> _downloaded = new List<string>();

        private static Stopwatch stopwatch = new Stopwatch();
        public static void Start()
        {
            Finished = false;

            _downloaded.Clear();

            stopwatch.Reset();
            stopwatch.Start();
            ChampionSpells = LoadChampionSpells();

            var currentVersion = GameVersion.CurrentPatch();
            var abillityIconsURL = $"http://ddragon.leagueoflegends.com/cdn/{currentVersion}/img/spell";
            var championIconsURL = $"http://ddragon.leagueoflegends.com/cdn/{currentVersion}/img/champion";
            foreach (var spell in ChampionSpells)
            {
                Download(spell, championIconsURL, abillityIconsURL);
            }

            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (i == 0 && !Finished)
            {
                Game.OnTick -= Game_OnTick;
                stopwatch.Stop();
                Finished = true;
                Console.WriteLine($"Finished download in: {(stopwatch.ElapsedMilliseconds / 1000f).ToString("F1")} Seconds");
            }
        }

        private static int i;

        private static List<ChampionSpell> LoadChampionSpells()
        {
            var result = new List<ChampionSpell>();
            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(h => h.GetChampionName() != "PracticeTool_TargetDummy"))
            {
                foreach (var spell in hero.Spellbook.Spells.Where(x => Extensions.KnownSpellSlots.Contains(x.Slot)))
                {
                    var newSpell = new ChampionSpell(hero.GetChampionName(), spell.GetSpellName(), spell.Slot);
                    if(!result.Contains(newSpell))
                        result.Add(newSpell);
                }
            }

            return result;
        }
        
        private static void DownloadSummonerSpells(ChampionSpell spell, string url)
        {
            var currentAbillityURI = new Uri($"{url}/{spell.SpellName}.png");

            if (!_downloaded.Contains(spell.SpellName))
            {
                Download($"{spell.SpellName}.png", FileManager.SummonerSpellsFolder, currentAbillityURI, true);
                _downloaded.Add(spell.SpellName);
            }
        }

        private static void DownloadSpell(ChampionSpell spell, string url)
        {
            var currentAbillityURI = new Uri($"{url}/{spell.SpellName}.png");

            if (!_downloaded.Contains(spell.SpellName))
            {
                Download($"{spell.Slot}.png", FileManager.ChampionFolder(spell.ChampionName), currentAbillityURI, true);
                _downloaded.Add(spell.SpellName);
            }
        }

        private static void DownloadChampionIcon(ChampionSpell spell, string url)
        {
            var currentChampionURI = new Uri($"{url}/{spell.ChampionName}.png");

            if (!_downloaded.Contains(spell.ChampionName))
            {
                Download($"{spell.ChampionName}.png", FileManager.ChampionFolder(spell.ChampionName), currentChampionURI);
                _downloaded.Add(spell.ChampionName);
            }
        }

        private static void Download(ChampionSpell spell, string championurl, string abilityurl)
        {
            if (spell.Slot.IsSummonerSpell())
                DownloadSummonerSpells(spell, abilityurl);
            else
                DownloadSpell(spell, abilityurl);

            DownloadChampionIcon(spell, championurl);
        }

        private static void Download(string fileName, string folder, Uri uri, bool spell = false)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = $"{folder}/{fileName}";
            if (File.Exists(filePath))
            {
                if(!ValidateFile(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    Logger.Info($"Skipped {fileName}");
                    return;
                }
            }

            i++;
            var webClient = new WebClient();
            webClient.DownloadFileAsync(uri, $"{folder}/{fileName}");
            Logger.Info($"Downloading {uri}");
            webClient.DownloadFileCompleted += (sender, args) =>
                {
                    i--;
                    webClient.Dispose();
                };
        }

        private static bool ValidateFile(string path)
        {
            var file = new FileInfo(path);
            return file.Length > 1000;
        }
    }
}
