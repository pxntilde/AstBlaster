using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Hero
{
    /// <summary>
    /// The ship for the hero
    /// </summary>
    public class HeroShip : Ship
    {
        /// <summary>
        /// Player input
        /// </summary>
        public InputData Input { protected get; set; }

        /// <summary>
        /// PHYSICS PROCESS
        /// </summary>
        public override void _PhysicsProcess(Single delta)
        {
            if (Input is null) return;
            ApplyThrust(Input.Thrust, Input.UnlockThrust);
            ApplyTorque(Input.Yaw, Input.UnlockRotation);
        }
    }
}