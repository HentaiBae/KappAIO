using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class TargetedSpellDetector
    {
        private static bool Loaded;
        static TargetedSpellDetector()
        {
            if (!Loaded)
            {
                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Loaded = true;
            }
        }

        public static List<DetectedTargetedSpellData> DetectedTargetedSpells = new List<DetectedTargetedSpellData>();

        private static void Game_OnTick(EventArgs args)
        {
            DetectedTargetedSpells.RemoveAll(s => s.Ended);

            foreach (var spell in DetectedTargetedSpells)
                OnTargetedSpellDetected.Invoke(spell);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null)
                return;

            DetectedTargetedSpells.RemoveAll(a => a.Caster != null && a.Caster.IdEquals(caster) && a.Missile != null && a.Missile.IdEquals(missile));
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if(caster == null)
                return;

            var target = missile.Target as Obj_AI_Base;
            if(target == null)
                return;

            var data =
                TargetedSpellDatabase.List.FirstOrDefault(
                    s => s.MissileNames != null && s.MissileNames.Any(m => m.Equals(missile.SData.Name, StringComparison.CurrentCultureIgnoreCase)) && s.hero.Equals(caster.Hero));

            if(data == null)
                return;

            var detected = new DetectedTargetedSpellData
                {
                    Caster = caster,
                    Target = target,
                    Data = data,
                    Start = missile.StartPosition,
                    StartTick = Core.GameTickCount,
                    Missile = missile
                };

            Add(detected);
        }
        
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as Obj_AI_Base;

            if (caster == null || target == null || !caster.IsValid || !target.IsValid)
                return;

            var data = TargetedSpellDatabase.List.FirstOrDefault(s => s.hero.Equals(caster.Hero) && s.slot.Equals(args.Slot));

            if (data == null)
            {
                //Console.WriteLine($"OnProcessTargetedSpell: {caster.ChampionName} - [{args.Slot}] - Not Detected");
                return;
            }

            var detected = new DetectedTargetedSpellData
            {
                Caster = caster,
                Target = target,
                Data = data,
                Start = args.Start,
                StartTick = Core.GameTickCount
            };

            Add(detected);
        }

        private static void Add(DetectedTargetedSpellData data)
        {
            if (data == null || DetectedTargetedSpells.Contains(data))
            {
                Console.WriteLine("Invalid DetectedTargetedSpellData");
                return;
            }

            OnTargetedSpellDetected.Invoke(data);
            DetectedTargetedSpells.Add(data);
        }
    }
}
