using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Plugins.Champions;
using KappAIO_Reborn.Plugins.Utility.Evade;
using KappAIO_Reborn.Plugins.Utility.HUD;

namespace KappAIO_Reborn
{
    public class Program
    {
        public static float LoadTick { get; private set; }
        public static bool UsingMasterMind;
        public static Menu GlobalMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            LoadTick = Core.GameTickCount;
            GlobalMenu = MainMenu.AddMenu("KappAIO", "kappaio");
            var evade = GlobalMenu.CreateCheckBox("Evade", "Enable Evade");
            var hud = GlobalMenu.CreateCheckBox("Hud", "Enable HUD");
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

            bool loadedEvade = false;
            if (evade.CurrentValue)
            {
                if (!loadedChampion)
                    GlobalMenu.DisplayName = "KappAIO: Evade";

                Evade.Init();
            }
            
            if (hud.CurrentValue)
            {
                if(!loadedEvade && !loadedChampion)
                    GlobalMenu.DisplayName = "KappAIO: HUD";
                HUDManager.Init();
            }

            //FoW.Init();

            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Core.GameTickCount - LoadTick > 3000 || UsingMasterMind) // give up or found mastermind kek
            {
                Game.OnTick -= Game_OnTick;
            }

            if (MainMenu.MenuInstances.Any(m => m.Key.ToLower().Contains("mastermind")))
            {
                UsingMasterMind = true;
            }
        }
    }
}
