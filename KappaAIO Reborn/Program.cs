using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Plugins.Champions;

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

            var Instance = (ChampionBase)Activator.CreateInstance(null, $"KappaAIO_Reborn.Plugins.Champions.{Player.Instance.ChampionName}.{Player.Instance.ChampionName}").Unwrap();
        }
    }
}
