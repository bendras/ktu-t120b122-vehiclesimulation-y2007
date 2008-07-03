using Microsoft.Xna.Framework;

namespace VehicleSimulation
{
    public static class Settings
    {
        private static Game game;

        public static Game Game
        {
            get { return game; }
            set { game = value; }
        }

        public static GraphicsDeviceManager GraphicsDevice
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }

        private static GraphicsDeviceManager graphicsDevice;


    }
}