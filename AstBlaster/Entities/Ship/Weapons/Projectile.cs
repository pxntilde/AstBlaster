using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Weapons
{
    /// <summary>
    /// Base class for a projectile
    /// </summary>
    public class Projectile : WrappingBody, IProjectile
    {
        public Ship Instigator { get; set; }

        public void Launch(Single force) => ApplyCentralImpulse(Transform.x * force);

        public void Spawn(Vector2 position, Vector2 direction)
        {
            GlobalPosition = position;
            LookAt(GlobalPosition + direction);
        }

        public override void _Process(Single delta)
        {
            Update();
        }

        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, 5f, Colors.White);
        }
    }
}
