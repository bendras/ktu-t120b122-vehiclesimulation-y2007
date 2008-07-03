// http://msdn2.microsoft.com/en-us/library/microsoft.xna.framework.content.pipeline.graphics.meshbuilder.aspx

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Custom content processor for creating terrain meshes. Given an
    /// input heightfield texture, this processor uses the MeshBuilder
    /// class to programatically generate terrain geometry.
    /// </summary>
    [ContentProcessor(DisplayName = "ContentPipelineExtension.TerrainProcessor")]
    public class TerrainProcessor : ContentProcessor<Texture2DContent, TerrainContent>
    {
        // This region defines the parameters that this processor accepts. This feature
        // is new to XNA Game Studio 2.0, and is used to further customize how your
        // assets are built. We'll use these parameters to control how our heightmap
        // image is converted into a mesh. The parameters use DefaultValue, Description,
        // and DisplayName attributes to control how they are displayed in the UI.
        #region Processor Parameters

        /// <summary>
        /// Controls the scale of the terrain. This will be the distance between
        /// vertices in the finished terrain mesh.
        /// </summary>
        [DefaultValue(30.0f)]
        [Description("The distance between vertices in the finished terrain mesh.")]
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        private float scale = 30.0f;

                
        /// <summary>
        /// Controls the height of the terrain. The heights of the vertices in the
        /// finished mesh will vary between 0 and -Bumpiness.
        /// </summary>
        [DefaultValue(640.0f)]
        [Description("Controls the height of the terrain.")]
        public float Bumpiness
        {
            get { return bumpiness; }
            set { bumpiness = value; }
        }
        private float bumpiness = 640.0f;
        
        
        /// <summary>
        /// Controls the how often the texture applied to the terrain will be repeated.
        /// </summary>
        [DefaultValue(.1f)]
        [Description("Controls how often the texture will be repeated "+
                     "across the terrain.")]
        public float TexCoordScale
        {
            get { return texCoordScale; }
            set { texCoordScale = value; }
        }
        private float texCoordScale = 0.1f;


        /// <summary>
        /// Controls the texture that will be applied to the terrain. If no value is
        /// supplied, a texture will not be applied.
        /// </summary>
        [DefaultValue("rocks.bmp")]
        [Description("Controls the texture that will be applied to the terrain. If no value is supplied, a texture will not be applied.")]
        [DisplayName("Terrain Texture")]
        public string TerrainTexture
        {
            get { return terrainTexture; }
            set { terrainTexture = value; }
        }
        private string terrainTexture = "rocks.bmp";

        #endregion


        /// <summary> Generates a terrain mesh from an input heightfield texture. </summary>
        public override TerrainContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            PixelBitmapContent<float> heightfield;

            // 1. Call StartMesh to get a MeshBuilder object.
            MeshBuilder builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));


            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain vertices.
            for (int y = 0; y < heightfield.Height; y++)
            {
                for (int x = 0; x < heightfield.Width; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X = scale * (x - ((heightfield.Width - 1) / 2.0f));
                    position.Z = scale * (y - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x, y) - 1) * bumpiness;

                    //To initialize this object, fill the positions buffer with data by using the CreatePosition function.
                    builder.CreatePosition(position);
                }
            }

            // Create a material, and point it at our terrain texture.
            BasicMaterialContent material = new BasicMaterialContent();
            material.SpecularColor = new Vector3(.4f, .4f, .4f);

            if (!string.IsNullOrEmpty(TerrainTexture))
            {
                string directory = Path.GetDirectoryName(input.Identity.SourceFilename);
                string texture = Path.Combine(directory, TerrainTexture);

                material.Texture = new ExternalReference<TextureContent>(texture);
            }

            

            // After retrieving the positions data, call the CreateVertexChannel generic function to specify the types of vertex channels needed—for example, normals, UVs, and color channels.
            // Create a vertex channel for holding texture coordinates.
            int texCoordId = builder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // 2. After setting up the position and vertex data channel buffers, begin creating triangles. Use SetMaterial and SetOpaqueData to set the data of each triangle, and use SetVertexChannelData to set the individual vertex data of each triangle. After setting this data, call AddTriangleVertex for each vertex of each triangle.
            builder.SetMaterial(material);

            // Create the individual triangles that make up our terrain.
            for (int y = 0; y < heightfield.Height - 1; y++)
            {
                for (int x = 0; x < heightfield.Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);

                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);
                    AddVertex(builder, texCoordId, heightfield.Width, x, y + 1);
                }
            }

            // 3. In the final step, call FinishMesh. After this call, the mesh is optimized automatically with calls to MergeDuplicateVertices and CalculateNormals.
            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent terrainMesh = builder.FinishMesh();
            
            TerrainContent terrainContent = new TerrainContent(terrainMesh, scale, heightfield.Width, heightfield.Height);
            terrainContent.Model = context.Convert<MeshContent, ModelContent>(terrainMesh, "ModelProcessor");


            return terrainContent;
        }


        /// <summary>
        /// Helper for adding a new triangle vertex to a MeshBuilder,
        /// along with an associated texture coordinate value.
        /// </summary>
        void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * TexCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }
    }
}