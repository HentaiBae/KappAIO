using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class DangerBuffDetector
    {
        static DangerBuffDetector()
        {
            foreach (var buff in DangerBuffDataDatabase.Current.Where(a => EntityManager.Heroes.AllHeroes.Any(b => a.Hero.Equals(b.Hero))))
            {
                _currentBuffs.Add(buff);
            }

            if (_currentBuffs.Any())
            {
                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
                Game.OnTick += Game_OnTick;
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var thebuff = args.Buff;
            var caster = thebuff?.Caster as AIHeroClient;
            if (caster != null && caster.IsValid)
            {
                foreach (var data in _currentBuffs.Where(b => b.Hero.Equals(caster.Hero) && !string.IsNullOrEmpty(b.BuffName)
                && (b.BuffName.Equals(args.Buff.Name, StringComparison.CurrentCultureIgnoreCase)
                || b.BuffName.Equals(args.Buff.DisplayName, StringComparison.CurrentCultureIgnoreCase))))
                {
                    var detected = new DetectedDangerBuffData
                    {
                        Caster = caster,
                        Target = sender,
                        Buff = thebuff,
                        Data = data
                    };

                    Add(detected);
                }
            }
        }

        private static List<DangerBuffData> _currentBuffs = new List<DangerBuffData>();
        public static List<DetectedDangerBuffData> DangerBuffsDetected = new List<DetectedDangerBuffData>();

        private static void Game_OnTick(EventArgs args)
        {
            DangerBuffsDetected.RemoveAll(b => b.Ended);

            foreach (var buff in DangerBuffsDetected)
            {
                OnDangerBuffDetected.Invoke(buff);
            }

            /*
            foreach (var target in EntityManager.Heroes.AllHeroes)
            {
                foreach (var data in _currentBuffs)
                {
                    var thebuff = target.GetBuff(data.BuffName);
                    var caster = thebuff?.Caster as AIHeroClient;
                    if (caster != null && caster.IsValid)
                    {
                        var detected = new DetectedDangerBuffData
                        {
                            Caster = caster,
                            Target = target,
                            Buff = thebuff,
                            Data = data
                        };

                        Add(detected);
                    }
                }
            }*/
        }

        public static void Add(DetectedDangerBuffData data)
        {
            if (data == null)
            {
                Console.WriteLine("Invalid DetectedDangerBuffData");
                return;
            }

            OnDangerBuffDetected.Invoke(data);

            if (!DangerBuffsDetected.Contains(data))
                DangerBuffsDetected.Add(data);
        }
    }
}
