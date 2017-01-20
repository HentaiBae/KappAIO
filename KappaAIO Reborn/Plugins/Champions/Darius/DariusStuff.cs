using System;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Utility;

namespace KappAIO_Reborn.Plugins.Champions.Darius
{
    internal static class DariusStuff
    {
        private static string _passiveName = "DariusHemo";
        private static string _qChargingBuff = "dariusqcast";
        private static string _ultResetBuff = "DariusExecuteMulticast";
        
        internal static bool HasDariusQChargingBuff => Player.HasBuff(_qChargingBuff);
        internal static bool HasDariusUltResetBuff => DariusUltResetBuff != null;

        internal static BuffInstance DariusUltResetBuff => Player.Instance.GetBuff(_ultResetBuff);

        internal static BuffInstance GetDariusPassive(Obj_AI_Base target) => target.GetBuff(_passiveName);

        internal static float PassiveDamage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return 0;

            if (!target.HasBuff(_passiveName))
                return 0;

            var basedmg = 9 + Player.Instance.Level;
            var mod = 0.3f;
            var bonusAD = Player.Instance.FlatPhysicalDamageMod;
            var currentDmg = basedmg + (bonusAD * mod);

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, currentDmg);
        }

        internal static float CurrentPassiveDamage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return 0;

            var buff = GetDariusPassive(target);
            if (buff == null || !buff.IsValid || !buff.IsActive)
                return 0;

            return PassiveDamage(target) * buff.Count;
        }

        internal static float PassiveDamageLeft(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return 0;

            var buff = GetDariusPassive(target);
            if (buff == null)
                return 0;
            
            var timePassed = Game.Time - buff.StartTime;
            var maxPassiveDamage = CurrentPassiveDamage(target);
            var minPassiveDamage = maxPassiveDamage / timePassed;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, Math.Min(maxPassiveDamage, minPassiveDamage));
        }

        internal static float QDmg(Obj_AI_Base target, bool blade = true)
        {
            if (!target.IsValidTarget())
                return 0;

            var dmgs = new[] { 20f, 35f, 50f, 65f, 80f };
            var mods = new[] { 0.5f, 0.55f, 0.6f, 0.65f, 0.7f };
            var scale = Player.Instance.TotalAttackDamage;
            var currentDmg = dmgs.GetCurrentDamage(mods, SpellSlot.Q, scale);

            if (blade)
                currentDmg *= 2f;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, currentDmg);
        }

        internal static float Wdmg(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return 0;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, Player.Instance.TotalAttackDamage * 0.4f);
        }

        internal static float Rdmg(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return 0;

            var index = Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1;

            if (index < 0)
                return 0;

            var baseDmg = new[] { 100f, 200f, 300f }[index];
            var mod = 0.75f;
            var scale = Player.Instance.FlatPhysicalDamageMod;
            var currentDamage = baseDmg + (scale * mod);
            var buff = GetDariusPassive(target);

            if (buff == null)
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, currentDamage);
            
            var scalePerStack = new[] { 20f, 40f, 60f }[index];
            var scaleMod = 0.15f;
            var currentStacksDamage = (scalePerStack + (scale * scaleMod)) * buff.Count;

            var maxRDamage = currentDamage * 2f;
            var minRDamage = baseDmg + currentStacksDamage;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, Math.Min(minRDamage, maxRDamage));
        }

        internal static float ComboDamage(Obj_AI_Base target, bool passive = true, bool q = true, bool w = true, bool r = true)
        {
            var result = 0f;
            var manacost = 0f;

            if (passive)
                result += PassiveDamageLeft(target);

            if (r && Darius.R.IsReady())
            {
                manacost += Darius.R.ManaCost;
                result += Rdmg(target);
            }
            
            if (q && Darius.Q.IsReady() && Player.Instance.Mana > manacost)
            {
                manacost += Darius.Q.ManaCost;
                result += QDmg(target);
            }

            if (w && Darius.W.IsReady() && Player.Instance.Mana > manacost)
            {
                result += Wdmg(target);
            }
            
            return result;
        }
    }
}
