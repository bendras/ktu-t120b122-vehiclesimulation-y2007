using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VehicleSimulation.Objects
{
    public class DrawableEntity : DrawableGameComponent
    {
        protected Model Model_;
        protected Vector3 Position_;

        public DrawableEntity(Game game) : base(game)
        {
            
        }

        public BoundingSphere[] BoundingSphere
        {
            get
            {
                List<BoundingSphere> boundingSpheres = new List<BoundingSphere>();
                foreach (ModelMesh mesh in Model_.Meshes)
                {
                    BoundingSphere sphere = new BoundingSphere(
                        mesh.BoundingSphere.Center + this.Position_, 
                        mesh.BoundingSphere.Radius);
                    boundingSpheres.Add(sphere);
                }
                return boundingSpheres.ToArray();
            }
        }

        public bool Intersects(BoundingSphere[] boundingSpheres)
        {
            foreach (BoundingSphere boundingSphere1 in boundingSpheres)
            {
                foreach (BoundingSphere boundingSphere2 in this.BoundingSphere)
                {
                    if (boundingSphere1.Intersects(boundingSphere2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}