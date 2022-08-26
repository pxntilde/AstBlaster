using AstBlaster.Utils.Exceptions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Hero
{
    /// <summary>
    /// Human-driven movement controller
    /// </summary>
    public class PlayerMovementController : Controller
    {
        public IMoveable Hero => _Hero ??= GetParentOrNull<IMoveable>() ?? throw new MisparentError(nameof(IMoveable), GetParent().GetType().ToString());
        private IMoveable _Hero;

        private Vector2 InputVector;
        private Boolean ForwardDamp;
        private Boolean RotationDamp;

        /// <summary>
        /// PROCESS
        /// </summary>
        public override void _Process(Single delta)
        {
            var input = Vector2.Zero;
            var forwardDamp = true;
            var rotationDamp = true;
            if (Input.IsActionPressed("thrusters"))
            {
                input.y += 1f;
            }
            if (Input.IsActionPressed("rotate_left"))
            {
                input.x -= 1f;
            }
            if (Input.IsActionPressed("rotate_right"))
            {
                input.x += 1f;
            }
            if (Input.IsActionPressed("forward_damp_override"))
            {
                forwardDamp = false;
            }
            if (Input.IsActionPressed("rotation_damp_override"))
            {
                rotationDamp = false;
            }

            InputVector = input;
            ForwardDamp = forwardDamp;
            RotationDamp = rotationDamp;
        }

        /// <summary>
        /// PHYSICS PROCESS
        /// </summary>
        public override void _PhysicsProcess(Single delta)
        {
            Hero.AddInputVector(InputVector);
            Hero.RotationDampOverride = !RotationDamp;
            Hero.ForwardDampOverride = !ForwardDamp;
        }
    }
}