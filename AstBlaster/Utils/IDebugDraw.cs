using AstBlaster.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace AstBlaster.Utils
{
    /// <summary>
    /// Draws debug shapes
    /// </summary>
    public interface IDebugDraw
    {
        /// <summary>
        /// Lines to draw (in local coordinates)
        /// </summary>
        public Dictionary<UInt64, DebugLine> Lines { get; }

        /// <summary>
        /// Circles to draw (in local coordinates)
        /// </summary>
        public Dictionary<UInt64, DebugCircle> Circles { get; }
    }

    public record struct DebugCircle
    {
        public DebugCircle(Vector2 position, Single radius, Color color)
        {
            Position = position;
            Radius = radius;
            Color = color;
        }

        public Vector2 Position { get; private set; }
        public Single Radius { get; private set; }
        public Color Color { get; private set; }
    }

    public record struct DebugLine
    {
        public DebugLine(Vector2 from, Vector2 to, Color color)
        {
            From = from;
            To = to;
            Color = color;
        }

        public Vector2 From { get; private set; }
        public Vector2 To { get; private set; }
        public Color Color { get; private set; }
    }
}
