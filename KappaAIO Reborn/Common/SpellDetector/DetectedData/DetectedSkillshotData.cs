using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using KappAIO_Reborn.Common.Databases.SpellData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;
using KappAIO_Reborn.Common.SpellDetector.Events;
using KappAIO_Reborn.Common.Utility;
using SharpDX;
using Type = KappAIO_Reborn.Common.Databases.SpellData.Type;

namespace KappAIO_Reborn.Common.SpellDetector.DetectedData
{
    public class DetectedSkillshotData
    {
        public void Update()
        {
            this.CheckCollision();
            
            var exPolygon = this.generatePolygon(Player.Instance.BoundingRadius + 35);

            this.EvadePolygon = exPolygon;
            this.OriginalPolygon = this.generatePolygon2();
            this.DrawingPolygon = this.generatePolygon();
            OnSkillShotUpdate.Invoke(this);
        }

        private void CheckCollision()
        {
            if (this.Data.Collisions == null)
                return;

            this.CollidePoint = null;
            this.CorrectCollidePoint = null;
            
            var check = new CollisionResult(this);
            this.CorrectCollidePoint = check.CorrectCollidePoint;
            this.CollidePoint = check.CollidePoint;
        }

        public Obj_AI_Base Caster = null, Target = null, Trap = null;
        public Obj_AI_Base BuffHolder
        {
            get
            {
                return this.Data.RequireBuffs == null ? null : EntityManager.Heroes.AllHeroes.OrderBy(h => h.Distance(this.End)).FirstOrDefault(this.Data.HasBuff);
            }
        }
        public MissileClient Missile = null;
        public Obj_GeneralParticleEmitter Particle = null;
        public SkillshotData Data;

        public Vector2 Center => (this.CurrentPosition + this.CollideEndPosition) / 2;
        public Vector2 Start, End;
        public Vector2 Direction => (this.End - this.Start).Normalized();
        public Vector2 Direction2 => (this.EndPosition - this.Start).Normalized();
        public Vector2? CorrectCollidePoint = null;
        public Vector2? CollidePoint = null;

        public Vector2 CurrentPosition
        {
            get
            {
                if (this.Data.StaticStart)
                    return this.Start;

                if (this.Data.SticksToCaster && this.Caster != null)
                {
                    return this.Caster.ServerPosition.To2D();
                }
                if (this.Data.SticksToMissile && this.Missile != null)
                {
                    return this.Missile.Position.Extend(this.Start, -(this.Width / 2f));
                }
                if (this.Data.StartsFromTarget && this.Target != null)
                {
                    return this.Target.ServerPosition.Extend(this.Start, -this.Target.BoundingRadius);
                }

                return this.CalculatedPosition();
            }
        }

        public Vector2 EndPosition
        {
            get
            {
                if (this.Data.StaticEnd)
                    return this.End;

                if (this.IsGlobal && this.Missile != null && this.Data.type == Type.LineMissile)
                {
                    return this.Missile.Position.To2D().Extend(this.Missile.Position.To2D() + this.Direction, this.Range);
                }

                if (this.Data.EndIsBuffHolderPosition && this.BuffHolder != null)
                {
                    var dashInfo = this.BuffHolder.GetDashInfo();
                    return (dashInfo?.EndPos ?? this.BuffHolder.ServerPosition).To2D();
                }

                if (this.Data.EndSticksToTarget && this.Target != null)
                {
                    return this.Target.ServerPosition.To2D();
                }

                var end = Vector2.Zero;

                if (this.Caster != null)
                {
                    if (this.Data.EndSticksToCaster)
                    {
                        return this.Caster.ServerPosition.To2D();
                    }
                    if (this.Data.SticksToCaster && this.Data.IsMoving)
                    {
                        return this.Caster.ServerPosition.To2D() + this.Direction * this.Range;
                    }
                    var lastPath = this.Caster.Path.LastOrDefault().To2D();
                    var direction = (this.Caster.Direction().Distance(this.Caster) < 50 ? this.Caster.Direction() : lastPath);
                    if (this.Data.EndIsCasterDirection)
                    {
                        end = this.Data.IsCasterName("Riven") ? lastPath : direction;
                    }
                    if (this.Data.IsCasterName("Sion") && this.Data.IsSlot(SpellSlot.R))
                    {
                        this.Speed = this.Caster.MoveSpeed;
                        end = this.Caster.ServerPosition.Extend(this.Start, -this.Range);
                    }
                    if (this.Data.EndStickToDirection)
                    {
                        return this.CurrentPosition.Extend(this.Direction, this.Range);
                    }
                }

                if (this.Missile != null && this.Data.EndSticksToMissile)
                {
                    return this.Missile.Position.To2D();
                }

                if (end.IsZero)
                    end = this.End;

                var result = this.Data.IsFixedRange ? this.Start.Extend(end, this.Range) : end.Extend(this.Start, -this.ImpactRange);

                if (this.Start.Distance(result) > this.Range)
                    result = this.Start.Extend(result, this.Range);

                return result;
            }
        }

