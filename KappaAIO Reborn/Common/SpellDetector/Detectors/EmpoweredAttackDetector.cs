using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Utils;
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
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnBasicAttack;
                //OnEmpoweredAttackDetected.OnDetect += OnEmpoweredAttackDetected_OnDetect;
                GameObject.OnCreate += GameObject_OnCreate;
                Loaded = true;
            }
        }

        public static List<DetectedEmpoweredAttackData> DetectedEmpoweredAttacks = new List<DetectedEmpoweredAttackData>();

        private static void OnEmpoweredAttackDetected_OnDetect(DetectedEmpoweredAttackData args)
        {
            Console.WriteLine($"{args.Data.MenuItemName} {args.TicksLeft} {Core.GameTickCount} / {args.AttackCastDelay} {args.Speed}");
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null || !missile.IsAutoAttack())
                return;

            var target = missile.Target as Obj_AI_Base;
            if (target != null)
            {
                var data = getData(caster, target, missile.Position, missile.SData.Name, missile);
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
                OnEmpoweredAttackUpdate.Invoke(attack);
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var target = args.Target as Obj_AI_Base;

            if (caster == null || target == null || !caster.IsValid || !target.IsValid || !args.IsAutoAttack())
                return;

            var data = getData(caster, target, args.Start, args.SData.Name);
            foreach (var d in data)
            {
                Add(d);
            }
        }

        private static IEnumerable<DetectedEmpoweredAttackData> getData(AIHeroClient caster, Obj_AI_Base target, Vector3 start, string AttackName, MissileClient missile = null)
        {
            var infos = EmpowerdAttackDatabase.Current.Where(s => s.Hero.Equals(caster.Hero) || s.Hero == Champion.Unknown);

            return (from info in infos
                    let attackname = string.IsNullOrEmpty(info.AttackName) || info.AttackName.Equals(AttackName, StringComparison.CurrentCultureIgnoreCase)
                    let requirebuff = string.IsNullOrEmpty(info.RequireBuff) || caster.GetBuffCount(info.RequireBuff) >= info.RequiredBuffCount
                    let targetrequirebuff = string.IsNullOrEmpty(info.TargetRequiredBuff) || target.GetBuffCount(info.TargetRequiredBuff) >= info.TargetRequiredBuffCount
                    let donthavebuff = string.IsNullOrEmpty(info.DontHaveBuff) || !target.HasBuff(info.DontHaveBuff)
                    where attackname && requirebuff && targetrequirebuff && donthavebuff
                    select new DetectedEmpoweredAttackData
                    {
                        Start = start,
                        Caster = caster,
                        Target = target,
                        Data = info,
                        AttackCastDelay = missile != null ? 0 : caster.AttackCastDelay * 1000f,
                        Speed = caster.IsMelee ? int.MaxValue : missile?.SData.MissileSpeed ?? caster.BasicAttack.MissileSpeed,
                        StartTick = Core.GameTickCount
                        });
        }

        private static void Add(DetectedEmpoweredAttackData data)
        {
            if (data == null || DetectedEmpoweredAttacks.Contains(data))
            {
                Console.WriteLine("Invalid DetectedEmpoweredAttackData");
                return;
            }

            if (data.Missile != null)
            {
                var detect = DetectedEmpoweredAttacks.FirstOrDefault(a => a.Caster.IdEquals(data.Caster) && a.Missile == null && a.Data.Equals(data.Data));
                if (detect != null)
                {
                    DetectedEmpoweredAttacks.Remove(detect);
                }
            }
            
            OnEmpoweredAttackDetected.Invoke(data);
            DetectedEmpoweredAttacks.Add(data);
        }
    }
}
