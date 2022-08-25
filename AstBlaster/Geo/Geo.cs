using AstBlaster.Utils.Exceptions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Geo
{
    /// <summary>
    /// Geometry functions
    /// </summary>
    public static class Geo
    {
        /// <summary>
        /// Creates a list of ordered positive and negative numbers that sum to 0 where each segment is in the range [0, <paramref name="scale"/>)
        /// </summary>
        /// <param name="segments">Number of segments in the loop</param>
        /// <param name="scale">Scale factor for the range</param>
        /// <param name="random">RNG</param>
        /// <returns></returns>
        public static List<Single> Random1DLoop(Int32 segments, Single scale, Random random = null)
        {
            if (segments < 2) throw new ArgumentOutOfRangeError(nameof(segments), "Must be two or more");
            if (random == null) random = new();

            var set = new HashSet<Single>();
            var path = new List<Single>();

            foreach (var _ in Enumerable.Range(0, segments))
            {
                while (set.Add((Single)random.NextDouble()) is false) ;
            }

            var min = set.Min();
            var max = set.Max();

            set.Remove(min);
            set.Remove(max);

            var pos = min;
            var neg = min;

            foreach (var num in set)
            {
                if (random.Next() % 2 == 0)
                {
                    path.Add(num - pos);
                    pos = num;
                }
                else
                {
                    path.Add(neg - num);
                    neg = num;
                }
            }

            path.Add(max - pos);
            path.Add(neg - max);

            path = path.Select(s => s / (max - min) * scale).ToList();

            return path;
        }

        /// <summary>
        /// Generates a random polygon with <paramref name="sides"/> sides
        /// </summary>
        /// <param name="sides">Number of sides</param>
        /// <param name="random">RNG</param>
        /// <returns>Polygon</returns>
        /// <exception cref="ArgumentOutOfRangeException">Sides must be greater than 3</exception>
        public static Polygon RandomPolygon(Int32 sides = 3, Random random = null)
        {
            if (sides < 3) throw new ArgumentOutOfRangeError(nameof(sides), "Sides must be 3 or greater");
            if (random is null) random = new();

            List<Vector2> segments = new();

            var xs = Random1DLoop(sides, 1f, random);
            var ys = Random1DLoop(sides, 1f, random);

            while (xs.Count > 0)
            {
                var x = xs[random.Next(xs.Count)];
                var y = ys[random.Next(ys.Count)];
                xs.Remove(x);
                ys.Remove(y);
                segments.Add(new Vector2(x, y));
            }

            // order them counterclockwise

            segments = segments.OrderByDescending(v => v.AngleTo(Vector2.Up)).ToList();

            // Put them end to end

            List<Vector2> polygon = new();
            polygon.Add(segments[0]);
            for (var i = 1; i < segments.Count; i++)
            {
                polygon.Add(segments[i] + polygon.Last());
            }

            // Center the polygon and scale it to radius

            var midpoint = new Vector2(
                polygon.Select(v => v.x).Min() + ((polygon.Select(v => v.x).Max() - polygon.Select(v => v.x).Min()) / 2f),
                polygon.Select(v => v.y).Min() + ((polygon.Select(v => v.y).Max() - polygon.Select(v => v.y).Min()) / 2f));
            //var factor = polygon.Max(v => (v - midpoint).Length() / 1f /* radius */);
            //polygon = polygon.Select(v => (v - midpoint) / factor).ToList();
            polygon = polygon.Select(v => v - midpoint).ToList();

            return new Polygon(polygon, true, false);
        }
    }
}
