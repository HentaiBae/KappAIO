using EloBuddy;

namespace KappAIO_Reborn.Plugins.Champions.Jhin
{
    public static class JhinStuff
    {
        private const string WBuffName = "JhinESpottedDebuff";

        public static bool CanW(this AIHeroClient target, bool preBuff = false)
        {
            if (target == null)
                return false;

            if (preBuff)
            {
                
            }

            var buff = target.GetBuff(WBuffName);

            return buff != null && buff.IsActive && (buff.EndTime - Game.Time * 1000f) > Game.Ping; ;
        }
    }
}
