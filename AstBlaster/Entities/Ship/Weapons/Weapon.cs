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
    public class Weapon : Node2D
    {
        private Game Game => _Game ??= Game.Instance;
        private Game _Game;

        /// <summary>
        /// Time until the next fire can happen
        /// </summary>
        private UInt64 NextFire = 0;

        /// <summary>
        /// Force to fire with
        /// </summary>
        [Export]
        public Single FireForce = 10000f;

        /// <summary>
        /// Whether to fire
        /// </summary>
        public Boolean Fire = false;

        /// <summary>
        /// Time between shots
        /// </summary>
        [Export]
        public UInt64 Cooldown { get; protected set; }

        /// <summary>
        /// Projectile to fire
        /// </summary>
        [Export]
        public PackedScene Projectile { get; protected set; }

        /// <summary>
        /// PROCESS
        /// </summary>
        public override void _Process(Single delta)
        {
            if (Time.GetTicksMsec() > NextFire)
            {
                if (Fire is true)
                {
                    FireWeapon();
                    NextFire += Cooldown;
                }
                else
                {
                    NextFire = Time.GetTicksMsec();
                }
            }
        }

        /// <summary>
        /// Fires the weapon
        /// </summary>
        protected virtual void FireWeapon()
        {
            var instance = Projectile.InstanceOrNull<IProjectile>();

            if (instance is null || instance is not Node)
            {
                throw new InvalidTypeError($"Scene is not a {nameof(IProjectile)}");
            }
            Game.AddChild(instance as Node);
            instance.Spawn(GlobalPosition, GetParent<Node2D>().Transform.x);
            instance.Launch(FireForce);
            instance.Instigator = GetParentOrNull<Ship>();
        }
    }
}
