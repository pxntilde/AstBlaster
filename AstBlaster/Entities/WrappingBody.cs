using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    /// <summary>
    /// Rigid body that wraps from one side of the screen to the other
    /// </summary>
    public class WrappingBody : RigidBody2D
    {
        /// <summary>
        /// Game instance
        /// </summary>
        private Game Game = Game.Instance;

        /// <summary>
        /// INTEGRATE FORCES
        /// </summary>
        public override void _IntegrateForces(Physics2DDirectBodyState state)
        {
            state.Transform = RectifyPosition(state.Transform);
        }

        /// <summary>
        /// Wraps body when out of bounds
        /// </summary>
        /// <param name="transform">body transform</param>
        /// <returns>Wrapped transform</returns>
        public static Transform2D RectifyPosition(Transform2D transform)
        {
            var position = transform.origin;
            var bounds = Game.Instance?.Bounds ?? throw new NullReferenceException("Game has no instance");


            if (position.x > bounds.End.x)
            {
                position.x = bounds.Position.x;
            }
            else if (position.x < bounds.Position.x)
            {
                position.x = bounds.End.x;
            }

            if (position.y > bounds.End.y)
            {
                position.y = bounds.Position.y;
            }
            else if (position.y < bounds.Position.y)
            {
                position.y = bounds.End.y;
            }

            transform.origin = position;

            return transform;
        }
    }
}
