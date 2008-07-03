using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VehicleSimulation.Objects.Sky
{
    /// <summary>
    /// Runtime class for loading and rendering a textured skydome
    /// that was created during the build process by the SkyProcessor.
    /// </summary>
    public class Sky
    {
        #region Fields

        Model skyModel;
        Texture skyTexture;

        #endregion


        /// <summary>
        /// Constructor is internal: this should only ever
        /// be called by the SkyReader class.
        /// </summary>
        internal Sky(ContentReader input)
        {
            skyModel = input.ReadObject<Model>();
            skyTexture = input.ReadObject<Texture>();
        }


        /// <summary>
        /// Helper for drawing the skydome mesh.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in skyModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["Texture"].SetValue(skyTexture);
                }

                mesh.Draw(SaveStateMode.SaveState);
            }
        }
    }
}