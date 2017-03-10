using System;
using EloBuddy;
using KappAIO_Reborn.Common.CustomEvents;

namespace KappAIO_Reborn.Plugins.Utility.Tracker
{
    public static class FoW
    {
        public static void Init()
        {
            Unit.OnPositionUpdate.OnUpdate += OnPositionUpdate_OnUpdate;
            Chat.OnClientSideMessage += Chat_OnClientSideMessage;
        }

        private static void Chat_OnClientSideMessage(ChatClientSideMessageEventArgs args)
        {
            if (args.Message.Contains("You must wait"))
                args.Process = false;
        }

        private static void OnPositionUpdate_OnUpdate(Unit.OnPositionUpdate.UpdatedPosition args)
        {
            if (args.FromFOW && !args.unit.IsAlly && !args.unit.IsDead)
            {
                Console.WriteLine($"Tracked: {args.unit.BaseSkinName}");
                TacticalMap.SendPing(PingCategory.Danger, args.After);
            }
        }
    }
}
