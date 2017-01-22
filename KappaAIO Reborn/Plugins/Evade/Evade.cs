using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using KappAIO_Reborn.Common.Utility;

namespace KappAIO_Reborn.Plugins.Evade
{
    public static class Evade
    {
        public static Menu menu, DrawMenu;
        public static void Init()
        {
            menu = Program.GlobalMenu;
            DrawMenu = menu.AddSubMenu("Evade: Drawings");
            DrawMenu.AddGroupLabel("Evade: Drawings");
            DrawMenu.CreateCheckBox("enable", "Enable Drawings");

            DrawMenu.AddSeparator(0);

            var skillshots = SkillshotDatabase.List.FindAll(s => EntityManager.Heroes.Enemies.Any(h => s.hero.Equals(h.Hero)));

            foreach (var skill in skillshots.OrderBy(s => s.hero))
            {
                DrawMenu.AddLabel(skill.MenuItemName);
                DrawMenu.CreateCheckBox($"Draw{skill.MenuItemName}", "Draw");
            }

            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!DrawMenu.CheckBoxValue("enable"))
                return;

            foreach (var skill in SkillshotDetector.SkillshotsDetected.Where(s => s.Caster.IsEnemy && DrawMenu.CheckBoxValue($"Draw{s.Data.MenuItemName}")))
            {
                skill.Polygon?.Draw(Color.AliceBlue, 2);
            }
        }
    }
}