        public Vector2 CollideEndPosition
        {
            get
            {
                return this.CollidePoint ?? this.EndPosition;
            }
        }

        public Vector2 CalculatedPosition(float aftertime = 0)
        {
            var time = Math.Max(0, this.TicksPassed - this.CastDelay);
            int x;

            if (this.Data.MissileAccel.Equals(0f))
                x = (int)(time * this.Speed / 1000f);
            else
            {
                var time1 = (this.Data.MissileAccel > 0f ? this.Data.MissileMaxSpeed : this.Data.MissileMinSpeed - this.Speed) * 1000f / this.Data.MissileAccel;

                if (time <= time1)
                {
                    x = (int)(time * this.Speed / 1000f + 0.5f * this.Data.MissileAccel * Math.Pow(time / 1000f, 2f));
                }
                else
                {
                    x = (int)(time1 * this.Speed / 1000f + 0.5f * this.Data.MissileAccel * Math.Pow(time1 / 1000f, 2f)
                         + (time - time1) / 1000f * (this.Data.MissileAccel < 0f ? this.Data.MissileMaxSpeed : this.Data.MissileMinSpeed));
                }
            }

            var end = this.CollidePoint ?? this.EndPosition;

            time = (int)Math.Max(0, Math.Min(end.Distance(this.Start), x));
            return (this.Start + this.Direction * time);
        }

        public bool DetectedByMissile, FromFOW;
        public bool IsEnemy => this.Caster != null && this.Caster.IsEnemy;
        public bool IsOnScreen => this.DrawingPolygon != null && this.DrawingPolygon.Points.Any(p => p.To3DWorld().IsOnScreen()) || this.Center.To3DWorld().IsOnScreen();
        public bool CollideExplode => this.CorrectCollidePoint.HasValue && this.Data.CollideExplode;
        public bool ExplodeEnd => this.CollidePoint.HasValue && this.Data.HasExplodingEnd;
        public bool IsGlobal
        {
            get
            {
                return this.Data.Range >= 4000 && this.Data.Range < 15000;
            }
        }
        public bool WillHit(Vector2 target) => this.EvadePolygon != null && this.EvadePolygon.IsInside(target);
        public bool WillHit(Obj_AI_Base target) => target.IsValidTarget() && this.EvadePolygon != null && this.EvadePolygon.IsInside(target);
        public bool IsSafe(Vector2 pos)
        {
            return !this.WillHit(pos);
        }
        public bool IsInside(Vector2 target) => target.IsValid() && this.OriginalPolygon.IsInside(target);
        public bool IsInside(Obj_AI_Base target) => target.IsValidTarget() && this.generatePolygon2(target.BoundingRadius).IsInside(target);
        private bool? _ended;
        public bool Ended
        {
            get
            {
                if (this._ended.HasValue && this._ended.Value)
                    return true;

                if ((this.Data.SticksToCaster || this.Data.EndSticksToCaster) && this.Caster.IsDead)
                {
                    return true;
                }

                if ((this.IsGlobal || (this.Data.SticksToMissile || this.Data.EndSticksToMissile)) && this.Missile != null && this.Missile.IsDead)
                {
                    return true;
                }

                if (this.Target != null && this.Target.IsDead)
                {
                    return true;
                }

                if (this.Data.IsTrap && this.Trap != null && this.Trap.IsDead)
                {
                    return true;
                }

                return Core.GameTickCount - this.EndTick > 0 || (this.TicksPassed > 30000 && !this.Data.IsTrap);
            }
            set
            {
                this._ended = value;
            }
        }
        public bool AboutToHit(Obj_AI_Base target, int delay)
        {
            return target.IsValidTarget() && this.AboutToHit(target.ServerPosition.To2D(), delay);
        }
        public bool AboutToHit(Vector2 target, int delay = 100)
        {
            var travelTime = this.TravelTime(target);

            return WillHit(target) && travelTime < delay;
        }
        public bool IsAboutToHit(int time, Obj_AI_Base unit)
        {
            if (this.Target == null)
                return false;

            time += Game.Ping / 2;
            return WillHit(this.Target.ServerPosition.To2D()) && time >= this.TravelTime(unit);
        }

