using AstBlaster.Entities.Ship;
using AstBlaster.Geo;
using AstBlaster.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    public class DebugAsteroid : Asteroid, IDebugDraw
    {
        public DebugAsteroid() : base() { }//Mass = Single.MaxValue; }

        private Vector2 LastHitPosition = Vector2.Zero;
        private LineSegment LastHitSide;

        public override void _PhysicsProcess(Single delta)
        {
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;
        }

        public override void _Draw()
        {
            DrawColoredPolygon(CachedShape, Colors.SaddleBrown, null, null, null, true);
            
            foreach(var circle in Circles.Values)
            {
                DrawCircle(circle.Position, circle.Radius, circle.Color);
            }

            foreach(var line in Lines.Values)
            {
                DrawLine(line.From, line.To, line.Color, 1.5f, true);
            }
        }

        public override void ApplyDamage(IDamager instigator, Single amount, Vector2 position)
        {
            position = ToLocal(position);
            LastHitPosition = position;

            Int32 targetIndex = 0;
            Single targetDistace = Single.PositiveInfinity;
            var sides = Polygon.GetSides();

            for (var i = 0; i < sides.Count; i++)
            {
                var distance = position.DistanceSquaredTo(sides[i].Midpoint);
                if (distance < targetDistace)
                {
                    targetDistace = distance;
                    targetIndex = i;
                }
            }

            LastHitSide = sides[targetIndex];

            var from = LastHitSide;
            LineSegment to = null;

            foreach (var side in Polygon.GetSides())
            {
                if (side != from && Geometry.SegmentIntersectsSegment2d(from.Midpoint, Mathf.Max(from.Midpoint.DistanceSquaredTo(side.Start), from.Midpoint.DistanceSquaredTo(side.End)) * -1f * from.Normal, side.Start, side.End) is not null)
                {
                    to = side;
                    //OppositeSide = to;
                    IntersectA = new(from.Midpoint, from.Midpoint - 2f * from.Midpoint.Length() * from.Normal);
                    IntersectB = new(side.Start, side.End);

                    break;
                }
            }

           // for (var i = 0; i <= sides.Count; i++)
        }

        private List<LineSegment> segs = new();

        private LineSegment IntersectA = null;
        private LineSegment IntersectB = null;
        private Vector2 OppositeCorner = Vector2.Zero;

        public Dictionary<UInt64, DebugLine> Lines { get; } = new();
        public Dictionary<UInt64, DebugCircle> Circles { get; } = new();
    }
}
