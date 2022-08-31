using AstBlaster.Entities.Ship;
using AstBlaster.Geo;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    /// <summary>
    /// A randomly generated polygon based physics body
    /// </summary>
    public class Asteroid : WrappingBody, IDestructable
    {
        /// <summary>
        /// Asteroid polygon
        /// </summary>
        protected Polygon Polygon;

        /// <summary>
        /// Damage per polygon side
        /// </summary>
        protected Dictionary<Int32, Single> Damage;

        /// <summary>
        /// Collision shape owner ID
        /// </summary>
        protected UInt32 OwnerID;

        /// <summary>
        /// Polyong cached as a shape for drawing
        /// </summary>
        protected Vector2[] CachedShape;

        #region Node

        /// <summary>
        /// INIT
        /// </summary>
        public Asteroid()
        {
            Polygon = Geo.Geo.RandomPolygon(3, 100f);
            CachedShape = Polygon.ToArray();
            ClearCollisionShapes();

            OwnerID = CreateShapeOwner(this);
            var collision = new ConvexPolygonShape2D();
            collision.Points = CachedShape;
            ShapeOwnerAddShape(OwnerID, collision);
            
            Damage = new();
            for (var i = 0; i < Polygon.Count; i++)
            {
                Damage.Add(i, 0f);
            }

            Mass = Polygon.Area;
        }

        /// <summary>
        /// PROCESS
        /// </summary>
        public override void _Process(Single delta)
        {
            Update();
        }

        /// <summary>
        /// DRAW
        /// </summary>
        public override void _Draw()
        {
            DrawColoredPolygon(CachedShape, Colors.SaddleBrown, null, null, null, true);
        }
        #endregion

        protected void ClearCollisionShapes()
        {
            var owners = GetShapeOwners();
            for (var i = 0; i < owners.Count; i++)
            {
                ShapeOwnerClearShapes((UInt32)(Int32)owners[i]);
            }
        }

        public override void _IntegrateForces(Physics2DDirectBodyState state)
        {
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;

            base._IntegrateForces(state);
        }

        public Node2D AsNode2D => this;

        public virtual void ApplyDamage(IDamager instigator, Single amount, Vector2 position)//, Vector2 Direction)
        {
            position = ToLocal(position);
            Int32 targetIndex = 0;
            Single targetDistaceSquared = Single.PositiveInfinity;
            var sides = Polygon.GetSides();

            for (var i = 0; i < sides.Count; i++)
            {
                var distance = position.DistanceSquaredTo(sides[i].Midpoint);
                if (distance < targetDistaceSquared)
                {
                    targetDistaceSquared = distance;
                    targetIndex = i;
                }
            }

            Damage[targetIndex] = amount;

            // temporary
            var game = Game.Instance;
            var polys = Polygon.Bisect(targetIndex);

            if (polys.Count > 0)
            {
                var aster = new NonrandomAsteroid();
               // GD.Print($"Creating {aster}");
                aster.Create(polys[0]);
               // GD.Print($"Created {aster}");
                game.AddChild(aster);
                aster.GlobalTransform = GlobalTransform;
                aster.AddCentralForce(GlobalPosition.DirectionTo(aster.GlobalPosition) * LinearVelocity.Length() * Mass);

                aster = new NonrandomAsteroid();
                aster.Create(polys[1]);
                game.AddChild(aster);
                aster.GlobalTransform = GlobalTransform;
                aster.AddCentralForce(GlobalPosition.DirectionTo(aster.GlobalPosition) * LinearVelocity.Length() * Mass);

                QueueFree();
            }
            else
            {
                GD.Print($"Failed");
            }
        }
    }
}