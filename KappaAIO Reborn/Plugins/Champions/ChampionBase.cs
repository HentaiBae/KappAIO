using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;

namespace KappAIO_Reborn.Plugins.Champions
{
    public abstract class ChampionBase
    {
        public static Menu menu;
        public static AIHeroClient user => Player.Instance;
        public abstract void OnLoad();
        public abstract void OnTick();
        public abstract void Combo();
        public abstract void Flee();
        public abstract void Harass();
        public abstract void LaneClear();
        public abstract void JungleClear();
        public abstract void KillSteal();

        protected ChampionBase()
        {
            Loading.OnLoadingComplete += this.Loading_OnLoadingComplete;
        }

        private static float lastSpellCast;
        private void Loading_OnLoadingComplete(EventArgs args)
        {
            menu = Program.GlobalMenu;
            //menu.AddSubMenu($"- Champion: {Player.Instance.ChampionName}");
            this.OnLoad();
            Game.OnTick += this.Game_OnTick;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static bool wait => Core.GameTickCount - lastSpellCast < Game.Ping;

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && !wait)
                lastSpellCast = Core.GameTickCount;
        }

        private void Game_OnTick(EventArgs args)
        {
            if (user.IsDead || user.IsRecalling() || wait)
                return;

            this.OnTick();
            this.KillSteal();
            var orbMode = Orbwalker.ActiveModesFlags;

            if (orbMode.HasFlag(Orbwalker.ActiveModes.Combo))
                this.Combo();

            if (orbMode.HasFlag(Orbwalker.ActiveModes.Harass))
                this.Harass();

            if (orbMode.HasFlag(Orbwalker.ActiveModes.Flee))
                this.Flee();

            if (orbMode.HasFlag(Orbwalker.ActiveModes.LaneClear))
                this.LaneClear();

            if (orbMode.HasFlag(Orbwalker.ActiveModes.JungleClear))
                this.JungleClear();
        }
    }
}
