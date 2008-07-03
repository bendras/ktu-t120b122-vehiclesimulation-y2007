using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VehicleSimulation.Objects.Terrain
{
    /// <summary>
    /// Terrain is a collection of data about the heightmap. It includes
    /// information about how high the terrain is, and how far apart each vertex is.
    /// It also has several functions to get information about the heightmap, including
    /// its height and normal at different points, and whether a point is on the 
    /// heightmap. It is the runtime equivalent of HeightMapInfoContent.
    /// </summary>
    public class Terrain
    {
        #region Private fields

        public Model model;
        Texture texture;

        /// <summary> TerrainScale is the distance between each entry in the Height property. </summary>
        /// <example>If TerrainScale is 30, Height[0,0] and Height[1,0] are 30 units apart. </example>
        private float terrainScale;

        /// <summary>  This 2D array of floats tells us the height that each position in the heightmap is.  </summary>
        private float[,] heights;

        private Vector3[,] normals;

        /// <summary>The position of the heightmap's -x, -z corner, in worldspace.  </summary>
        private Vector3 heightmapPosition;

        /// <summary>The total width of the heightmap, including terrainscale.  </summary>
        private float heightmapWidth;

        /// <summary>Tthe total height of the height map, including terrainscale. </summary>
        private float heightmapHeight;

        #endregion

        /// <summary>
        /// Initialize all of the member variables.
        /// </summary>
        public Terrain(float[,] heights, Vector3[,] normals, float terrainScale)
        {
            if (heights == null)
            {
                throw new ArgumentNullException("heights");
            }
            if (normals == null)
            {
                throw new ArgumentNullException("normals");
            }

            this.terrainScale = terrainScale;
            this.heights = heights;
            this.normals = normals;

            this.heightmapWidth = (heights.GetLength(0) - 1) * terrainScale;
            this.heightmapHeight = (heights.GetLength(1) - 1) * terrainScale;

            this.heightmapPosition.X = -(heights.GetLength(0) - 1) / 2.0f * terrainScale;
            this.heightmapPosition.Z = -(heights.GetLength(1) - 1) / 2.0f * terrainScale;
        }


        /// <summary>This function takes in a position, and tells whether or not the position is on the heightmap. </summary>
        public bool IsOnHeightmap(Vector3 position)
        {
            // first we'll figure out where on the heightmap "position" is...
            Vector3 positionOnHeightmap = position - this.heightmapPosition;

            // ... and then check to see if that value goes outside the bounds of the heightmap.
            return (positionOnHeightmap.X > 0 &&
                    positionOnHeightmap.X < this.heightmapWidth &&
                    positionOnHeightmap.Z > 0 &&
                    positionOnHeightmap.Z < this.heightmapHeight);
        }

        /// <param name="position">Position</param>
        /// <param name="height">Heightmap's height.</param>
        /// <param name="normal">Normal at that point.</param>
        /// <exception cref="IndexOutOfRangeException">If position isn't on the heightmap.</exception>
        public void GetHeightAndNormal(Vector3 position, out float height, out Vector3 normal)
        {
            // the first thing we need to do is figure out where on the heightmap "position" is.
            Vector3 positionOnHeightmap = position - this.heightmapPosition;

            // we'll use integer division to figure out where in the "heights" array
            // positionOnHeightmap is. Remember that integer division always rounds
            // down, so that the result of these divisions is the indices of the "upper
            // left" of the 4 corners of that cell.
            int left = (int)positionOnHeightmap.X / (int)this.terrainScale;
            int top = (int)positionOnHeightmap.Z / (int)this.terrainScale;

            // next, we'll use modulus to find out how far away we are from the upper
            // left corner of the cell. Mod will give us a value from 0 to terrainScale,
            // which we then divide by terrainScale to normalize 0 to 1.
            float xNormalized = (positionOnHeightmap.X % this.terrainScale) / this.terrainScale;
            float zNormalized = (positionOnHeightmap.Z % this.terrainScale) / this.terrainScale;

            // Now that we've calculated the indices of the corners of our cell, and
            // where we are in that cell, we'll use bilinear interpolation to calculuate
            // our height. This process is best explained with a diagram, so please see
            // the accompanying doc for more information.
            // First, calculate the heights on the bottom and top edge of our cell by
            // interpolating from the left and right sides.
            float topHeight = MathHelper.Lerp(this.heights[left, top], this.heights[left + 1, top], xNormalized);
            float bottomHeight = MathHelper.Lerp(this.heights[left, top + 1], this.heights[left + 1, top + 1], xNormalized);

            // next, interpolate between those two values to calculate the height at our position.
            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

            // We'll repeat the same process to calculate the normal.
            Vector3 topNormal = Vector3.Lerp(this.normals[left, top], this.normals[left + 1, top], xNormalized);
            Vector3 bottomNormal = Vector3.Lerp(this.normals[left, top + 1], this.normals[left + 1, top + 1], xNormalized);

            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();
        }

        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            Matrix[] boneTransforms = new Matrix[this.model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    // Set the fog to match the black background color
                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = 1000;
                    effect.FogEnd = 3200;
                }

                mesh.Draw();
            }
        }
    }

   
}