using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK.Utils;
using KappAIO_Reborn.Common.Utility;
using KappAIO_Reborn.Plugins.Utility.Activator;
using KappAIO_Reborn.Plugins.Utility.HUD;
using KappAIO_Reborn.Plugins.Utility.Tracker;

namespace KappAIO_Reborn.Plugins.Utility
{
    public static class LoadUtility
    {
        private class UtilityToLoad
        {
            public UtilityToLoad(string name, Action a)
            {
                this.Name = name;
                this.Action = a;
            }
            public Action Action;
            public bool Enabled => Program.UtilityMenu.CheckBoxValue(this.Name);
            public string Name;
        }

        private static List<UtilityToLoad> toloadActions = new List<UtilityToLoad>
        {
            new UtilityToLoad("Evade", Evade.Init),
            new UtilityToLoad("Teleports Tracker", TeleportsTracker.Init),
            new UtilityToLoad("HUD", HUDManager.Init),
            new UtilityToLoad("Items Activator", ItemActiv.Init),
        };

        public static void Init()
        {
            foreach (var a in toloadActions)
                Program.UtilityMenu.CreateCheckBox(a.Name, $"Load {a.Name}");

            var count = toloadActions.Count(a => a.Enabled);
            Logger.Info($"KappAIO: Loading {count} Utility Plugin{(count > 1 ? "s" : "")}");
            foreach (var utility in toloadActions.Where(a => a.Enabled))
            {
                Logger.Info($"KappAIO: Loading {utility.Name}");
                utility.Action();
            }
        }
    }
}
