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
            Polygon = Geo.Geo.RandomPolygon(13, 200f);
            CachedShape = Polygon.ToArray();
            AssignCollision();
            
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
            //DrawCircle(Polygon.Centroid, 3f, Colors.Green);
            //DrawCircle(Vector2.Zero, 2f, Colors.Red);
        }
        #endregion

        protected void AssignCollision()
        {
            ClearCollisionShapes();
            OwnerID = CreateShapeOwner(this);
            var collision = new ConvexPolygonShape2D();
            collision.Points = CachedShape;
            ShapeOwnerAddShape(OwnerID, collision);
        }

        protected void ClearCollisionShapes()
        {
            var owners = GetShapeOwners();
            for (var i = 0; i < owners.Count; i++)
            {
                ShapeOwnerClearShapes((UInt32)(Int32)owners[i]);
            }
        }

        /// <summary>
        /// Moves center of mass to origin
        /// </summary>
        public void Recenter(Boolean keepGlobalPosition = true)
        {
            var offset = Polygon.Center();
            if (keepGlobalPosition is true)
            {
                Position -= offset;
            }
            CachedShape = Polygon.ToArray();
            AssignCollision();
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
            var force = (LinearVelocity.Length() + amount / 2f) * Mass;

            if (polys.Count > 0)
            {

                var aster = new NonrandomAsteroid();
                aster.Create(polys[0]);
                game.AddChild(aster);
                aster.GlobalTransform = GlobalTransform;
                aster.Recenter();
                aster.AddCentralForce(GlobalPosition.DirectionTo(aster.GlobalPosition) * force);
                aster.ApplyImpulse(aster.ToLocal(ToGlobal(position)), -aster.ToLocal(ToGlobal(position)).Normalized() * amount / 200f);
                
                aster = new NonrandomAsteroid();
                aster.Create(polys[1]);
                game.AddChild(aster);
                aster.GlobalTransform = GlobalTransform;
                aster.Recenter();
                aster.ApplyImpulse(aster.ToLocal(ToGlobal(position)), -aster.ToLocal(ToGlobal(position)).Normalized() * amount / 200f);


                QueueFree();
            }
            else
            {
                GD.Print($"Failed");
            }
        }
    }
}