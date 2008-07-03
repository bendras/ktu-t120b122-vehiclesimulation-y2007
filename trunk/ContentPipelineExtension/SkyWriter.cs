using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Content pipeline support class for saving out SkyContent objects.
    /// </summary>
    [ContentTypeWriter]
    public class SkyWriter : ContentTypeWriter<SkyContent>
    {
        /// <summary>
        /// Saves sky data into an XNB file.
        /// </summary>
        protected override void Write(ContentWriter output, SkyContent value)
        {
            output.WriteObject(value.Model);
            output.WriteObject(value.Texture);
        }


        /// <summary>
        /// Tells the content pipeline what CLR type the sky
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "VehicleSimulation.Objects.Sky.Sky, VehicleSimulation, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the sky data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "VehicleSimulation.Objects.Sky.SkyReader, VehicleSimulation, Version=1.0.0.0, Culture=neutral";
        }
    }
}