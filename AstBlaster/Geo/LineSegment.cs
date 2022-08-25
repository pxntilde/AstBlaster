using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Geo
{
    /// <summary>
    /// A line segment represented by it's start and end point
    /// </summary>
    public record LineSegment
    {
        /// <summary>
        /// Creates a new line segment
        /// </summary>
        /// <param name="start">Starting position</param>
        /// <param name="end">End position</param>
        public LineSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Start of segment
        /// </summary>
        public Vector2 Start { get; private set; }

        /// <summary>
        /// End of segment
        /// </summary>
        public Vector2 End { get; private set; }

        /// <summary>
        /// Segment midpoint
        /// </summary>
        public Vector2 Midpoint => 0.5f * (Start + End);

        /// <summary>
        /// Normal vector (perpendicular)
        /// </summary>
        public Vector2 Normal => new Vector2(Direction.y, -1f * Direction.x);

        /// <summary>
        /// Direction of line segment
        /// </summary>
        public Vector2 Direction => (End - Start).Normalized();

        /// <summary>
        /// A line segment with the start and end switched
        /// </summary>
        public LineSegment Inverted => this with { Start = this.End, End = this.Start };

        /// <summary>
        /// The vector from Start to End
        /// </summary>
        public Vector2 Size => new(End - Start);

        /// <summary>
        /// Length of the line
        /// </summary>
        public Single Length => Size.Length();
    }
}
