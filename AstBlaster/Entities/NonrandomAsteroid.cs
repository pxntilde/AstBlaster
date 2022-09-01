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
            CachedShape = Polygon.ToArray();
            AssignCollision();

            Damage = new();
            for (var i = 0; i < Polygon.Count; i++)
            {
                Damage.Add(i, 0f);
            }

            Mass = Polygon.Area;
        }

        

        //public override void _Draw()
        //{
        //    base._Draw();
        //    DrawCircle(Polygon.Centroid, 3f, Colors.Green);
        //    DrawCircle(Vector2.Zero, 2f, Colors.Red);
        //}
    }
}
