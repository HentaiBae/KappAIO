using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;

namespace KappAIO_Reborn.Plugins.Champions
{
    public abstract class ChampionBase
    {
        public Menu menu;
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

        private void Loading_OnLoadingComplete(EventArgs args)
        {
            this.menu = MainMenu.AddMenu($"Kappa {Player.Instance.ChampionName}", $"kappaio{Player.Instance.ChampionName}");
            this.OnLoad();
            Game.OnTick += this.Game_OnTick;
        }

        private void Game_OnTick(EventArgs args)
        {
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
