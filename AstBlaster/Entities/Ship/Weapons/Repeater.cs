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
    /// Repeater style weapon
    /// </summary>
    public class Repeater : Node2D, IWeapon
    {
        private Game Game => _Game ??= Game.Instance;
        private Game _Game;

        #region IWeapon
        public IDamager Instigator { get; set; }
        public Boolean Firing { get; set; }
        #endregion

        /// <summary>
        /// Time until the next fire can happen
        /// </summary>
        private UInt64 NextShot = 0;

        /// <summary>
        /// Force to fire with
        /// </summary>
        [Export]
        public Single FireForce = 10000f;

        /// <summary>
        /// Time between shots in ms
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
            if (CanFire is true)
            {
                if (Firing is true)
                {
                    FireWeapon();
                    NextShot += Cooldown;
                }
                else
                {
                    NextShot = Time.GetTicksMsec();
                }
            }
        }

        /// <summary>
        /// Whether a shot is ready to be fired
        /// </summary>
        protected virtual Boolean CanFire => Time.GetTicksMsec() > NextShot;

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

            Game.AddChild(instance.AsNode);
            instance.SetOrientation(GlobalPosition, GlobalTransform.x);
            instance.SetForce(FireForce);
            instance.Instigator = Instigator;
        }
    }
}
