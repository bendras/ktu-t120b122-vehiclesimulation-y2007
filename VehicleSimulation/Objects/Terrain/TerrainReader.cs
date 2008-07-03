using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VehicleSimulation.Objects.Terrain;

namespace VehicleSimulation.Objects.Terrain
{
    /// <summary>
    /// This class will load the Terrain when the game starts. 
    /// This class needs to match the HeightMapInfoWriter.
    /// </summary>
    public class TerrainReader : ContentTypeReader<Terrain>
    {
        protected override Terrain Read(ContentReader input, Terrain existingInstance)
        {
            float terrainScale = input.ReadSingle();
            int width = input.ReadInt32();
            int height = input.ReadInt32();
            float[,] heights = new float[width, height];
            Vector3[,] normals = new Vector3[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    heights[x, z] = input.ReadSingle();
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    normals[x, z] = input.ReadVector3();
                }
            }
            Terrain terrain = new Terrain(heights, normals, terrainScale);
            terrain.model = input.ReadObject<Model>();

            return terrain;
        }
    }
}