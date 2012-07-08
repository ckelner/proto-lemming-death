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

//farseer shit
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

#endregion

namespace GameStateManagement
{
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        float pauseAlpha;
        SpriteBatch spriteBatch;
        private Texture2D _circleSprite;
        private Texture2D _groundSprite;
        private const float MeterInPixels = 64f;
        private Body _circleBody;
        private Body _groundBody;
        private World _world;
        private KeyboardState _oldKeyState;
        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

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
            _world = new World(new Vector2(0, 20));
            _cameraPosition = Vector2.Zero;
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

            // Initialize camera controls - Kelner - Needs improvement
            /** TODO: Camera follow Hero? **/
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            // Convert screen center from pixels to meters
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f,
                                                ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            // Load sprites
            _circleSprite = content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m

            Vector2 circlePosition = (_screenCenter / MeterInPixels) + new Vector2(0, -1.5f);

            // Create the circle fixture
            // Kelner - This is World (duh), Body Radius (because we are declaring a circle), 
            // Body Density (for PHYSICS!), and starting position) 
            _circleBody = BodyFactory.CreateCircle(_world, 96f / (2f * MeterInPixels), 1f, circlePosition);
            // Kelner - Dynamic Bodytype is Positive Mass, non-zero velocity determined by forces, moved by solver
            // This is something that can move versus something that can not be moved
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            // Kelner - define the ground position for physics interaction
            // the texture itself is drawn later
            /** TODO - Kelner - I think this could be done better, I don't like the idea of keeping
             * the physics object seperate from the texture, otherwise you end up with problems like
             * the player falling through the world
             **/
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 1.25f);

            // Create the ground fixture
            // Kelner - World, width, height, density, and position
            _groundBody = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, groundPosition);
            // unmoving
            _groundBody.IsStatic = true;
            // bounce and friction
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;

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

            // update the world - this is the physics engine
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
        }

        /// <summary>
        /// updates the camera position based on keyboard input
        /// </summary>
        private void moveCamera(GameTime gameTime)
        {
            // Kelner - unused right now
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

            // get the keyboard state
            /** TODO - Kelner - Do gamepad shits **/
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

            // We make it possible to rotate the circle body (ROLLING YO!)
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _circleBody.ApplyTorque(-10);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _circleBody.ApplyTorque(10);
            }

            // Kelner - This allows us to double jump!
            if (keyboardState.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

            // Kelner - store to know old state
            _oldKeyState = keyboardState;

            base.HandleInput(input);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // MAKE IT BLACK
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            /* Circle position and rotation */
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 circlePos = _circleBody.Position * MeterInPixels;
            // gets the circle angle
            float circleRotation = _circleBody.Rotation;

            /* Ground position and origin */
            Vector2 groundPos = _groundBody.Position * MeterInPixels;
            Vector2 groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);

            // Align sprite center to body position
            Vector2 circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            //Draw circle
            spriteBatch.Draw(_circleSprite, circlePos, null, Color.White, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);
            //Draw ground
            spriteBatch.Draw(_groundSprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
        
        #endregion
    }
}
