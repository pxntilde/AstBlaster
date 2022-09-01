using AstBlaster.Utils.Exceptions;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Geo
{
    /// <summary>
    /// Defines a polygon, represented as a set of counterclockwise positions
    /// </summary>
    public class Polygon : IEnumerable<Vector2>
    {
        /// <summary>
        /// Polygon vertices
        /// </summary>
        private List<Vector2> Vertices;

        /// <summary>
        /// Number of sides/vertices
        /// </summary>
        public Int32 Count => Vertices.Count;

        /// <summary>
        /// Polygon area in square units
        /// </summary>
        public Single Area { get; private set; }

        /// <summary>
        /// Center of mass
        /// </summary>
        public Vector2 Centroid { get; private set; }
        
        /// <summary>
        /// Creates a new instance of a <see cref="Polygon"/> from a list of vertices
        /// </summary>
        /// <param name="vertices">List of vertices</param>
        /// <param name="isConvex">Whether the vertices represent a convex shape (optimization)</param>
        /// <param name="isCounterClockwise">Whether the vertices are counterclockwise</param>
        /// <exception cref="ArgumentOutOfRangeError">Too few vertices</exception>
        public Polygon(in List<Vector2> vertices, Boolean isConvex = false, Boolean isCounterClockwise = false)
        {
            if (vertices.Count < 3) throw new ArgumentOutOfRangeError(nameof(vertices), "Too few vertices");
            var hull = isConvex ? vertices : Geometry.ConvexHull2d(vertices.ToArray()).ToList();
            var closed = vertices.First() == vertices.Last();
            if (closed is true && hull.Count < 4) throw new ArgumentOutOfRangeError(nameof(vertices), "Too few vertices");
            if (isCounterClockwise is false) hull.Reverse();
            Vertices = closed is true ? hull.Take(hull.Count - 1).ToList() : hull;
            Area = CalculateArea();
            Centroid = CalculateCentroid();
        }

        #region Getters
        /// <summary>
        /// Returns the vertex <paramref name="index"/> from vertex 0, wrapping
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>Vertex</returns>
        public Vector2 this[Int32 index]
        {
            get => Vertices[index % Count];
        }

        /// <summary>
        /// Returns vertices from <paramref name="first"/> to and including <paramref name="final"/>
        /// </summary>
        /// <param name="first">First vertex</param>
        /// <param name="final">Final vertex</param>
        /// <returns>List of vertices in the range</returns>
        /// <remarks>A positive range will yeild a counterclockwise winding. Multiple walks around the polygon are permitted</remarks>
        public List<Vector2> this[Int32 first, Int32 final]
        {
            get
            {
                List<Vector2> list = new();
                if (first < final)
                {
                    for (var i = 0; i <= final; i++)
                    {
                        list.Add(Vertices[i % Count]);
                    }
                }
                else if (first < final)
                {
                    for (var i = 0; i >= final; i--)
                    {
                        list.Add(Vertices[i % Count]);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a list of <paramref name="count"/> vertices starting with <paramref name="start"/>
        /// </summary>
        /// <param name="start">Starting vertex</param>
        /// <param name="count">Number of vertices</param>
        /// <param name="reverse">If true, the list will walk clockwise from <paramref name="start"/></param>
        /// <returns>List of vertices</returns>
        /// <exception cref="ArgumentOutOfRangeError">Count is not greater than 0</exception>
        /// <remarks>Multiple walks around the polygon are permitted</remarks>
        public List<Vector2> GetVertices(Int32 start, Int32 count, Boolean reverse = false)
        {
            //if (count is <= 0) throw new ArgumentOutOfRangeError(nameof(count), "Must be greater than 0");
            //if (count is 0) return new() { this[start] };
            while (count < 0) count += Count;

            List<Vector2> list = new();
            var direction = reverse is false ? 1 : -1;
            for (var i = 0; i < count; i++)
            {
                var index = start + i * direction;
                list.Add(Vertices[index % Count]);
            }
            return list;
        }

        /// <summary>
        /// Gets the position of the vertex at <paramref name="index"/>
        /// </summary>
        /// <param name="index">Index of vertex</param>
        /// <returns>Vertex position</returns>
        public Vector2 Vertex(Int32 index) => Vertices[index % Count];

        /// <summary>
        /// Returns a side as a line segment between two vertices starting with <paramref name="index"/>
        /// </summary>
        /// <param name="index">Vertex index of first side</param>
        /// <returns>Side as a line segment</returns>
        public LineSegment GetSide(Int32 index) => new(this[index], this[index + 1]);

        /// <summary>
        /// Gets list of consecutive sides
        /// </summary>
        /// <param name="start">Starting vertex</param>
        /// <param name="count">Number of sides</param>
        /// <param name="reverse">Get the sides in reverse order. Side direction will also be reversed</param>
        /// <returns>List of sides</returns>
        /// <exception cref="ArgumentOutOfRangeError">Count must be greater than 0</exception>
        public List<LineSegment> GetSides(Int32 start, Int32 count, Boolean reverse = false)
        {
            if (count is <= 0) throw new ArgumentOutOfRangeError(nameof(count), "Must be greater than 0");
            List<LineSegment> sides = new();
            var direction = reverse is false ? 1 : -1;
            for (var i = 0; i < count; i++)
            {
                var index = start + (i * direction);
                sides.Add(new(this[index], this[index + direction]));
            }
            return sides;
        }

        /// <summary>
        /// Gets list of consecutive sides
        /// </summary>
        /// <param name="reverse">Reverse side order</param>
        /// <returns>List of sides</returns>
        public List<LineSegment> GetSides(Boolean reverse = false) => GetSides(0, Count, reverse);

        #endregion

        #region Setters
        /// <summary>
        /// Scales the polygon by <paramref name="amount"/>
        /// </summary>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentOutOfRangeError">Scale must be positive and non-zero</exception>
        public void ScaleBy(Single amount)
        {
            if (amount is <= 0) throw new ArgumentOutOfRangeError(nameof(amount), "Scale must be positive and non-zero");
            for (var v = 0; v < Count; v++)
            {
                Vertices[v] *= amount;
            }
            Area = Area * amount * amount;
            Centroid = CalculateCentroid();
        }
        #endregion

        #region Private
        /// <summary>
        /// Scales polygon so the furthest vertex is on a unit circle centered on the polygon's centroid
        /// </summary>
        public void Normalize()
        {
            var center = Centroid;

            for(var i = 0; i < Count; i++)
            {
                Vertices[i] -= center;
            }

            var max = 0f;
            foreach(var vertex in Vertices)
            {
                var ls = vertex.Length();
                if (ls > max)
                {
                    max = ls;
                }
            }

            for (var i = 0; i < Count; i++)
            {
                Vertices[i] = Vertices[i] / max + center;
            }
        }
        
        ///// <summary>
        ///// Centers the polygon on the polygon's centroid
        ///// </summary>
        //public void Center()
        //{
        //    var centroid = Centroid;

        //    for (var index = 0; index < Count; index++)
        //    {
        //        Vertices[index] -= centroid;
        //    }
            
        //    Centroid -= centroid;
        //}

        /// <summary>
        /// Calculates the polygon's centroid
        /// </summary>
        /// <returns>Location of centroid</returns>
        private Vector2 CalculateCentroid()
        {
            var sum = Vector2.Zero;

            for (var index = 0; index < Count; index++)
            {
                var v1 = this[index];
                var v2 = this[index + 1];
                sum += (v1 + v2) * (v1.x * v2.y - v2.x * v1.y);
            }

            return sum / (6f * Area);
        }

        /// <summary>
        /// Calculates the area of the polygon
        /// </summary>
        private Single CalculateArea()
        {
            var area = 0f;

            for (var i = 0; i < Count; i++)
            {
                area += (this[i + 1].x - this[i].x) * (this[i + 1].y + this[i].y);
            }

            return Mathf.Abs(area) / 2f;
        }
        #endregion

        /// <summary>
        /// Centers the polygon on the centroid
        /// </summary>
        /// <returns>Applied offset</returns>
        public Vector2 Center()
        {
            var offset = -1f * Centroid;
            for(var index = 0; index < Count; index++)
            {
                Vertices[index] += offset;
            }
            Centroid = Vector2.Zero;
            return offset;
        }

        /// <summary>
        /// Bisects the polygon
        /// </summary>
        /// <param name="index">Index of side to bisect from</param>
        /// <returns>Two polygons</returns>
        public List<Polygon> Bisect(Int32 index)
        {
                    //GD.Print($"v {index}");
            List<Polygon> polygons = new();

            var from = GetSide(index);
            var midpoint = from.Midpoint;
            var direction = from.Direction;

            // Loop from next side to previous side
            for (var i = index + 1; i < index + Count; i++)
            {
                if (i == index)
                {
                    GD.Print($"Too far");
                    continue;
                }

                var oppositeSide = GetSide(i);
                //GD.Print($"Checking side {i}");
                // Check if it is the opposite side
                if (direction.Dot(oppositeSide.Start.DirectionTo(midpoint)) > 0f !=
                    direction.Dot(oppositeSide.End.DirectionTo(midpoint)) > 0f)
                {
                    //GD.Print($"Side {i} intersects");

                    var intersection = (Vector2)Geometry.LineIntersectsLine2d(oppositeSide.Start, oppositeSide.Direction, midpoint, from.Normal);
                    
                    List<Vector2> polyA = new();

                    foreach (var aVertex in GetVertices(index + 1, i - (index + 1) + 1))
                    {
                        polyA.Add(aVertex);
                       // GD.Print($"{aVertex}");
                    }

                    polyA.Add(intersection);
                    polyA.Add(midpoint);
                   // GD.Print($"{intersection}");
                   // GD.Print($"{midpoint}");

                    List<Vector2> polyB = new();

                    foreach (var bVertex in GetVertices(i + 1, index - (i + 1) + 1))
                    {
                        polyB.Add(bVertex);
                    }

                    polyB.Add(midpoint);
                    polyB.Add(intersection);

                    polygons.Add(new(polyA, true, true));
                    polygons.Add(new(polyB, true, true));

                    break;
                }
            }
                    //GD.Print($"^");

            //if (to is null)
            //{
            //    GD.Print("You fucked up");
            //    return null;
            //}

            //var startIndex = Vertices.IndexOf(from.End);
            //var endIndex = Vertices.IndexOf(to.Start);
            //if (endIndex < startIndex) endIndex += Count;

            //List<Vector2> poly = GetVertices(startIndex, endIndex - startIndex);
            //GD.Print($"from {Vertices.IndexOf(from.End)} {from.End} to {Vertices.IndexOf(to.Start)} {to.Start} ");
            //poly.Insert(0, from.Midpoint);
            //poly.Add(to.Midpoint);

            //polygons.Add(new(poly, true, true));

            //startIndex = Vertices.IndexOf(to.End);
            //endIndex = Vertices.IndexOf(from.Start);
            //if (endIndex < startIndex) endIndex += Count;

            //poly = GetVertices(startIndex, endIndex);
            //poly.Insert(0, to.Midpoint);
            //poly.Add(from.Midpoint);

            //polygons.Add(new(poly, true, true));

            return polygons;
        }

        #region IEnumerable
        public IEnumerator<Vector2> GetEnumerator() => Vertices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
