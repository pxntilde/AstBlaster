using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship
{
    /// <summary>
    /// Base class for a ship
    /// </summary>
    public abstract class Ship : WrappingBody
    {
        protected Boolean UnlockThrust = false;
        protected Boolean UnlockRotation = false;

        /// <summary>
        /// Maximum thrust force in 1kg pixel / s^2
        /// </summary>
        [Export]
        public Single Thrust { get; protected set; } = 60000f;

        /// <summary>
        /// Speed to target during locked thrust in units per second
        /// </summary>
        [Export]
        public Single TargetSpeed { get; protected set; } = 500f;

        /// <summary>
        /// Maximum torque
        /// </summary>
        [Export]
        public Single Torque { get; protected set; } = 25000f;

        /// <summary>
        /// Target rotation speed during locked rotation in radians per second
        /// </summary>
        [Export]
        public Single TargetRotation { get; protected set; } = 100f;

        /// <summary>
        /// Damps forward velocity
        /// </summary>
        protected virtual void DampMovement() => AddCentralForce((-1f * LinearVelocity * Thrust).LimitLength(Thrust));

        /// <summary>
        /// Damps rotation velocity
        /// </summary>
        protected virtual void DampRotation() => AddTorque(Mathf.Clamp(Torque * AngularVelocity * -0.9f, Torque * -1f, Torque)); 

        /// <summary>
        /// Applies thrust
        /// </summary>
        protected virtual void ApplyThrust(Single scale = 1f, Boolean unlocked = false)
        {
            if (unlocked is true)
            {
                AddCentralForce(Thrust * Transform.x * scale);
            }
            else
            {
                var v = LinearVelocity;
                var t = Transform.x * scale * TargetSpeed;
                var m = Mass;
                var force = m * (t - v);

                AddCentralForce(force.LimitLength(Thrust));
            }
        }

        /// <summary>
        /// Applies torque
        /// </summary>
        protected virtual void ApplyTorque(Single scale = 1f, Boolean unlocked = false)
        {
            if (unlocked is true)
            {
                AddTorque(Torque * scale);
            }
            else
            {
                var v = AngularVelocity;
                var t = TargetRotation * scale;
                var m = Inertia;
                var force = m * (t - v);

                AddTorque(Mathf.Clamp(force, -Torque, Torque));
            }
        }

        /// <summary>
        /// INTEGRATE FORCES
        /// </summary>
        public override void _IntegrateForces(Physics2DDirectBodyState state)
        {
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;

            base._IntegrateForces(state);
        }
    }
}
