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
        private static bool loaded;
        static DangerBuffDetector()
        {
            if(loaded)
                return;
            loaded = true;
            _currentBuffs = DangerBuffDataDatabase.Current;

            if (_currentBuffs.Any())
            {
                Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
                Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Game.OnTick += Game_OnTick;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if(caster == null)
                return;
            
            foreach (var data in _currentBuffs.Where(b => b.Hero.Equals(caster.Hero) && b.RequireCast && !string.IsNullOrEmpty(b.BuffName) && b.Slot == args.Slot))
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget() && o.Team != caster.Team && o.HasBuff(data.BuffName)))
                {
                    var getBuff = obj.GetBuff(data.BuffName);
                    if (getBuff != null)
                    {
                        var detected = new DetectedDangerBuffData
                        {
                            Caster = caster,
                            Target = obj,
                            Buff = getBuff,
                            Data = data
                        };

                        Add(detected);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            DangerBuffsDetected.RemoveAll(b => 
            b.Buff.Caster.IdEquals(args.Buff.Caster)
            && b.Buff.DisplayName.Equals(args.Buff.DisplayName)
            && args.Buff.Name.Equals(b.Buff.Name)
            && b.Target.IdEquals(sender) && (b.Ended = true));
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var thebuff = args.Buff;
            var caster = thebuff?.Caster as AIHeroClient;
            if (caster != null && caster.IsValid)
            {
                foreach (var data in _currentBuffs.Where(b => b.Hero.Equals(caster.Hero) && !b.RequireCast && !string.IsNullOrEmpty(b.BuffName)
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

        private static DangerBuffData[] _currentBuffs = { };
        public static List<DetectedDangerBuffData> DangerBuffsDetected = new List<DetectedDangerBuffData>();

        private static void Game_OnTick(EventArgs args)
        {
            DangerBuffsDetected.RemoveAll(b => b.Ended);

            foreach (var buff in DangerBuffsDetected)
                OnDangerBuffUpdate.Invoke(buff);

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
