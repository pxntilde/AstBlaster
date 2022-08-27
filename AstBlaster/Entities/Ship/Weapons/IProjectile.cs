using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Weapons
{
    /// <summary>
    /// Defines a projectile
    /// </summary>
    public interface IProjectile
    {
        public void Spawn(Vector2 position, Vector2 direction);
        public void Launch(Single force);
        public Ship Instigator { set; }
    }
}
