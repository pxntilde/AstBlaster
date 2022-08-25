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
        /// Creates a list of ordered positive and negative numbers that sum to 0 where each segment is in the range [0, <paramref name="size"/>)
        /// </summary>
        /// <param name="segments">Number of segments in the loop</param>
        /// <param name="size">breadth of the loop</param>
        /// <param name="random">RNG</param>
        /// <returns></returns>
        public static List<Single> Random1DLoop(Int32 segments, Single size = 1f, Random random = null)
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

            // Create forward and backward path

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

            // Scale to size

            var width = path.Sum(p => Mathf.Abs(p) / 2f);
            path = path.Select(s => s / width * size).ToList();

            return path;
        }

        /// <summary>
        /// Generates a random polygon with <paramref name="sides"/> sides and a radius of <paramref name="radius"/>
        /// </summary>
        /// <param name="sides">Number of sides</param>
        /// <param name="radius">Distance of maximum vertex</param>
        /// <param name="random">RNG</param>
        /// <returns>Polygon</returns>
        /// <exception cref="ArgumentOutOfRangeException">Sides must be greater than 3</exception>
        public static Polygon RandomPolygon(Int32 sides = 3, Single radius = 1f, Random random = null)
        {
            if (sides < 3) throw new ArgumentOutOfRangeError(nameof(sides), "Sides must be 3 or greater");
            if (random is null) random = new();

            List<Vector2> segments = new();

            // Make vertices that lie on a unit circle

            var xs = Random1DLoop(sides, 2f, random);
            var ys = Random1DLoop(sides, 2f, random);

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
            polygon.Add(segments[0]);// + new Vector2(1f, 0f));
            for (var i = 1; i < segments.Count; i++)
            {
                polygon.Add(segments[i] + polygon.Last());
            }

            var instance = new Polygon(polygon, true, true);
            instance.Center();
            instance.Normalize();
            instance.ScaleBy(radius);
            return instance;
        }
    }
}
