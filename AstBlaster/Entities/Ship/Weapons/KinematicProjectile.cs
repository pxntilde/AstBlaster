using AstBlaster.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Weapons
{
    public class KinematicProjectile : KinematicBody2D, IProjectile
    {
        public IDamager Instigator { get; set; }
        public Node AsNode => this;

        /// <summary>
        /// Velocity
        /// </summary>
        public Vector2 Velocity { get; protected set; }

        /// <summary>
        /// Mass
        /// </summary>
        [Export]
        public Single Mass { get; protected set; } = 1f;

        public void SetForce(Single force)
        {
            Velocity = Transform.x * (force / Mass);
            GD.Print(Velocity);
        }

        public void SetOrientation(Vector2 position, Vector2 direction)
        {
            GlobalPosition = position;
            LookAt(position + direction);
        }

        public static UInt64 Key = Game.UniqueKey;

        public override void _PhysicsProcess(Single delta)
        {

            var collision = MoveAndCollide(Velocity, false);
            //MoveAndCollide(Velocity * delta);

            Transform = WrappingBody.RectifyPosition(Transform);

            if (collision is null) return;

            if (collision.Collider is IDebugDraw debug)
            {
                var node = debug as Node2D;
                debug.Circles[Key] = new(node.ToLocal(collision.Position), 2f, Colors.Red);
                var pos = node.ToLocal(collision.Position);
                debug.Circles[Key + 1] = new(pos, 2f, Colors.Blue);
                debug.Lines[Key] = new(pos, pos + ( -100f * collision.Normal), Colors.Blue);
            }
            if (collision.Collider is RigidBody2D rigidBody)
            {
                var force = Mass * Velocity.Length();
                GD.Print($"force: {force}");
                rigidBody.AddForce(rigidBody.GlobalPosition - collision.Position, force * -1f * collision.Normal);
            }

            QueueFree();
        }

        public override void _Process(Single delta)
        {
            Update();
        }

        /// <summary>
        /// DRAW
        /// </summary>
        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, 5f, Colors.White);
        }
    }
}
