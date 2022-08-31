using AstBlaster.Utils.Exceptions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship.Weapons
{
    /// <summary>
    /// A ship weapon
    /// </summary>
    public interface IWeapon
    {
        /// <summary>
        /// Whether the weapon is firing
        /// </summary>
        public Boolean Firing { get; set; }

        /// <summary>
        /// Entity responsible for damage from fired projectiles
        /// </summary>
        public IDamager Instigator { get; set; }
    }
}
