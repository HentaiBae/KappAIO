using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Plugins.Champions;
using KappAIO_Reborn.Plugins.Evade;

namespace KappAIO_Reborn
{
    class Program
    {
        public static Menu GlobalMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            GlobalMenu = MainMenu.AddMenu("KappAIO", "kappaio");
            var evade = GlobalMenu.CreateCheckBox("Evade", "Enable Evade");
            bool loadedChampion;

            try
            {
                var Instance = (ChampionBase)Activator.CreateInstance(null, $"KappAIO_Reborn.Plugins.Champions.{Player.Instance.ChampionName}.{Player.Instance.ChampionName}").Unwrap();
                GlobalMenu.DisplayName = $"KappAIO: {Player.Instance.ChampionName}";
                loadedChampion = true;
            }
            catch (Exception)
            {
                Console.WriteLine($"{Player.Instance.ChampionName} Not Supported");
                loadedChampion = false;
            }

            if (evade.CurrentValue)
            {
                GlobalMenu.AddSubMenu("- Evade");
                if (!loadedChampion)
                    GlobalMenu.DisplayName = "KappAIO: Evade";

                Evade.Init();
            }
        }
    }
}
