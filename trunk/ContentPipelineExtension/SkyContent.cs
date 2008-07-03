using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Design time class for holding a skydome. This is created by
    /// the SkyProcessor, then written out to a compiled XNB file.
    /// At runtime, the data is loaded into the runtime Sky class.
    /// </summary>
    public class SkyContent
    {
        public ModelContent Model;
        public TextureContent Texture;
    }
}