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
        public AIHeroClient Caster;
        public AIHeroClient Target;
        public MissileClient Missile;
        public SkillshotData Data;
        public Vector3 Start;
        public Vector3 End;
        public Vector3? CollidePoint;
        public Obj_AI_Base CollideTarget;
        public Obj_AI_Base[] CollideTargets;
        public bool DetectedMissile => this.Missile != null;

        public float MaxTravelTime(Vector3 target)
        {
            return (this.Start.Distance(target) / this.Data.Speed * 1000f) + this.Data.CastDelay;
        }
        public float MaxTravelTime(Obj_AI_Base target)
        {
            return this.MaxTravelTime(target.ServerPosition);
        }

        public float TravelTime(Vector3 target)
        {
            var correct = (this.CurrentPosition.Distance(target) / this.Data.Speed * 1000f) + this.Data.CastDelay;
            if (this.DetectedMissile)
                return correct;

            return correct - this.TicksPassed;
        }

        public float TravelTime(Obj_AI_Base target)
        {
            return this.TravelTime(target.ServerPosition);
        }

        public float TimeToCollide => this.TravelTime(this.CollideEndPosition);
        public float StartTick;
        public float EndTick => this.StartTick + this.MaxTravelTime(this.EndPosition);
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => this.StartTick - Core.GameTickCount;
        public bool Ended => Core.GameTickCount - this.EndTick > 0 || this.TravelTime(this.CollideEndPosition) < 0;

        public bool WillHit(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
                return false;

            var pred = target.PrediectPosition((int)this.TravelTime(target));
            return this.Polygon != null && this.Polygon.IsInside(pred);
        }

        public Vector3 CurrentPosition
        {
            get
            {
                if (this.DetectedMissile)
                {
                    if (this.Data.SticksToMissile)
                    {
                        return this.Missile.Position.Extend(this.Start, -this.Data.Width / 2f).To3D();
                    }
                }
                if (this.Caster != null)
                {
                    if (this.Data.SticksToCaster)
                    {
                        return this.Caster.ServerPosition;
                    }
                }
                if (this.Target != null)
                {
                    if (this.Data.StartsFromTarget)
                    {
                        return this.Target.ServerPosition;
                    }
                }

                return this.Start;
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                var endpos = Vector3.Zero;

                if (this.Data.DodgeFrom != null && this.Data.DodgeFrom.Any())
                {
                    var obj = ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(o => o.IsValid && this.Data.DodgeFrom.Contains(o.Name));
                    if (obj != null)
                    {
                        endpos = obj.Position;
                    }
                }

                if (this.Caster != null)
                {
                    var direction = this.Caster.Direction().Distance(this.Caster) < 100 ? this.Caster.Direction().To3D() : this.Caster.Path.LastOrDefault();
                    if (this.Data.EndIsCasterDirection)
                    {
                        endpos = direction;
                    }
                    if (this.Data.EndSticksToCaster)
                    {
                        endpos = this.Caster.ServerPosition;
                    }
                    if (this.Data.SpellName.Equals("SionR"))
                    {
                        this.Data.Speed = this.Caster.MoveSpeed;
                        endpos = this.CurrentPosition.Extend(this.Start.Extend(this.Caster, -this.Data.Range).To3D(), this.Data.Range).To3D();
                    }
                }

                if (this.Target != null)
                {
                    if (!string.IsNullOrEmpty(this.Data.TargetName) && this.Target.Name.Equals(this.Data.TargetName))
                    {
                        endpos = this.Target.Position;
                    }
                }

                if (endpos.IsZero)
                {
                    endpos = this.Start.Distance(this.End) > this.Data.Range ? this.Start.Extend(this.End, this.Data.Range).To3D() : this.End;
                }
                Vector3 result;
                if (this.Data.IsFixedRange)
                {
                    result = this.Start.Extend(endpos, this.Data.Range).To3D();
                }
                else
                {
                    result = this.Data.ExtraRange > 0 && Data.ExtraRange < float.MaxValue && Data.ExtraRange < int.MaxValue ? endpos.Extend(this.Start, -this.Data.ExtraRange).To3D() : endpos;
                }

                return result;
            }
        }

        public Vector3 CollideEndPosition
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
                    return new Geometry.Polygon.Sector(this.CurrentPosition, this.CollideEndPosition, (float)(this.Data.Angle * Math.PI / 180), this.Data.Range);
                }
                return null;
            }
        }

        public Geometry.Polygon OriginalPolygon
        {
            get
            {
                var extraWidth = Player.Instance.BoundingRadius / 2f;
                var width = this.Data.Width + extraWidth;
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
