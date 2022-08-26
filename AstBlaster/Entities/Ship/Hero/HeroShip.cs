using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Hero
{

    public class HeroShip : Ship, IMoveable
    {
        private Vector2 InputVector;



        #region Godot functions
        /// <summary>
        /// PHYSICS PROCESS
        /// </summary>
        public override void _PhysicsProcess(Single delta)
        {
            if (InputVector.y > 0f)
            {
                ApplyThrust(InputVector.y);
            }
            else if (ForwardDampOverride is false)
            {
                DampForward();
            }

            if (InputVector.x != 0f)
            {
                ApplyTorque(InputVector.x);
            }
            else if (RotationDampOverride is false)
            {
                DampRotation();
            }

            base._PhysicsProcess(delta);

            InputVector = Vector2.Zero;
            CurrentThrust = Vector2.Zero;
            CurrentTorque = 0f;
        }
        #endregion



        #region IControllableShip
        public void AddInputVector(Vector2 inputVector) => InputVector += inputVector;
        public Boolean RotationDampOverride { get; set; }
        public Boolean ForwardDampOverride { get; set; } 
        #endregion
    }
}
