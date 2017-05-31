using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace KappAIO_Reborn.Common.Utility
{
    public class CustomGeometry
    {
        public class Arc : Geometry.Polygon
        {
            public Vector2 Start { get; }
            public Vector2 End { get; }

            public int HitBox { get; }
            private float Distance { get; }

            public Arc(Vector2 start, Vector2 end, int hitbox)
            {
                this.Start = start;
                this.End = end;
                this.HitBox = hitbox;
                this.Distance = this.Start.Distance(this.End);
            }

            public Geometry.Polygon ToSDKPolygon(int offset = 0)
            {
                offset += this.HitBox;
                var result = new Geometry.Polygon();

                var innerRadius = -0.1562f * this.Distance + 687.31f;
                var outerRadius = 0.35256f * this.Distance + 133f;

                outerRadius = outerRadius / (float)Math.Cos(2 * Math.PI / 22);

                var innerCenters = CircleCircleIntersection(this.Start, this.End, innerRadius, innerRadius);
                var outerCenters = CircleCircleIntersection(this.Start, this.End, outerRadius, outerRadius);

                var innerCenter = innerCenters[0];
                var outerCenter = outerCenters[0];

                var direction = (this.End - outerCenter).Normalized();
                var end = (this.Start - outerCenter).Normalized();
                var maxAngle = (float)(direction.AngleBetween(end) * Math.PI / 180);

                var step = -maxAngle / 22;
                for (var i = 0; i < 22; i++)
                {
                    var angle = step * i;
                    var point = outerCenter + (outerRadius + 15 + offset) * direction.Rotated(angle);
                    result.Add(point);
                }

                direction = (this.Start - innerCenter).Normalized();
                end = (this.End - innerCenter).Normalized();
                maxAngle = (float)(direction.AngleBetween(end) * Math.PI / 180);
                step = maxAngle / 22;
                for (var i = 0; i < 22; i++)
                {
                    var angle = step * i;
                    var point = innerCenter + Math.Max(0, innerRadius - offset - 100) * direction.Rotated(angle);
                    result.Add(point);
                }

                return result;
            }
        }

        public class Ring : Geometry.Polygon
        {
            public Vector2 Center;
            public float Radius;
            public float RingRadius;

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                this.Center = center;
                this.Radius = radius;
                this.RingRadius = ringRadius;
            }

            public Geometry.Polygon ToSDKPolygon(int offset = 0)
            {
                var result = new Geometry.Polygon();

                var outRadius = (offset + this.Radius + this.RingRadius) / (float)Math.Cos(2 * Math.PI / 22);
                var innerRadius = this.Radius - this.RingRadius - offset;

                for (var i = 0; i <= 22; i++)
                {
                    var angle = i * 2 * Math.PI / 22;
                    var point = new Vector2(this.Center.X - outRadius * (float)Math.Cos(angle), this.Center.Y - outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                for (var i = 0; i <= 22; i++)
                {
                    var angle = i * 2 * Math.PI / 22;
                    var point = new Vector2(this.Center.X + innerRadius * (float)Math.Cos(angle), this.Center.Y - innerRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }
        }

        public class Sector2 : Geometry.Polygon
        {
            public float Angle;
            public Vector2 Center;
            public Vector2 Direction;
            public float Radius;
            private readonly int _quality;

            public Sector2(Vector3 center, Vector3 direction, float angle, float radius, int quality = 20)
                : this(center.To2D(), direction.To2D(), angle, radius, quality)
            {
            }

            public Sector2(Vector2 center, Vector2 direction, float angle, float radius, int quality = 20)
            {
                Center = center;
                Direction = (direction - center).Normalized();
                Angle = angle;
                Radius = radius;
                _quality = quality;
                UpdatePolygon();
            }

            public void UpdatePolygon(int offset = 0)
            {
                Points.Clear();
                var outRadius = (Radius + offset) / (float)Math.Cos(2 * Math.PI / _quality);
                var angle2 = (Angle + (float)(30f * Math.PI / 180f));
                var side1 = Direction.Rotated(-Angle * 0.5f);
                var side2 = Direction.Rotated(-angle2 * 3.5f);
                var lq = this._quality / 2;
                for (var i = 0; i <= _quality; i++)
                {
                    var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                    Points.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
                }
                var start = this.Center.Extend(this.Center + this.Direction, -150);
                for (var i = 0; i <= lq; i++)
                {
                    var cDirection = side2.Rotated(-i * angle2 / lq).Normalized();
                    Points.Add(new Vector2(start.X + 300 * cDirection.X, start.Y + 300 * cDirection.Y));
                }
            }
        }

        public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var D = center1.Distance(center2);
            if (D > radius1 + radius2 || (D <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var A = (radius1 * radius1 - radius2 * radius2 + D * D) / (2 * D);
            var H = (float)Math.Sqrt(radius1 * radius1 - A * A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A * Direction;
            var S1 = PA + H * Direction.Perpendicular();
            var S2 = PA - H * Direction.Perpendicular();
            return new[] { S1, S2 };
        }
    }
}