        public float StartTick = Core.GameTickCount;
        public float? FixedEndTick = null;
        public float EndTick => this.FixedEndTick ?? this.StartTick + this.MaxTravelTime(this.CollideEndPosition);
        public float TicksLeft => this.EndTick - Core.GameTickCount;
        public float TicksPassed => Core.GameTickCount - this.StartTick;
        public float extraDelay = 0;
        public float CastDelay
        {
            get
            {
                var result = 0f;
                if (!this.DetectedByMissile || this.Data.type != Type.LineMissile || (this.Speed <= 0 || this.Speed >= int.MaxValue))
                {
                    result += this.Data.CastDelay;
                }

                if (!Data.DontAddExtraDuration)
                {
                    result += Data.ExtraDuration;
                }

                return result + extraDelay;
            }
        }
        private float? _speed = null;
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
        private float extraValue => 0;
        private float? _range = null;
        private float? _width = null;
        public float Range
        {
            get
            {
                try
                {
                    return (this._range ?? this.Data.Range) + this.extraValue + this.ImpactRange;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong Range Skillshot ID: {this.Data.MenuItemName} {(this.Particle != null ? "Particle: " + Particle.Name : "")}");
                    return this.Data.Range;
                }
            }
            set
            {
                this._range = value;
            }
        }
        public float ImpactRange
        {
            get
            {
                try
                {
                    return this.Data.ExtraRange + (this.extraValue / 2f);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong ExtraRange Skillshot ID: {this.Data.MenuItemName}");
                    return this.Data.ExtraRange;
                }
            }
        }
        public float Width
        {
            get
            {
                try
                {
                    return _width ?? this.Data.Width + this.extraValue;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong Width Skillshot ID: {this.Data.MenuItemName}");
                    return this.Data.Width;
                }
            }
            set
            {
                this._width = value;
            }
        }
        public float RingRadius
        {
            get
            {
                try
                {
                    return this.Data.RingRadius;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong RingRadius Skillshot ID: {this.Data.MenuItemName}");
                    return this.Data.RingRadius;
                }
            }
        }
        public float Angle
        {
            get
            {
                try
                {
                    return this.Data.Angle;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong Angle Skillshot ID: {this.Data.MenuItemName}");
                    return this.Data.Angle;
                }
            }
        }
        public float ExplodeWidth
        {
            get
            {
                try
                {
                    return this.Data.ExplodeWidth;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Wrong ExplodeWidth Skillshot ID: {this.Data.MenuItemName}");
                    return this.Data.ExplodeWidth;
                }
            }
        }
        public float MaxTravelTime(Vector2 target)
        {
            return (this.Start.Distance(target) / this.Speed * 1000f) + this.Data.CastDelay + this.Data.ExtraDuration;
        }
        public float MaxTravelTime(Obj_AI_Base target)
        {
            return this.MaxTravelTime(target.ServerPosition.To2D());
        }
        public float TravelTime(Vector2 target)
        {
            var correct = ((this.Start.Distance(target) / this.Speed) * 1000f) + this.CastDelay;
            if (this.Data.type == Type.CircleMissile)
                correct = (this.CollideEndPosition.Distance(target) / this.Speed) * 1000f + this.CastDelay;

            return correct - this.TicksPassed;
        }
        public float TravelTime(Obj_AI_Base target)
        {
            return this.TravelTime(target.ServerPosition.To2D());
        }
        public float CurrentTravelTime(Vector2 pos)
        {
            return CurrentPosition.Distance(pos) / Speed * 1000f + CastDelay;
        }
        public float GetHealthPrediction(Obj_AI_Base target)
        {
            return Prediction.Health.GetPrediction(target, (int)Math.Max(1, CurrentTravelTime(target.ServerPosition.To2D()) - Game.Ping));
        }
        public float GetSpellDamage(Obj_AI_Base target)
        {
            if (target == null || this.Caster == null || this.Data.Slots == null || !this.Data.Slots.Any())
                return 0;

            var hero = this.Caster as AIHeroClient;
            return hero?.GetSpellDamage(target, this.Data.Slots[0]) ?? 0;
        }

