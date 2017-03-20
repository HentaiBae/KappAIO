using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
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
        public AIHeroClient Target;
        public MissileClient Missile;
        public Obj_GeneralParticleEmitter Particle;
        public SkillshotData Data;
        public Vector2 Start;
        public Vector2 End;
        public Vector2? CollidePoint;
        public Vector2 Direction => (this.End - this.Start).Normalized();
        public Obj_AI_Base CollideTarget;
        public Obj_AI_Base[] CollideTargets;
        public bool DetectedMissile;
        public bool IsGlobal
        {
            get
            {
                return this.Data.Range >= 4000 && this.Data.Range < 15000;
            }
        }

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

        public float extraDelay;
        public float CastDelay
        {
            get
            {
                var result = 0f;
                if (!this.DetectedMissile || this.Data.type == Type.CircleMissile || this.Data.type == Type.Cone || this.Data.type == Type.Arc || this.Data.type == Type.Ring)
                {
                    result += this.Data.CastDelay;
                }

                if (result.Equals(0f) && (this.Speed <= 0 || this.Speed >= int.MaxValue))
                    result += this.Data.CastDelay;

                if (!Data.DontAddExtraDuration && Data.ExtraDuration > 0 && Data.ExtraDuration < int.MaxValue)
                {
                    result += Data.ExtraDuration;
                }

                return result + extraDelay;
            }
        }
        public float delay => CastDelay;

        private float? _speed;
        public float Speed
        {
            get
            {
                return _speed ?? this.Data.Speed;
            }
            set
            {
                this._speed = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                //return true;
                return (this.CurrentPosition.IsOnScreen() || this.CurrentPosition.IsInRange(Player.Instance, 3000)
                    || this.CollideEndPosition.IsOnScreen() || this.CollideEndPosition.IsInRange(Player.Instance, 3000))
                    || Missile != null && this.Missile.Position.IsOnScreen() || Polygon.Points.Any(p => p.IsOnScreen());
            }
        }

        public float MaxTravelTime(Vector2 target)
        {
            return (this.Start.Distance(target) / this.Speed * 1000f) + delay + this.Data.ExtraDuration;
        }
        public float MaxTravelTime(Obj_AI_Base target)
        {
            return this.MaxTravelTime(target.ServerPosition.To2D());
        }

        public float TravelTime(Vector2 target)
        {
            var correct = (this.Start.Distance(target) / this.Speed * 1000f) + this.delay;

            if (this.Data.type == Type.CircleMissile)
            {
                correct = CollideEndPosition.Distance(target) / this.Speed * 1000f + this.delay;
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
        public bool Ended
        {
            get
            {
                if (this.Data.SticksToCaster && this.Caster.IsDead)
                {
                    return true;
                }

                if ((this.IsGlobal || this.Data.SticksToMissile) && this.Missile != null && this.Missile.IsDead)
                {
                    return true;
                }

                if (this.Target != null && this.Target.IsDead)
                {
                    return true;
                }

                return Core.GameTickCount - this.EndTick > 0 || this.TicksPassed > 20000;
            }
        }

        public bool IsInside(Obj_AI_Base target)
        {
            var radius = target.BaseSkinName == "JarvanIVStandard" ? 150 : target.BoundingRadius;
            var hitbox = new EloBuddy.SDK.Geometry.Polygon.Circle(target.ServerPosition, radius);
            var predhitbox = new EloBuddy.SDK.Geometry.Polygon.Circle(target.PrediectPosition(TravelTime(target)), radius);
            return hitbox.Points.Any(OriginalPolygon.IsInside) && predhitbox.Points.Any(OriginalPolygon.IsInside);
        }

        public bool WillHit(Obj_AI_Base target, float time = -1f)
        {
            if (!target.IsValidTarget())
                return false;

            time = time.Equals(-1) ? Math.Max(this.TravelTime(target), 0) : time;
            var pred = target.PrediectPosition((int)time);
            return this.WillHit(pred.To2D()) && WillHit(target.ServerPosition.To2D());
        }
        public bool WillHit(Vector2 target)
        {
            if (!target.IsValid())
                return false;

            if (this.Data.type == Type.Cone)
                return this.Polygon != null && this.Polygon.IsInside(target);

            var hitbox = new Geometry.Polygon.Circle(target, Player.Instance.BoundingRadius);
            return this.Polygon != null && hitbox.Points.Any(this.Polygon.IsInside);
        }

        public Vector2 CurrentPosition
        {
            get
            {
                if (this.Data.type == Type.Cone)
                {
                    if (this.Data.IsSpellName("CamilleW") && this.Caster != null)
                    {
                        return Caster.ServerPosition.To2D();
                    }
                    return this.Start;
                }
                if (this.Missile != null)
                {
                    if (this.Data.SticksToMissile)
                    {
                        return this.Missile.GetMissileFixedYPosition().Extend(this.Start, -this.Data.Width / 2f);
                    }
                }
                if (this.Caster != null)
                {
                    if (this.Data.SticksToCaster && Caster.IsHPBarRendered)
                    {
                        return this.Caster.ServerPosition.To2D();
                    }
                }
                if (this.Target != null)
                {
                    if (this.Data.StartsFromTarget)
                    {
                        return this.Target.ServerPosition.To2D();
                    }
                }

                if (this.Data.StaticStart)
                {
                    return this.Start;
                }

                return this.Start;
            }
        }

        public Vector2 EndPosition
        {
            get
            {
                var endpos = Vector2.Zero;

                if (this.Data.EndSticksToTarget && this.Target != null)
                {
                    return this.Target.ServerPosition.To2D();
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
                    if (this.Data.IsSpellName("SionR"))
                    {
                        this.Speed = this.Caster.MoveSpeed;
                        endpos = this.Caster.ServerPosition.Extend(this.Start, -this.Data.Range);
                    }
                    if (this.Data.SticksToCaster)
                    {
                        if (this.Data.IsSpellName("TaricE") || this.Data.IsSpellName("CamilleW"))
                        {
                            return this.Caster.ServerPosition.To2D() + this.Direction * this.Data.Range;
                        }
                    }
                }

                if (this.Missile != null)
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
                var xpoly = new Geometry.Polygon();
                switch (this.SkillshotType)
                {
                    case Type.LineMissile:
                        xpoly = new Geometry.Polygon.Rectangle(CurrentPosition, CollideEndPosition, this.Data.Width + 15 + Player.Instance.BoundingRadius);
                        break;
                    case Type.CircleMissile:
                        xpoly = new Geometry.Polygon.Circle(CollideEndPosition, this.Data.Width + 15 + Player.Instance.BoundingRadius);
                        break;
                    case Type.Cone:
                        xpoly = new Geometry.Polygon.Sector(CurrentPosition, this.CollideEndPosition, (float)((this.Data.Angle + 5) * Math.PI / 180), this.Data.Range + 15);
                        break;
                    case Type.Ring:
                        xpoly = new CustomGeometry.Ring(this.CollideEndPosition, this.Data.Width + 15 + Player.Instance.BoundingRadius, this.Data.RingRadius).ToSDKPolygon();
                        break;
                    case Type.Arc:
                        xpoly = new CustomGeometry.Arc(this.Start, this.CollideEndPosition, 30 + (int)ObjectManager.Player.BoundingRadius).ToSDKPolygon();
                        break;
                }

                if (this.Data.HasExplodingEnd)
                {
                    var explodePolygon = new Geometry.Polygon();
                    var newpolygon = xpoly;

                    if (this.Data.Explodetype == Type.CircleMissile)
                    {
                        explodePolygon = new Geometry.Polygon.Circle(CollideEndPosition, this.Data.ExplodeWidth + 25 + Player.Instance.BoundingRadius);
                    }

                    if (this.Data.Explodetype == Type.LineMissile)
                    {
                        var st = CollideEndPosition - (CollideEndPosition - CurrentPosition).Normalized().Perpendicular() * (this.Data.ExplodeWidth + 25 + Player.Instance.BoundingRadius);
                        var en = CollideEndPosition + (CollideEndPosition - CurrentPosition).Normalized().Perpendicular() * (this.Data.ExplodeWidth + 25 + Player.Instance.BoundingRadius);
                        explodePolygon = new Geometry.Polygon.Rectangle(st, en, (this.Data.ExplodeWidth + 25 + Player.Instance.BoundingRadius) / 2);
                    }

                    if (this.Data.Explodetype == Type.Cone)
                    {
                        var st = CollideEndPosition - Direction * (this.Data.ExplodeWidth * 0.25f);
                        var en = CollideEndPosition + Direction * (this.Data.ExplodeWidth * 3);
                        explodePolygon = new Geometry.Polygon.Sector(st, en, (float)((this.Data.Angle + 5) * Math.PI / 180), this.Data.ExplodeWidth + 20 + ObjectManager.Player.BoundingRadius);
                    }

                    var poly = Geometry.ClipPolygons(new[] { newpolygon, explodePolygon });
                    var vectors = new List<IntPoint>();
                    foreach (var p in poly)
                    {
                        vectors.AddRange(p.ToPolygon().ToClipperPath());
                    }

                    xpoly = vectors.ToPolygon();
                }

                return xpoly;
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
