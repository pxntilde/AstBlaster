using AstBlaster.Entities.Ship;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    /// <summary>
    /// Destructable entity
    /// </summary>
    public interface IDestructable
    {
        public void ApplyDamage(IDamager instigator, Single amount, Vector2 position);
        public Node2D AsNode2D { get; }
    }
}
