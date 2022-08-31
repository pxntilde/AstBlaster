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
        /// <summary>
        /// Sets the position and direction in global space
        /// </summary>
        /// <param name="position">Global position</param>
        /// <param name="direction">Global direction</param>
        public void SetOrientation(Vector2 position, Vector2 direction);
        
        /// <summary>
        /// Sets the initial force
        /// </summary>
        /// <param name="force">Amount</param>
        public void SetForce(Single force);

        /// <summary>
        /// Entity responsible for damage
        /// </summary>
        public IDamager Instigator { set; }

        /// <summary>
        /// IProjetile as a node (for AddChild)
        /// </summary>
        public Node AsNode { get; }
    }
}
