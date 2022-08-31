using AstBlaster.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Weapons
{

    /// <summary>
    /// Basic projectile
    /// </summary>
    public class Projectile : WrappingBody, IProjectile
    {
        #region IProjectile
        public Node AsNode => this;

        public IDamager Instigator { get; set; }

        public void SetForce(Single force) => ApplyCentralImpulse(Transform.x * force);

        public void SetOrientation(Vector2 position, Vector2 direction)
        {
            GlobalPosition = position;
            LookAt(GlobalPosition + direction);
        }
        #endregion

        private Vector2 LastVelocity;
        private Vector2 LastPosition;

        public Projectile()
        {
            Connect("body_entered", this, nameof(OnBodyEntered));
        }

        /// <summary>
        /// PROCESS
        /// </summary>
        public override void _Process(Single delta)
        {
            Update();
        }

        public override void _IntegrateForces(Physics2DDirectBodyState state)
        {
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;

            for (var contact = 0; contact < state.GetContactCount(); contact++)
            {
                if (state.GetContactColliderObject(contact) is Node2D body)
                {
                    if (body is IDebugDraw debug)
                    {
                        debug.Circles[Keys[1]] = new(body.ToLocal(state.GetContactLocalPosition(contact)), 2f, Colors.Green);

                        debug.Circles[Keys[0]] = new DebugCircle(body.ToLocal(GlobalPosition), 5f, Colors.Gray);
                        debug.Lines[Keys[0]] = new(body.ToLocal(GlobalPosition - LastVelocity.Normalized() * 100f), body.ToLocal(GlobalPosition), Colors.White);

                        Physics2DTestMotionResult result = new();
                        if (TestMotion(LastVelocity, false, 0.1f, result) && result.Collider == body)
                        {
                            debug.Lines[Keys[1]] = new DebugLine(body.ToLocal(GlobalPosition), body.ToLocal(result.CollisionPoint), Colors.Orange);
                            QueueFree();
                        }
                        else if (TestMotion(-LinearVelocity * GetPhysicsProcessDeltaTime(), false, 0.1f, result) && result.Collider == body)
                        {
                            debug.Lines[Keys[1]] = new DebugLine(body.ToLocal(GlobalPosition), body.ToLocal(result.CollisionPoint), Colors.Yellow);
                            QueueFree();
                        }
                        else
                        {
                            debug.Lines.Remove(Keys[1]);
                        }
                    }
                }
                
                if (state.GetContactColliderObject(contact) is IDestructable destructable)
                {
                    Physics2DTestMotionResult result = new();

                    if ((TestMotion(LastVelocity, false, 0.1f, result) ||
                        TestMotion(-LinearVelocity, false, 0.1f, result)) && 
                        result.Collider == destructable)
                    {
                        var position = result.CollisionPoint;
                        destructable.ApplyDamage(Instigator, LastVelocity.Length(), position);
                        QueueFree();
                        return;
                    }
                }
            }

            base._IntegrateForces(state);
            LastVelocity = LinearVelocity;
        }

        /// <summary>
        /// DRAW
        /// </summary>
        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, 5f, Colors.White);
        }

        /// <summary>
        /// ON BODY ENTERED
        /// </summary>
        private void OnBodyEntered(PhysicsBody2D body)
        {
            if (body is IDebugDraw debug)
            {
                //debug.Circles[Keys[0]] = new DebugCircle(body.ToLocal(GlobalPosition), 5f, Colors.Gray);
                //debug.Lines[Keys[0]] = new(body.ToLocal(GlobalPosition - LastVelocity.Normalized() * 100f), body.ToLocal(GlobalPosition), Colors.White);

                //Physics2DTestMotionResult result = new();
                //if (TestMotion(LastVelocity, false, 0.1f, result) && result.Collider == body)
                //{
                //    debug.Circles[Keys[2]] = new DebugCircle(body.ToLocal(result.CollisionPoint), 5f, Colors.Orange);
                //    debug.Lines[Keys[1]] = new DebugLine(body.ToLocal(GlobalPosition), body.ToLocal(result.CollisionPoint), Colors.Orange);
                //    QueueFree();
                //}
                //else if (TestMotion(-LinearVelocity * GetPhysicsProcessDeltaTime(), false, 0.1f, result) && result.Collider == body)
                //{
                //    debug.Circles[Keys[2]] = new DebugCircle(body.ToLocal(result.CollisionPoint), 5f, Colors.Yellow);
                //    debug.Lines[Keys[1]] = new DebugLine(body.ToLocal(GlobalPosition), body.ToLocal(result.CollisionPoint), Colors.Yellow);
                //    QueueFree();

                //}
                //else
                //{
                //    debug.Lines.Remove(Keys[1]);
                //    debug.Circles.Remove(Keys[2]);
                //}

            }
        }

        private static List<UInt64> Keys = new()
        {
            Game.UniqueKey,
            Game.UniqueKey,
            Game.UniqueKey,
            Game.UniqueKey,
            Game.UniqueKey,
            Game.UniqueKey,
        };
    }
}
