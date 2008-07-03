using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VehicleSimulation.Objects.Entities;
using VehicleSimulation.Objects.Sky;
using VehicleSimulation.Objects.Terrain;
using VehicleSimulation.Physics;

namespace VehicleSimulation
{
    /// <summary>
    /// Sample showing how to use get the height of a programmatically generated
    /// heightmap.
    /// </summary>
    public class VehicleSimulation : DrawableGameComponent
    {
        #region Fields

        /// <summary>How much the camera's position is offset from the tank.</summary>
        readonly Vector3 CameraPositionOffset = new Vector3(0, 40, 150);
        /// <summary>The point the camera will aim at. This value is an offset from the tank's position.</summary>
        readonly Vector3 CameraTargetOffset = new Vector3(0, 30, 0);

        GraphicsDevice graphics;

        Sky sky;
        Terrain terrain;
        Tank tank;
        Tank tank2;
        private HarborBuoy harborBuoy;
        private GrandMassiveMonster monster;

        Matrix projectionMatrix;
        Matrix viewMatrix;

        private float oldFacingDirection;

        AudioManager audioManager;

        #endregion

        #region Constructor

        public VehicleSimulation(Game game, GraphicsDevice graphicsDevice) : base(game)
        {
//            this.graphics = new GraphicsDeviceManager(this);
            this.graphics = graphicsDevice;

//            if (GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware).MaxPixelShaderProfile < ShaderProfile.PS_2_0)
//            {
//                if (GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware).MaxPixelShaderProfile < ShaderProfile.PS_1_1)
//                {
//                    throw new NotSupportedException("Nepalaiko XNA PS_1.1");
//                }
//                //graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(SetToReference);
//            }

//            this.graphics.PreferredBackBufferWidth = 1024;
//            this.graphics.PreferredBackBufferHeight = 768;
            //this.graphics.IsFullScreen = true;

//            this.Game.Content.RootDirectory = "Content";

            this.tank = new Tank(this.Game);
            this.tank2 = new Tank(this.Game);
            this.tank2.position = new Vector3(0,-250,200);
            this.Game.Components.Add(this.tank);
            this.Game.Components.Add(this.tank2);

            this.harborBuoy = new HarborBuoy(this.Game);
            this.Game.Components.Add(harborBuoy);

            this.monster = new GrandMassiveMonster(this.Game);
            this.Game.Components.Add(this.monster);

            this.audioManager = new AudioManager(this.Game);
            this.Game.Components.Add(this.audioManager);
            this.tank.AudioManager = this.audioManager;
        }

        #endregion

        #region Initialization

        //+
        public override void Initialize()
        {
            // Calculate the aspect ratio (http://en.wikipedia.org/wiki/Aspect_ratio_(image))
            float aspectRatio = this.graphics.Viewport.Width / this.graphics.Viewport.Height;
            // Use aspectRatio to create the projection matrix.
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1f, 10000);

            base.Initialize();

            this.LoadContent();
        }
        //+
        protected override void LoadContent()
        {
            this.sky = this.Game.Content.Load<Sky>("sky");
            this.terrain = this.Game.Content.Load<Terrain>("terrain");

            base.LoadContent();

            this.graphics.Reset();
        }

        #endregion

        #region Update
        //+
        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            HandleInput();

            UpdateCamera();



//            audioManager.Listener.Position = cameraPosition;
//            audioManager.Listener.Forward = cameraForward;
//            audioManager.Listener.Up = cameraUp;
//            audioManager.Listener.Velocity = cameraVelocity;



            base.Update(gameTime);
        }

        /// <summary>
        /// this function will calculate the camera's position and the position of 
        /// its target. From those, we'll update the viewMatrix.
        /// </summary>
        private void UpdateCamera()
        {
            // The camera's position depends on the tank's facing direction: when the
            // tank turns, the camera needs to stay behind it. So, we'll calculate a
            // rotation matrix using the tank's facing direction, and use it to
            // transform the two offset values that control the camera.
            Matrix cameraFacingMatrix = Matrix.CreateRotationY(this.oldFacingDirection);
            Vector3 positionOffset = Vector3.Transform(this.CameraPositionOffset, cameraFacingMatrix);
            Vector3 targetOffset = Vector3.Transform(this.CameraTargetOffset, cameraFacingMatrix);

            if (this.tank.FacingDirection != this.oldFacingDirection)
            {
                this.oldFacingDirection += (this.tank.FacingDirection - this.oldFacingDirection)*0.05f;
            }

            // once we've transformed the camera's position offset vector, it's easy to
            // figure out where we think the camera should be.
            Vector3 cameraPosition = this.tank.Position + positionOffset;

            // We don't want the camera to go beneath the heightmap, so if the camera is
            // over the terrain, we'll move it up.
            if (this.terrain.IsOnHeightmap(cameraPosition))
            {
                // we don't want the camera to go beneath the terrain's height +
                // a small offset.
                float minimumHeight;
                Vector3 normal;
                this.terrain.GetHeightAndNormal(cameraPosition, out minimumHeight, out normal);

                minimumHeight += this.CameraPositionOffset.Y;

                if (cameraPosition.Y < minimumHeight)
                {
                    cameraPosition.Y = minimumHeight;
                }
            }

            // next, we need to calculate the point that the camera is aiming it. That's
            // simple enough - the camera is aiming at the tank, and has to take the 
            // targetOffset into account.
            Vector3 cameraTarget = this.tank.Position + targetOffset;


            // with those values, we'll calculate the viewMatrix.
            this.viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }

        /// <summary>
        /// Handles input for quitting the game.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                //Exit();
            }

            this.tank.HandleInput(currentKeyboardState, this.terrain);
            this.tank.HandleColisions(this.Game.Components);
        }

        #endregion

        //+
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = this.graphics;

            device.Clear(Color.Black);

            

            // If there was any alpha blended translucent geometry in
            // the scene, that would be drawn here.

            this.sky.Draw(this.viewMatrix, this.projectionMatrix);

            this.harborBuoy.Draw(this.viewMatrix, this.projectionMatrix);
            this.monster.Draw(this.viewMatrix, this.projectionMatrix);

            this.terrain.Draw(this.viewMatrix, this.projectionMatrix);

            this.tank.Draw(this.viewMatrix, this.projectionMatrix);
            this.tank2.Draw(this.viewMatrix, this.projectionMatrix);

            device.Present();

            base.Draw(gameTime);
        }
    }
}
