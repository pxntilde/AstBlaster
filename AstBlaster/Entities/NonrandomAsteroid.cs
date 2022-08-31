using AstBlaster.Geo;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    public class NonrandomAsteroid : Asteroid
    {
        public NonrandomAsteroid()
        {
            CollisionLayer = 4u;
            CollisionMask = 2u + 4u + 8u;
        }

        public void Create(Polygon poly)
        {
            Polygon = poly;
            var centroid = poly.Centroid;
            poly.Center();
            Position += centroid;
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
    }
}
