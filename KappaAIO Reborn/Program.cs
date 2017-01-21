using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using KappAIO_Reborn.Common.Utility;
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
            GlobalMenu.CreateCheckBox("skillshots", "Enable Skillshots Drawings");

            try
            {
                var Instance = (ChampionBase)Activator.CreateInstance(null, $"KappAIO_Reborn.Plugins.Champions.{Player.Instance.ChampionName}.{Player.Instance.ChampionName}").Unwrap();
                GlobalMenu.DisplayName = $"KappAIO: {Player.Instance.ChampionName}";
            }
            catch (Exception)
            {
                Console.WriteLine($"{Player.Instance.ChampionName} Not Supported");
            }

            //Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if(!GlobalMenu.CheckBoxValue("skillshots"))
                return;

            foreach (var skill in SkillshotDetector.SkillshotsDetected.Where(s => s.Caster.IsEnemy))
            {
                skill.Polygon?.Draw(Color.AliceBlue, 2);
            }
        }
    }
}
