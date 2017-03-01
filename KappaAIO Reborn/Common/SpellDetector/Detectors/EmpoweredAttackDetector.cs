using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using KappAIO_Reborn.Common.Databases.Spells;
using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Events;
using SharpDX;

namespace KappAIO_Reborn.Common.SpellDetector.Detectors
{
    public class EmpoweredAttackDetector
    {
        private static bool Loaded;
        static EmpoweredAttackDetector()
        {
            if (!Loaded)
            {
                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
                //OnEmpoweredAttackDetected.OnDetect += OnEmpoweredAttackDetected_OnDetect;
                GameObject.OnCreate += GameObject_OnCreate;
                Loaded = true;
            }
        }

        private static void OnEmpoweredAttackDetected_OnDetect(DetectedEmpoweredAttackData args)
        {
            Console.WriteLine($"{args.Data.MenuItemName} {args.TicksLeft} {Core.GameTickCount} / {args.AttackCastDelay} {args.Speed}");
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if(caster == null || !missile.IsAutoAttack())
                return;

            var target = missile.Target as Obj_AI_Base;
            if (target != null)
            {
                var data = getData(caster, target, missile.StartPosition, missile.SData.Name);
                foreach (var d in data)
                {
                    Add(d);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            DetectedEmpoweredAttacks.RemoveAll(a => a.Ended);

            foreach (var attack in DetectedEmpoweredAttacks)
                OnEmpoweredAttackDetected.Invoke(attack);
        }

        public static List<DetectedEmpoweredAttackData> DetectedEmpoweredAttacks = new List<DetectedEmpoweredAttackData>();

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as Obj_AI_Base;

            if (caster == null || target == null || !caster.IsValid || !target.IsValid)
                return;

            var data = getData(caster, target, args.Start, args.SData.Name);
            foreach (var d in data)
            {
                Add(d);
            }
        }

        private static DetectedEmpoweredAttackData[] getData(AIHeroClient caster, Obj_AI_Base target, Vector3 start, string AttackName)
        {
            var result = new List<DetectedEmpoweredAttackData>();
            var infos = EmpowerdAttackDatabase.List.FindAll(s => s.Hero.Equals(caster.Hero) || s.Hero == Champion.Unknown);
            foreach (var info in infos)
            {
                var attackname = string.IsNullOrEmpty(info.AttackName) || info.AttackName.Equals(AttackName, StringComparison.CurrentCultureIgnoreCase);
                var requirebuff = string.IsNullOrEmpty(info.RequireBuff) || caster.GetBuffCount(info.RequireBuff) >= info.RequiredBuffCount;
                var targetrequirebuff = string.IsNullOrEmpty(info.TargetRequiredBuff) || target.GetBuffCount(info.TargetRequiredBuff) >= info.TargetRequiredBuffCount;
                var donthavebuff = string.IsNullOrEmpty(info.DontHaveBuff) || !target.HasBuff(info.DontHaveBuff);
                if (attackname && requirebuff && targetrequirebuff && donthavebuff)
                {
                    var detected = new DetectedEmpoweredAttackData
                    {
                        Start = start,
                        Caster = caster,
                        Target = target,
                        Data = info,
                        AttackCastDelay = caster.AttackCastDelay * 1000f,
                        Speed = Math.Max(250, caster.IsMelee ? int.MaxValue : caster.BasicAttack.MissileSpeed),
                        StartTick = Core.GameTickCount
                    };

                    result.Add(detected);
                }
            }

            return result.ToArray();
        }

        private static void Add(DetectedEmpoweredAttackData data)
        {
            if (data == null || DetectedEmpoweredAttacks.Contains(data))
            {
                Console.WriteLine("Invalid DetectedEmpoweredAttackData");
                return;
            }

            OnEmpoweredAttackDetected.Invoke(data);
            DetectedEmpoweredAttacks.Add(data);
        }
    }
}
