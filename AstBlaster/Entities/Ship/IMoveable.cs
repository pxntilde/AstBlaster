using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship
{
    public interface IMoveable
    {
        /// <summary>
        /// Input vector from controller
        /// </summary>
        public void AddInputVector(Vector2 inputVector);

        /// <summary>
        /// Whether to override rotation damping
        /// </summary>
        public Boolean RotationDampOverride { get; set; }

        /// <summary>
        /// Whether to override forward damping
        /// </summary>
        public Boolean ForwardDampOverride { get; set; }
    }
}
