using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Utility;
using SharpDX;

namespace KappAIO_Reborn.Common.Databases.Items
{
    public enum ItemCastType
    {
        Active, Targeted, Position
    }

    public enum TargetingType
    {
        All, AllAllies, AllyHeros, AllyMinions, AllEnemies, EnemyHeros, EnemyMinions, MyHero
    }

    public enum CastTime
    {
        AoE, MyHealth, EnemyHealth, AllyHealth, PostAttack, PreAttck, OnTick, Cleanse
    }

    public class ItemData
    {
        public ItemData(ItemId id, ItemCastType type, float range, float speed, float castDelay, params TargetingType[] targets)
        {
            this.item = new Item(id, range);
            this.CastDelay = castDelay;
            this.Speed = speed;
            this.CastType = type;
            this.TargetType = targets;
        }
        public ItemData(int id, ItemCastType type, float range, float speed, float castDelay, params TargetingType[] targets)
        {
            this.item = new Item(id, range);
            this.CastDelay = castDelay;
            this.Speed = speed;
            this.CastType = type;
            this.TargetType = targets;
        }

        public Item item;
        public string Name => this.item.ItemInfo.Name;
        public float Range => this.item.Range;
        public float Width = 0;
        public float Radius => this.Width / 2f;
        public float CastDelay;
        public float Speed;
        public int MyHP, AllyHP, EnemyHP, AoeHit;
        public ItemCastType CastType;
        public TargetingType[] TargetType;
        public CastTime[] CastTimes;
        public Orbwalker.ActiveModes[] OrbModes;
        public bool Ready => this.item.IsReady() && this.item.IsOwned(Player.Instance);

        public void Cast()
        {
            if (Ready && this.CastType.Equals(ItemCastType.Active))
            {
                this.item.Cast();
            }
        }

        public void Cast(Obj_AI_Base target)
        {
            if(!this.Ready || !target.IsValidTarget(this.Range + this.Radius))
                return;

            switch (this.CastType)
            {
                case ItemCastType.Active:
                    {
                        this.item.Cast();
                    }
                    break;
                case ItemCastType.Targeted:
                    {
                        this.item.Cast(target);
                    }
                    break;
                case ItemCastType.Position:
                    {
                        var travelTime = (int)((Player.Instance.Distance(target) / this.Speed * 1000f) + this.CastDelay);
                        var castpos = target.PrediectPosition(travelTime);

                        if(castpos.Distance(Player.Instance) <= this.Range)
                            this.Cast(castpos);
                        else if(castpos.Distance(Player.Instance) <= this.Range + this.Radius)
                            this.Cast(Player.Instance.ServerPosition.Extend(castpos, this.Range + this.Radius).To3DWorld());
                    }
                    break;
            }
        }

        public void Cast(Vector3 target)
        {
            if (this.Ready && this.IsInRange(target) && target.IsValid())
            {
                this.item.Cast(target);
            }
        }

        public Vector3 GetPrediction(Obj_AI_Base target)
        {
            var data = new Prediction.Position.PredictionData(
                Prediction.Position.PredictionData.PredictionType.Circular,
                (int)Range,
                (int)Radius,
                0,
                (int)this.CastDelay,
                (int)this.Speed,
                int.MaxValue);
            return Prediction.Position.GetPrediction(target, data).CastPosition;
        }

        public bool IsInRange(Obj_AI_Base target)
        {
            return target != null && this.IsInRange(target.Position, target.BoundingRadius/2);
        }

        public bool IsInRange(AttackableUnit target)
        {
            return target != null && this.IsInRange(target.Position, target.BoundingRadius / 2);
        }

        public bool IsInRange(Vector3 target, float hitbox = 0)
        {
            return Player.Instance.IsInRange(target, this.Range + this.Radius + hitbox);
        }

        public bool ShouldCast(AttackableUnit target/*, params CastTime[] timeCasted*/)
        {
            if (!this.Ready || !target.IsValidTarget() || !this.IsInRange(target))
                return false;

            return this.TargetTypeMatch(target);// && this.matchCastType(timeCasted);
        }

        public bool TargetTypeMatch(AttackableUnit target)
        {
            var targettype =
                target is AIHeroClient ? target.IsEnemy ? TargetingType.EnemyHeros : TargetingType.AllyHeros
                : target is Obj_AI_Minion ? target.IsEnemy ? TargetingType.EnemyMinions : TargetingType.AllyMinions
                : TargetingType.All;

            var allCheck = target.IsEnemy ? TargetingType.AllEnemies : TargetingType.AllAllies;
            return this.matchType(TargetingType.All, allCheck, targettype);
        }

        private bool matchType(params TargetingType[] types)
        {
            return this.TargetType == null || types.Any(this.TargetType.Contains);
        }

        public bool matchCastType(params CastTime[] timeCasted)
        {
            return this.CastTimes == null || timeCasted.Any(this.CastTimes.Contains);
        }

        public bool ModeIsActive
        {
            get
            {
                return this.OrbModes == null || Orbwalker.ModeIsActive(this.OrbModes);
            }
        }
    }
}