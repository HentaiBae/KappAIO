using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Utility;

namespace KappAIO_Reborn.Plugins.Utility.HUD
{
    public static class HUDConfig
    {
        private static Menu menu;
        public static bool DrawAlly => menu.CheckBoxValue("ally");
        public static bool DrawEnemy => menu.CheckBoxValue("enemy");
        public static bool DrawRecall => menu.CheckBoxValue("recall");
        public static bool DrawXP => menu.CheckBoxValue("xp");
        public static bool DrawHP => menu.CheckBoxValue("hp");
        public static bool DrawMP => menu.CheckBoxValue("mp");
        //public static bool DownloadAllTexture => menu.CheckBoxValue("downloadall");
        public static int Spacing => menu.SliderValue("space");
        public static int IconsSize => menu.SliderValue("iconsize");
        public static int BarSize => menu.SliderValue("barsize");
        public static int AllyX => menu.SliderValue("AllyX");
        public static int AllyY => menu.SliderValue("AllyY");
        public static int EnemyX => menu.SliderValue("EnemyX");
        public static int EnemyY => menu.SliderValue("EnemyY");
        private static bool loaded;
        
        public static void Init()
        {
            if(loaded)
                return;

            loaded = true;
            menu = Program.UtilityMenu.AddSubMenu("HUD: Settings");
            if (Program.UtilityMenu.CheckBoxValue("HUD"))
            {
                menu.AddGroupLabel("Teams:");
                menu.CreateCheckBox("ally", "Draw Ally Team");
                menu.CreateCheckBox("enemy", "Draw Enemy Team");
                menu.AddSeparator(0);

                menu.AddGroupLabel("Stats:");
                menu.CreateCheckBox("recall", "Draw Recall/Teleport Info");
                menu.CreateCheckBox("xp", "Draw XP Bar");
                menu.CreateCheckBox("hp", "Draw HP Bar");
                menu.CreateCheckBox("mp", "Draw MP Bar");
                menu.AddSeparator(0);

                /*
                menu.AddGroupLabel("Texture Updater:");
                menu.AddLabel("If you disable this option it will download");
                menu.AddLabel("only the texture in your game if it doesnt exist.");
                menu.CreateCheckBox("downloadall", "Download All Texture each patch", false);
                menu.AddSeparator(0);*/

                menu.AddGroupLabel("Position");
                menu.CreateSlider("space", "Spacing {0}", 20);
                menu.AddSeparator(0);

                menu.AddLabel("Ally Team");
                menu.CreateSlider("AllyX", "Ally X {0}%", 5);
                menu.CreateSlider("AllyY", "Ally Y {0}%", 15);
                menu.AddSeparator(0);

                menu.AddLabel("Enemy Team");
                menu.CreateSlider("EnemyX", "Enemy X {0}%", 90);
                menu.CreateSlider("EnemyY", "Enemy Y {0}%", 15);
                menu.AddSeparator(0);
            }

            menu.AddGroupLabel("Image Settings: (Require Reload F5)");
            menu.CreateSlider("iconsize", "Icon Size {0}%", 75, 15, 125);
            menu.CreateSlider("barsize", "(XP/HP/MP) Bar Size {0}%", 55, 15);
        }
    }
}
