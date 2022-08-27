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
    /// Tells a hero what to do
    /// </summary>
    public class HeroController : Node
    {
        /// <summary>
        /// Ship to be controlled
        /// </summary>
        public HeroShip Ship => _Ship ??= GetParent<HeroShip>() ?? throw new MisparentError(nameof(HeroShip), GetParent().GetType().ToString());
        public HeroShip _Ship;

        /// <summary>
        /// PROCESS
        /// </summary>
        public override void _Process(Single delta)
        {
            var thrust = 0f;
            var yaw = 0f;
            var unlockThrust = false;
            var unlockRotation = false;

            if (Input.IsActionPressed("thrusters"))
            {
                thrust += 1f;
            }
            if (Input.IsActionPressed("rotate_left"))
            {
                yaw -= 1f;
            }
            if (Input.IsActionPressed("rotate_right"))
            {
                yaw += 1f;
            }
            if (Input.IsActionPressed("unlock_thrust"))
            {
                unlockThrust = true;
            }
            if (Input.IsActionPressed("unlock_rotation"))
            {
                unlockRotation = true;
            }

            Ship.Input = new(thrust, yaw, unlockThrust, unlockRotation);
        }
    }
}
