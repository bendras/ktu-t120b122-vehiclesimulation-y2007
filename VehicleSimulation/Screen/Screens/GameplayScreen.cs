#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScreenManager=VehicleSimulation.Screen.ScreenManager;

#endregion

namespace VehicleSimulation.Screen.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        VehicleSimulation vehicleSimulation;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(Game game) : base(game)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.vehicleSimulation = new VehicleSimulation(game, game.GraphicsDevice);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            this.vehicleSimulation.Initialize();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (IsActive)
            {
                this.vehicleSimulation.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(this.Game));
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                for (int i = 0; i < InputState.MaxInputs; i++)
                {
                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Left))
                        movement.X--;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Right))
                        movement.X++;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Up))
                        movement.Y--;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Down))
                        movement.Y++;

                    Vector2 thumbstick = input.CurrentGamePadStates[i].ThumbSticks.Left;

                    movement.X += thumbstick.X;
                    movement.Y -= thumbstick.Y;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
//            // This game has a blue background. Why? Because!
//            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
//                                               Color.CornflowerBlue, 0, 0);
//
//            // Our player and enemy are both actually just text strings.
//            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
//
//            spriteBatch.Begin();
//
//            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);
//
//            spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
//                                   enemyPosition, Color.DarkRed);
//
//            spriteBatch.End();
//
//            // If the game is transitioning on or off, fade it out to black.
//            if (TransitionPosition > 0)
//                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);

            this.vehicleSimulation.Draw(gameTime);

        }


        #endregion
    }
}
