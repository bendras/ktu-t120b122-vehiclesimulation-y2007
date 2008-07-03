using Microsoft.Xna.Framework.Content;

namespace VehicleSimulation.Objects.Sky
{
    /// <summary>Helper for reading a Sky object from the compiled XNB format. </summary>
    public class SkyReader : ContentTypeReader<Sky>
    {
        protected override Sky Read(ContentReader input, Sky existingInstance)
        {
            return new Sky(input);
        }
    }
}