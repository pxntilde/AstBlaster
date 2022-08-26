using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship
{
    public abstract class Ship : WrappingBody
    {

        /// <summary>
        /// Maximum thrust force
        /// </summary>
        [Export]
        public Single MaxThrust { get; private set; } = 60000f;

        /// <summary>
        /// Maximum torque
        /// </summary>
        [Export]
        public Single MaxTorque { get; private set; } = 25000f;

        protected Vector2 CurrentThrust = Vector2.Zero;
        protected Single CurrentTorque = 0f;

        /// <summary>
        /// Damps forward velocity
        /// </summary>
        protected void DampForward()
        {
            CurrentThrust = (-1f * LinearVelocity * MaxThrust).LimitLength(MaxThrust);
        }

        /// <summary>
        /// Damps rotation velocity
        /// </summary>
        protected void DampRotation()
        {
            CurrentTorque = Mathf.Clamp(MaxTorque * AngularVelocity * -1f, MaxTorque * -1f, MaxTorque);
        }

        /// <summary>
        /// Applies thrust
        /// </summary>
        protected void ApplyThrust(Single scale = 1f)
        {
            CurrentThrust = (CurrentThrust + Transform.x * scale * MaxThrust).LimitLength(MaxThrust);
        }

        /// <summary>
        /// Applies torque
        /// </summary>
        protected void ApplyTorque(Single scale = 1f)
        {
            CurrentTorque = Mathf.Clamp(CurrentTorque + MaxTorque * scale, MaxTorque * -1f, MaxTorque);
        }

        public override void _PhysicsProcess(Single delta)
        {
            if (CurrentThrust != Vector2.Zero)
            {
                AddForce(Vector2.Zero, CurrentThrust);
            }

            if (CurrentTorque != 0)
            {
                AddTorque(CurrentTorque);
            }
        }
    }
}
