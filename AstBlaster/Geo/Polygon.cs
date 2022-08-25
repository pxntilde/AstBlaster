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
        private Int32 Count => Vertices.Count;
        
        /// <summary>
        /// Creates a new instance of a <see cref="Polygon"/> from a list of vertices
        /// </summary>
        /// <param name="vertices">List of vertices</param>
        /// <param name="isConvex">Whether the vertices represent a convex shape (optimization)</param>
        /// <param name="isCounterClockwise">Whether the vertices are counterclockwise</param>
        /// <exception cref="ArgumentOutOfRangeError">Too few vertices</exception>
        public Polygon(in List<Vector2> vertices, Boolean isConvex = false, Boolean isCounterClockwise = false)
        {
            var closed = vertices.First() == vertices.Last();
            if (vertices.Count < (closed is true ? 4 : 3)) throw new ArgumentOutOfRangeError(nameof(vertices), "Too few vertices");
            var hull = isConvex ? vertices : Geometry.ConvexHull2d(vertices.ToArray()).ToList();
            if (isCounterClockwise) hull.Reverse();
            Vertices = closed is true ? hull.Take(hull.Count - 1).ToList() : hull;
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
            if (count is <= 0) throw new ArgumentOutOfRangeError(nameof(count), "Must be greater than 0");
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

        #endregion

        #region Setters
        public void ScaleBy(Single amount)
        {
            for (var v = 0; v < Count; v++)
            {
                Vertices[v] *= amount;
            }
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Vector2> GetEnumerator() => Vertices.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