        public Geometry.Polygon EvadePolygon, ExplodePolygon, OriginalPolygon, DrawingPolygon;
        private Geometry.Polygon generatePolygon(float extraWidth = 0)
        {
            var extraAngle = Math.Max(1, Math.Max(1, extraWidth) / 5);
            extraWidth += this.Width;

            Geometry.Polygon polygon = null;
            switch (this.Data.type)
            {
                case Type.LineMissile:
                    polygon = new Geometry.Polygon.Rectangle(this.CurrentPosition, this.CollidePoint == null && extraWidth > Width ? this.CollideEndPosition.Extend(this.CurrentPosition, -(extraWidth * 0.75f)) : this.CollideEndPosition, extraWidth);
                    break;
                case Type.CircleMissile:
                    polygon = new Geometry.Polygon.Circle(this.Data.IsMoving ? this.CurrentPosition : this.CollideEndPosition, extraWidth);
                    break;
                case Type.Cone:
                    polygon = new Geometry.Polygon.Sector(this.CurrentPosition, this.CollideEndPosition, (float)((this.Angle + extraAngle) * Math.PI / 180), this.Range);
                    break;
                case Type.Arc:
                    polygon = new CustomGeometry.Arc(this.Start, this.CollideEndPosition, (int)extraWidth).ToSDKPolygon();
                    break;
                case Type.Ring:
                    polygon = new CustomGeometry.Ring(this.CollideEndPosition, extraWidth, this.RingRadius).ToSDKPolygon();
                    break;
            }

            if (polygon != null && (this.ExplodeEnd || this.CollideExplode))
            {
                var newpolygon = polygon;
                var pos = this.CurrentPosition;
                var collidepoint = this.CollideExplode ? this.CorrectCollidePoint.GetValueOrDefault() : this.CollideEndPosition;
                switch (this.Data.Explodetype)
                {
                    case Type.CircleMissile:
                        this.ExplodePolygon = new Geometry.Polygon.Circle(collidepoint, this.ExplodeWidth);
                        break;
                    case Type.LineMissile:
                        var st = collidepoint - (collidepoint - pos).Normalized().Perpendicular() * (this.ExplodeWidth);
                        var en = collidepoint + (collidepoint - pos).Normalized().Perpendicular() * (this.ExplodeWidth);
                        this.ExplodePolygon = new Geometry.Polygon.Rectangle(st, en, this.ExplodeWidth / 2);
                        break;
                    case Type.Cone:
                        var st2 = collidepoint - Direction * (this.ExplodeWidth * 0.25f);
                        var en2 = collidepoint + Direction * (this.ExplodeWidth * 3);
                        this.ExplodePolygon = new Geometry.Polygon.Sector(st2, en2, (float)(this.Angle * Math.PI / 180), this.ExplodeWidth);
                        break;
                }

                var poly = Geometry.ClipPolygons(new[] { newpolygon, this.ExplodePolygon });
                var vectors = new List<IntPoint>();
                foreach (var p in poly)
                    vectors.AddRange(p.ToPolygon().ToClipperPath());

                polygon = vectors.ToPolygon();
            }

            return polygon;
        }
        public Geometry.Polygon generatePolygon2(float extraWidth = 0) // no collision end
        {
            extraWidth += this.Width;
            switch (this.Data.type)
            {
                case Type.LineMissile:
                    return new Geometry.Polygon.Rectangle(this.CurrentPosition, this.EndPosition, extraWidth);
                case Type.CircleMissile:
                    return new Geometry.Polygon.Circle(this.Data.IsMoving ? this.CurrentPosition : this.EndPosition, extraWidth);
                case Type.Cone:
                    return new Geometry.Polygon.Sector(this.CurrentPosition, this.EndPosition, (float)(this.Angle * Math.PI / 180), this.Range);
                case Type.Arc:
                    return new CustomGeometry.Arc(this.Start, this.CollideEndPosition, (int)extraWidth).ToSDKPolygon();
                case Type.Ring:
                    return new CustomGeometry.Ring(this.CollideEndPosition, extraWidth, this.RingRadius).ToSDKPolygon();
            }

            return null;
        }
        public Geometry.Polygon CollisionPolygon
        {
            get
            {
                return new Geometry.Polygon.Rectangle(this.CurrentPosition, this.EndPosition, Math.Max(this.Data.Width, this.Data.Angle));
            }
        }
        public Geometry.Polygon PredictedPolygon(float afterTime, float extraWidth = 0)
        {
            var extraAngle = Math.Max(1, Math.Max(1, extraWidth) / 4);
            extraWidth += this.Width;

            Geometry.Polygon polygon = null;
            switch (this.Data.type)
            {
                case Type.LineMissile:
                    polygon = new Geometry.Polygon.Rectangle(this.CalculatedPosition(afterTime), this.CollideEndPosition, extraWidth);
                    break;
                case Type.CircleMissile:
                    polygon = new Geometry.Polygon.Circle(this.Data.IsMoving ? this.CalculatedPosition(afterTime) : this.CollideEndPosition, extraWidth);
                    break;
                case Type.Cone:
                    polygon = new Geometry.Polygon.Sector(this.CurrentPosition, this.CollideEndPosition, (float)((this.Angle + extraAngle) * Math.PI / 180), this.Range);
                    break;
                case Type.Arc:
                    polygon = new CustomGeometry.Arc(this.Start, this.CollideEndPosition, (int)extraWidth).ToSDKPolygon();
                    break;
                case Type.Ring:
                    polygon = new CustomGeometry.Ring(this.CollideEndPosition, extraWidth, this.RingRadius).ToSDKPolygon();
                    break;
            }

            if (polygon != null && (this.ExplodeEnd || this.CollideExplode))
            {
                var newpolygon = polygon;
                var pos = this.CurrentPosition;
                var collidepoint = this.CollideExplode ? this.CorrectCollidePoint.GetValueOrDefault() : this.CollideEndPosition;
                switch (this.Data.Explodetype)
                {
                    case Type.CircleMissile:
                        this.ExplodePolygon = new Geometry.Polygon.Circle(collidepoint, this.ExplodeWidth);
                        break;
                    case Type.LineMissile:
                        var st = collidepoint - (collidepoint - pos).Normalized().Perpendicular() * (this.ExplodeWidth);
                        var en = collidepoint + (collidepoint - pos).Normalized().Perpendicular() * (this.ExplodeWidth);
                        this.ExplodePolygon = new Geometry.Polygon.Rectangle(st, en, this.ExplodeWidth / 2);
                        break;
                    case Type.Cone:
                        var st2 = collidepoint - Direction * (this.ExplodeWidth * 0.25f);
                        var en2 = collidepoint + Direction * (this.ExplodeWidth * 3);
                        this.ExplodePolygon = new Geometry.Polygon.Sector(st2, en2, (float)(this.Angle * Math.PI / 180), this.ExplodeWidth);
                        break;
                }

                var poly = Geometry.ClipPolygons(new[] { newpolygon, this.ExplodePolygon });
                var vectors = new List<IntPoint>();
                foreach (var p in poly)
                    vectors.AddRange(p.ToPolygon().ToClipperPath());

                polygon = vectors.ToPolygon();
            }

            return polygon;
        }
    }
}
