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
using PLD;
using System.Collections.Generic;
#endregion

namespace GameStateManagement
{
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        float pauseAlpha;
        Texture2D backgroundFarTexture;
        Texture2D backgroundNearTexture;
        SpriteBatch spriteBatch;
        private Camera camera;
        private List<Layer> layers;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(String name)
        {
            HumanReadableName = name;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            
            spriteBatch = ScreenManager.SpriteBatch;

            gameFont = content.Load<SpriteFont>("gamefont");
            backgroundFarTexture = content.Load<Texture2D>("mountain-parallax");
            backgroundNearTexture = content.Load<Texture2D>("moon-reflect-parallax");

            // Create a camera instance and limit its moving range
            camera = new Camera(ScreenManager.GraphicsDevice.Viewport) { Limits = new Rectangle(0, 0, 3200, 600) };

            // Create 2 layers with parallax ranging from 0% to 100% (only horizontal)
            layers = new List<Layer>
            {
                new Layer(camera) { Parallax = new Vector2(0.0f, 1.0f) },
                new Layer(camera) { Parallax = new Vector2(0.1f, 1.0f) },
            };

            // Add one sprite to each layer
            layers[0].Sprites.Add(new Sprite { Texture = backgroundFarTexture });
            layers[1].Sprites.Add(new Sprite { Texture = backgroundNearTexture });

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            moveCamera(gameTime);
        }

        /// <summary>
        /// updates the camera position based on keyboard input
        /// </summary>
        private void moveCamera(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Right))
                camera.Move(new Vector2(400.0f * elapsedTime, 0.0f), true);

            if (keyboardState.IsKeyDown(Keys.Left))
                camera.Move(new Vector2(-400.0f * elapsedTime, 0.0f), true);

            if (keyboardState.IsKeyDown(Keys.Down))
                camera.Move(new Vector2(0.0f, 400.0f * elapsedTime), true);

            if (keyboardState.IsKeyDown(Keys.Up))
                camera.Move(new Vector2(0.0f, -400.0f * elapsedTime), true);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen("pause"), ControllingPlayer);
            }
            base.HandleInput(input);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            // Center the text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            Color color = Color.White * TransitionAlpha;
            

            // Draw
            DrawParallaxBGs();
            spriteBatch.Begin();
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the parallaxing backgrounds
        /// </summary>
        private void DrawParallaxBGs()
        {
            foreach (Layer layer in layers)
                layer.Draw(spriteBatch);
        }
        #endregion
    }
}
