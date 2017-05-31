using System.Collections.Generic;
using System.Linq;
using KappAIO_Reborn.Common.Databases.Items;

namespace KappAIO_Reborn.Plugins.Utility.Activator
{
    public static class ItemActiv
    {
        private static List<ItemInstance> currentItemsInstances = new List<ItemInstance>();

        public static void Init()
        {
            var menu = Program.UtilityMenu.AddSubMenu("Items Activator");
            foreach (var item in ItemsDatabase.Current.Where(i => i.item.ItemInfo.AvailableForMap && !i.matchCastType(CastTime.Cleanse)))
                currentItemsInstances.Add(new ItemInstance(item, menu));

            Cleanse.Init(ItemsDatabase.Current.Where(i => i.matchCastType(CastTime.Cleanse)).ToArray());
        }
    }
}
