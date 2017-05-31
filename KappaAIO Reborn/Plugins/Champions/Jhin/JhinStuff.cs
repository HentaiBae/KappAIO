using System;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using Color = System.Drawing.Color;

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

            return buff != null && buff.IsActive && ((buff.EndTime - Game.Time) * 1000f) > Game.Ping;
        }
    }

    public class JhinR
    {
        public JhinR()
        {
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            Game.OnTick += this.Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            if(this.IsActive)
                this.Sector.Draw(Color.AliceBlue);
        }

        private const string FirstR = "JhinR";
        public CustomGeometry.Sector2 Sector;
        public bool IsActive;
        private Vector3 lastRPos;
        private int lastR;

        private void Game_OnTick(EventArgs args)
        {
            var spellbook = Player.Instance.Spellbook;
            if ((!spellbook.IsChanneling && !spellbook.IsCharging && !spellbook.IsCastingSpell) || Core.GameTickCount - this.lastR > 11000)
            {
                this.IsActive = false;
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!sender.IsMe)
                return;

            if (args.Slot == SpellSlot.R)
            {
                if (args.SData.Name.Equals(FirstR))
                {
                    this.lastRPos = args.End;
                    this.IsActive = true;
                    this.lastR = Core.GameTickCount;
                    this.Sector = new CustomGeometry.Sector2(Player.Instance.ServerPosition, Player.Instance.ServerPosition.Extend(this.lastRPos, 3400).To3D(), (float)(60f * Math.PI / 180), 3400);
                }
            }
        }
    }
}
