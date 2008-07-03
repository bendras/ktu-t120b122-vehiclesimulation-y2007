using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using VehicleSimulation.Objects.Terrain;
using VehicleSimulation.Physics;

namespace VehicleSimulation.Objects.Entities
{
    class GrandMassiveMonster : DrawableEntity
    {
        #region Fields

        private readonly float TankVelocity = 5;
        private readonly float TankWheelRadius = 18;
        private readonly float TankTurnSpeed = .025f;

        // The tank's model - a fearsome sight.
        Model model;

        // How is the tank oriented? Calculate based on the user's input and heightmap's normals.
        Matrix orientation = Matrix.Identity;
        private Vector3 position = new Vector3(-1000, 400, 1000);
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

        #endregion

        public GrandMassiveMonster(Game game) : base(game)
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

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            this.model = this.Game.Content.Load<Model>("GrandMassiveMonster");
            base.Model_ = this.model;
            base.Position_ = this.position;

            this.leftBackWheelBone = this.model.Bones[0];
            this.rightBackWheelBone = this.model.Bones[1];
//            this.leftFrontWheelBone = this.model.Bones["l_front_wheel_geo"];
//            this.rightFrontWheelBone = this.model.Bones["r_front_wheel_geo"];

            this.initialLeftBackWheelTransform = this.leftBackWheelBone.Transform;
            this.initialRightBackWheelTransform = this.rightBackWheelBone.Transform;
//            this.initialLeftFrontWheelTransform = this.leftFrontWheelBone.Transform;
//            this.initialRightFrontWheelTransform = this.rightFrontWheelBone.Transform;
        }


        #region Update and Draw

        /// <summary>
        /// This function is called when the game is Updating in response to user input.
        /// It'll move the tank around the heightmap, and update all of the tank's 
        /// necessary state.
        /// </summary>
        public void HandleInput(KeyboardState currentKeyboardState, Terrain.Terrain terrainInfo)
        {
            float turnAmount = 0f;
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            {
                turnAmount += 1;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            {
                turnAmount -= 1;
            }

            facingDirection += turnAmount * this.TankTurnSpeed;


            Vector3 movement = Vector3.Zero;
//            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
//            {
//                movement.Z = Acceleration.AccelerateForward(movement.Z);
//            }
//            if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
//            {
//                movement.Z = Acceleration.AccelerateBackward(movement.Z);
//            }

            movement.Z = Acceleration.Accelerate(currentKeyboardState.GetPressedKeys());

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
                position = newPosition;
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
            this.leftBackWheelBone.Transform = this.wheelRollMatrix * this.initialLeftBackWheelTransform;
            this.rightBackWheelBone.Transform = this.wheelRollMatrix * this.initialRightBackWheelTransform;
//            this.leftFrontWheelBone.Transform = this.wheelRollMatrix * this.initialLeftFrontWheelTransform;
//            this.rightFrontWheelBone.Transform = this.wheelRollMatrix * this.initialRightFrontWheelTransform;
        }

        #endregion
    }
}