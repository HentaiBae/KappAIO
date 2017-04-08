using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Plugins.Champions;
using KappAIO_Reborn.Plugins.Utility;
using KappAIO_Reborn.Plugins.Utility.HUD;

namespace KappAIO_Reborn
{
    public class Program
    {
        public static float LoadTick { get; private set; }
        public static bool UsingMasterMind;
        public static Menu GlobalMenu, UtilityMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            LoadTick = Core.GameTickCount;
            GlobalMenu = MainMenu.AddMenu("KappAIO", "kappaio");
            var utility = GlobalMenu.CreateCheckBox("utility", "Load KappaUtility");

            try
            {
                var Instance = (ChampionBase)Activator.CreateInstance(null, $"KappAIO_Reborn.Plugins.Champions.{Player.Instance.ChampionName}.{Player.Instance.ChampionName}").Unwrap();
                GlobalMenu.DisplayName = $"KappAIO: {Player.Instance.ChampionName}";
            }
            catch (Exception)
            {
                Logger.Error($"KappAIO: {Player.Instance.ChampionName} Not Supported");
            }

            if (utility.CurrentValue)
            {
                UtilityMenu = MainMenu.AddMenu("KappaUtility", "KappaUtility");
                LoadUtility.Init();
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
