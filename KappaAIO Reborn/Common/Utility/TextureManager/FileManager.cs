using System;
using EloBuddy;

namespace KappAIO_Reborn.Common.Utility.TextureManager
{
    public static class FileManager
    {
        public static string KappaUtilityFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\KappaUtility";
        public static string ChampionFolder(Champion Champion) => $"{KappaUtilityFolder}/{GameVersion.CurrentPatch()}/ChampionIcons/{Champion}";
        public static string SummonerSpellsFolder => $"{KappaUtilityFolder}/{GameVersion.CurrentPatch()}/SummonerSpellsIcons";
    }
}
