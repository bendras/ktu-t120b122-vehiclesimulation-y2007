using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using VehicleSimulation.Objects.Terrain;
using VehicleSimulation.Physics;

namespace VehicleSimulation.Objects.Entities
{
    class Tank : DrawableEntity
    {
        #region Fields

        private readonly float TankVelocity = 5;
        private readonly float TankWheelRadius = 18;
        private readonly float TankTurnSpeed = .025f;

        // The tank's model - a fearsome sight.
        Model model;

        // How is the tank oriented? Calculate based on the user's input and heightmap's normals.
        Matrix orientation = Matrix.Identity;
        public Vector3 oldPosition = new Vector3(0, 0, 0);
        public Vector3 position = new Vector3(0, 0, 0);
        private float facingDirection;

        // Use this value when making the wheels roll. It's calculated based on the distance moved.
        Matrix wheelRollMatrix = Matrix.Identity;
        // Pointer to wheels bones in "this.model.Bones[]". ("Simple Animation Sample")
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        // The original transform matrix for each animating bone.
        Matrix initialLeftBackWheelTransform;
        Matrix initialRightBackWheelTransform;
        Matrix initialLeftFrontWheelTransform;
        Matrix initialRightFrontWheelTransform;
        private AudioManager audioManager;

        #endregion

        public Tank(Game game) : base(game)
        {
        }

        #region Properties

        /// <summary> The position of the tank. The camera will use this value to position itself.</summary>
        public Vector3 Position
        {
            get { return position; }            
        }

        /// <summary>The direction that the tank is facing, in radians. </summary>
        public float FacingDirection
        {
            get { return facingDirection; }            
        }

        public AudioManager AudioManager
        {
            get { return audioManager; }
            set { audioManager = value; }
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            this.model = this.Game.Content.Load<Model>("Tank");
            base.Model_ = this.model;
            base.Position_ = this.position;

            this.leftBackWheelBone = this.model.Bones["l_back_wheel_geo"];
            this.rightBackWheelBone = this.model.Bones["r_back_wheel_geo"];
            this.leftFrontWheelBone = this.model.Bones["l_front_wheel_geo"];
            this.rightFrontWheelBone = this.model.Bones["r_front_wheel_geo"];

            this.initialLeftBackWheelTransform = this.leftBackWheelBone.Transform;
            this.initialRightBackWheelTransform = this.rightBackWheelBone.Transform;
            this.initialLeftFrontWheelTransform = this.leftFrontWheelBone.Transform;
            this.initialRightFrontWheelTransform = this.rightFrontWheelBone.Transform;
        }


        #region Update and Draw

        /// <summary>
        /// This function is called when the game is Updating in response to user input.
        /// It'll move the tank around the heightmap, and update all of the tank's 
        /// necessary state.
        /// </summary>
        public void HandleInput(KeyboardState currentKeyboardState, Terrain.Terrain terrainInfo)
        {
            if (!this.Enabled)
            {
                return;
            }

            float turnAmount = 0f;
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            {
                turnAmount += 1;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            {
                turnAmount -= 1;
            }

            


            Vector3 movement = Vector3.Zero;
//            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
//            {
//                movement.Z = Acceleration.AccelerateForward(movement.Z);
//            }
//            if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
//            {
//                movement.Z = Acceleration.AccelerateBackward(movement.Z);
//            }

            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
            {
                audioManager.PlayEngine();
            }
            else
            {
                audioManager.StopPlayEngine();
            }

            movement.Z = Acceleration.Accelerate(currentKeyboardState.GetPressedKeys());


            if (movement.Z > 0)
            {
                turnAmount = -turnAmount;
            }

            facingDirection += turnAmount * this.TankTurnSpeed;


            // next, we'll create a rotation matrix from the direction the tank is 
            // facing, and use it to transform the vector.
            this.orientation = Matrix.CreateRotationY(this.facingDirection);
            Vector3 velocity = Vector3.Transform(movement, this.orientation);
            velocity *= this.TankVelocity;

            // Now we know how much the user wants to move. We'll construct a temporary
            // vector, newPosition, which will represent where the user wants to go. If
            // that value is on the heightmap, we'll allow the move.
            Vector3 newPosition = this.position + velocity;
            if (terrainInfo.IsOnHeightmap(newPosition))
            {
                // now that we know we're on the heightmap, we need to know the correct
                // height and normal at this position.
                Vector3 normal;
                terrainInfo.GetHeightAndNormal(newPosition, out newPosition.Y, out normal);


                // As discussed in the doc, we'll use the normal of the heightmap
                // and our desired forward direction to recalculate our orientation
                // matrix. It's important to normalize, as well.
                this.orientation.Up = normal;

                this.orientation.Right = Vector3.Cross(this.orientation.Forward, this.orientation.Up);
                this.orientation.Right = Vector3.Normalize(this.orientation.Right);

                this.orientation.Forward = Vector3.Cross(this.orientation.Up, this.orientation.Right);
                this.orientation.Forward = Vector3.Normalize(this.orientation.Forward);

                // now we need to roll the tank's wheels "forward." to do this, we'll
                // calculate how far they have rolled, and from there calculate how much
                // they must have rotated.
                float distanceMoved = Vector3.Distance(this.position, newPosition);
                float theta = distanceMoved / this.TankWheelRadius;
                int rollDirection = movement.Z > 0 ? 1 : -1;

                this.wheelRollMatrix *= Matrix.CreateRotationX(theta * rollDirection);

                // once we've finished all computations, we can set our position to the
                // new position that we calculated.
                this.oldPosition = this.position;
                this.position = newPosition;
                base.Position_ = this.position;
            }
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            RotateWheels();

            // Get all bone transformations.
            Matrix[] boneTransformations = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(boneTransformations);
            
//            Matrix worldMatrix = this.orientation * Matrix.CreateTranslation(this.position);
            Matrix worldMatrix = this.orientation *Matrix.CreateTranslation(this.position);

            Effect effect1 = null;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransformations[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.EnableDefaultLighting();




//                    effect.PreferPerPixelLighting = true;
//
//                    // Set the fog to match the black background color
//                    effect.FogEnabled = true;
//                    effect.FogColor = Vector3.Zero;
//                    effect.FogStart = 1000;
//                    effect.FogEnd = 3200;
                }
                mesh.Draw();
            }
        }

        private void RotateWheels()
        {
            if (this.oldPosition == this.position)
            {
                return;
            }
            this.leftBackWheelBone.Transform = this.wheelRollMatrix * this.initialLeftBackWheelTransform;
            this.rightBackWheelBone.Transform = this.wheelRollMatrix * this.initialRightBackWheelTransform;
            this.leftFrontWheelBone.Transform = this.wheelRollMatrix * this.initialLeftFrontWheelTransform;
            this.rightFrontWheelBone.Transform = this.wheelRollMatrix * this.initialRightFrontWheelTransform;
        }

        #endregion

        public void HandleColisions(GameComponentCollection components)
        {
            foreach (IGameComponent component in components)
            {
                DrawableEntity drawableEntity = component as DrawableEntity;
                if (drawableEntity == null)
                {
                    continue;
                }
                if (drawableEntity == this)
                {
                    continue;
                }

                if (drawableEntity.Intersects(this.BoundingSphere))
                {
                    this.position = this.oldPosition;

//                    audioManager.Play3DCue("boom");
                    audioManager.PlayBoom();
                }
            }
        }
    }
}