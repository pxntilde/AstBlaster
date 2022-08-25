using AstBlaster.Geo;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities
{
    public class Asteroid : WrappingBody, IDestructable
    {
        private Polygon Polygon;
        private Dictionary<Int32, Single> Damage;
        private UInt32 OwnerID;
        private Vector2[] CachedShape;

        #region Node
        /// <summary>
        /// ENTER TREE
        /// </summary>
        public override void _EnterTree()
        {
            Polygon = Geo.Geo.RandomPolygon(11, 100f);
            CachedShape = Polygon.ToArray();
            OwnerID = CreateShapeOwner(this);
            var collision = new ConvexPolygonShape2D();
            collision.Points = CachedShape;
            ShapeOwnerAddShape(OwnerID, collision);

            Damage = new();
            for (var i = 0; i < Polygon.Count; i++)
            {
                Damage.Add(i, 0f);
            }
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
            //DrawCircle(Vector2.Zero, 100f, Colors.Bisque);
            DrawColoredPolygon(CachedShape, Colors.SaddleBrown, null, null, null, true);
            //DrawCircle(Vector2.Zero, 1f, Colors.White);
            //DrawCircle(Polygon.Centroid, 2f, Colors.Pink);
        }
        #endregion
    }
}
