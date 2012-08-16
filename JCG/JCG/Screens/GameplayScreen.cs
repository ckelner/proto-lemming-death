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
using System.Collections.Generic;

//farseer shit
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using PLD.Hero;
using PLD.GameWorld;
using PLD.Camera;

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
        private KeyboardState _oldKeyState;
        // static vars
        private static float MeterInPixels = 64f;
        // Simple camera controls
        private Camera2d _camera;
        private Vector2 _screenCenter;
        // The Hero
        Hero theHero = new Hero();
        // The World
        gameWorld gWorld;

        #endregion

        #region Initialization
        public GameplayScreen(String name)
        {
            HumanReadableName = name;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            gWorld = new gameWorld(new Vector2(0,20), MeterInPixels, 10000f, 5000f);
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            
            spriteBatch = ScreenManager.SpriteBatch;
            gameFont = content.Load<SpriteFont>("fonts/gamefont");

            // Convert screen center from pixels to meters
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            // init hero
            theHero.createHero(_screenCenter, gWorld.MeterInPixels, gWorld._world, content, 33f, 63f, 1f, BodyType.Dynamic, 0.3f, 0.5f, 0.0f, 0.01f);

            // init level
            gWorld.createDemoLevel(content, _screenCenter);

            // this has to be here for graphics manager
            _camera = new Camera2d(ScreenManager.GraphicsDevice.Viewport, (int)gWorld.width, (int)gWorld.height, 1f);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

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
            base.Update(gameTime, otherScreenHasFocus, false);

            // update the world - this is the physics engine
            gWorld._world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            // update camera position
            _camera.Pos = theHero.getPosView(MeterInPixels);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
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

            // let the hero noodle on the input as well
            theHero.handleInput(keyboardState, _oldKeyState);

            // Kelner - store to know old state
            _oldKeyState = keyboardState;

            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            // MAKE IT BLACK
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            // Keldog - Let's tile this shizzle my nizzle
            // spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetTransformation());
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, _camera.GetTransformation());
            
            // Draw Level
            gWorld.drawDemoLevel(spriteBatch);

            // Draw Hero
            theHero.draw(MeterInPixels, spriteBatch);

            spriteBatch.End();
        }
        
        #endregion
    }
}
