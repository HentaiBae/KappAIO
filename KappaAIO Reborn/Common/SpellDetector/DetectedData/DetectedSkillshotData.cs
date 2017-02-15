using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using Type = KappAIO_Reborn.Common.Databases.SpellData.Type;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedSkillshotData
    {
        public Obj_AI_Base Caster;
        public Obj_AI_Base Target;
        public Obj_GeneralParticleEmitter Particle;
        public MissileClient Missile;
        public SkillshotData Data;
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Direction => (this.End - this.Start).Normalized();
        public Vector2? CollidePoint;
        public Obj_AI_Base CollideTarget;
        public Obj_AI_Base[] CollideTargets;
        public bool DetectedMissile => this.Missile != null;

        public Type? _type;
        public Type SkillshotType
        {
            get
            {
                return _type ?? this.Data.type;
            }
            set
            {
                this._type = value;
            }
        }

        public bool IsGlobal
        {
            get
            {
                return this.Data.Range >= 4000 && this.Data.Range < 15000;
            }
        }

        public bool IsVisible
        {
            get
            {
                return (this.CurrentPosition.IsOnScreen() || this.CurrentPosition.IsInRange(Player.Instance, 3000) || this.CollideEndPosition.IsOnScreen() || this.CollideEndPosition.IsInRange(Player.Instance, 3000)) || DetectedMissile && this.Missile.Position.IsOnScreen();
            }
        }

        private float delay => Data.CastDelay > TicksPassed && DetectedMissile && this.Data.type != Type.CircleMissile ? 0 : this.Data.CastDelay;

        public float MaxTravelTime(Vector2 target)
        {
            return (this.Start.Distance(target) / this.Data.Speed * 1000f) + delay + this.Data.ExtraDuration;
        }
        public float MaxTravelTime(Obj_AI_Base target)
        {
            return this.MaxTravelTime(target.ServerPosition.To2D());
        }

        public float TravelTime(Vector2 target)
        {
            var correct = (this.Start.Distance(target) / this.Data.Speed * 1000f) + this.delay;

            if (this.Data.type == Type.CircleMissile)
            {
                correct = CollideEndPosition.Distance(target) / this.Data.Speed * 1000f + this.delay;
            }

            return correct - this.TicksPassed;
        }

        public float TravelTime(Obj_AI_Base target)
        {
            return this.TravelTime(target.ServerPosition.To2D());
        }

        public float TimeToCollide => this.TravelTime(this.CollideEndPosition);
        public float StartTick;
        public float EndTick => this.StartTick + this.MaxTravelTime(this.CollideEndPosition);
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        public bool Ended => (IsGlobal && DetectedMissile ? (this.Missile == null || this.Missile.IsDead) : (Core.GameTickCount - this.EndTick > 0)
            || (this.DetectedMissile && this.Missile.IsDead) || this.Target != null && this.Target.IsDead) || this.TicksPassed > 20000;

        public bool IsInside(Obj_AI_Base target)
        {
            var hitbox = new EloBuddy.SDK.Geometry.Polygon.Circle(target.ServerPosition, target.BoundingRadius);
            var predhitbox = new EloBuddy.SDK.Geometry.Polygon.Circle(target.PrediectPosition(TravelTime(target)), target.BoundingRadius);
            return hitbox.Points.Any(OriginalPolygon.IsInside) && predhitbox.Points.Any(OriginalPolygon.IsInside);
        }

        public bool WillHit(Obj_AI_Base target, float time = -1f)
        {
            if (!target.IsValidTarget())
                return false;

            time = time.Equals(-1) ? Math.Max(this.TravelTime(target), 0) : time;
            var pred = target.PrediectPosition((int)time);
            return this.WillHit(pred.To2D());
        }
        public bool WillHit(Vector2 target)
        {
            if (!target.IsValid())
                return false;

            return this.Polygon != null && this.Polygon.IsInside(target);
        }

        public Vector2 CurrentPosition
        {
            get
            {
                if (this.DetectedMissile)
                {
                    if (this.Data.SticksToMissile)
                    {
                        return this.Missile.Position.Extend(this.Start, -this.Data.Width / 2f);
                    }
                }
                if (this.Caster != null)
                {
                    if (this.Data.SticksToCaster)
                    {
                        return this.Caster.PrediectPosition(TicksLeft).To2D();
                    }
                }
                if (this.Target != null)
                {
                    if (this.Data.StartsFromTarget)
                    {
                        return this.Target.ServerPosition.To2D();
                    }
                }
                
                return this.Start;
            }
        }

        public Vector2 EndPosition
        {
            get
            {
                var endpos = Vector2.Zero;

                if (this.Data.DodgeFrom != null && this.Data.DodgeFrom.Any())
                {
                    var obj = ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(o => o.IsValid && this.Data.DodgeFrom.Contains(o.Name));
                    if (obj != null)
                    {
                        endpos = obj.Position.To2D();
                    }
                }

                if (this.Caster != null)
                {
                    var direction = (this.Caster.Direction().Distance(this.Caster) < 100 ? this.Caster.Direction() : this.Caster.Path.LastOrDefault().To2D());
                    if (this.Data.EndIsCasterDirection)
                    {
                        endpos = direction;
                    }
                    if (this.Data.EndSticksToCaster)
                    {
                        return this.Caster.ServerPosition.To2D();
                    }
                    if (this.Data.SpellName.Equals("SionR"))
                    {
                        this.Data.Speed = this.Caster.MoveSpeed;
                        endpos = this.Caster.ServerPosition.Extend(this.Start, -this.Data.Range);
                    }
                    if (this.Data.SticksToCaster)
                    {
                        if (this.Data.SpellName == "TaricE")
                        {
                            return this.Caster.ServerPosition.To2D() + this.Direction * this.Data.Range;
                        }
                    }
                }

                if (this.DetectedMissile)
                {
                    if (this.Data.EndSticksToMissile)
                    {
                        endpos = this.Missile.Position.To2D();
                    }
                }

                if (this.Target != null)
                {
                    if (!string.IsNullOrEmpty(this.Data.TargetName) && this.Target.Name.Equals(this.Data.TargetName))
                    {
                        endpos = this.Target.Position.To2D();
                    }
                }

                if (endpos.IsZero)
                {
                    endpos = this.Start.Distance(this.End) > this.Data.Range ? this.Start.Extend(this.End, this.Data.Range) : this.End;
                }
                Vector2 result;

                if (this.Data.IsFixedRange)
                {
                    result = this.Start.Extend(endpos, this.Data.Range);
                }
                else
                {
                    result = this.Data.ExtraRange > 0 && this.Data.ExtraRange < float.MaxValue && this.Data.ExtraRange < int.MaxValue ? endpos.Extend(this.Start, -this.Data.ExtraRange) : endpos;
                }

                if (this.Data.StaticEnd)
                {
                    result = this.End;
                }

                return result;
            }
        }

        public Vector2 CollideEndPosition
        {
            get
            {
                if (this.CollidePoint.HasValue)
                {
                    return this.CollidePoint.Value;
                }

                return this.EndPosition;
            }
        }

        public Geometry.Polygon Polygon
        {
            get
            {
                var extraWidth = Player.Instance.BoundingRadius / 2f;
                var width = this.Data.Width + extraWidth;
                if (this.Data.type == Type.LineMissile)
                {
                    return new Geometry.Polygon.Rectangle(this.CurrentPosition, this.CollideEndPosition, width);
                }
                if (this.Data.type == Type.CircleMissile)
                {
                    return new Geometry.Polygon.Circle(this.CollideEndPosition, width);
                }
                if (this.Data.type == Type.Cone)
                {
                    return new Geometry.Polygon.Sector(this.CurrentPosition, this.CollideEndPosition, (float)((this.Data.Angle + 5) * Math.PI / 180), this.Data.Range + 10);
                }
                if (this.Data.type == Type.Ring)
                {
                    return new CustomGeometry.Ring(CollideEndPosition, width, this.Data.RingRadius).ToSDKPolygon();
                }
                if (this.Data.type == Type.Arc)
                {
                    return new CustomGeometry.Arc(CurrentPosition, CollideEndPosition, 25 + (int)ObjectManager.Player.BoundingRadius).ToSDKPolygon();
                }
                return null;
            }
        }

        public Geometry.Polygon OriginalPolygon
        {
            get
            {
                var width = this.Data.Width;
                if (this.Data.type == Type.LineMissile)
                {
                    return new Geometry.Polygon.Rectangle(this.CurrentPosition, this.EndPosition, width);
                }
                if (this.Data.type == Type.CircleMissile)
                {
                    return new Geometry.Polygon.Circle(this.EndPosition, width);
                }
                if (this.Data.type == Type.Cone)
                {
                    return new Geometry.Polygon.Sector(this.CurrentPosition, this.End, (float)(this.Data.Angle * Math.PI / 180), this.Data.Range);
                }
                return null;
            }
        }
    }
}
