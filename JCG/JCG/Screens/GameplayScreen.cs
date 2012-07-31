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
using PLD.Hero;

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
        // the world objects
        private Texture2D _groundSprite;
        private Body _groundBody;
        private Texture2D _smallStoneTexture;
        private Body _smallStone;
        private int smallStoneWidth = 10;
        private int smallStoneHeight = 5;
        private Texture2D _smallStoneGroundTexture;
        private Body _smallStoneGround;
        private int smallStoneGroundWidth = 10;
        private int smallStoneGroundHeight = 5;
        private const float MeterInPixels = 64f;
        private World _world;
        private KeyboardState _oldKeyState;
        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        // The Hero
        Hero theHero = new Hero();

        #endregion

        #region Initialization
        public GameplayScreen(String name)
        {
            HumanReadableName = name;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            _world = new World(new Vector2(0, 20));
            _cameraPosition = Vector2.Zero;
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            
            spriteBatch = ScreenManager.SpriteBatch;
            gameFont = content.Load<SpriteFont>("gamefont");

            // Initialize camera controls - Kelner - Needs improvement
            /** TODO: Camera follow Hero? **/
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            // Convert screen center from pixels to meters
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            // init hero
            theHero.createHero(_screenCenter, MeterInPixels, _world, content, 33f, 63f, 1f, BodyType.Dynamic, 0.3f, 0.5f, 0.0f, 0.01f);

            // init level
            createLevel();

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        // creates the level
        private void createLevel()
        {
            #region ground
            // load ground sprite
            _groundSprite = content.Load<Texture2D>("groundSprite"); // 505px x 58px =>   7.89m x 0.91m

            // Kelner - define the ground position for physics interaction
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 1.25f);

            // Create the ground fixture
            // Kelner - World, width, height, density, and position
            _groundBody = BodyFactory.CreateRectangle(_world, 505f / MeterInPixels, 58f / MeterInPixels, 1f, groundPosition);
            // unmoving
            _groundBody.IsStatic = true;
            // bounce and friction
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;
            #endregion ground

            #region stoneMidAir
            Vector2 smallStonePos = (_screenCenter / MeterInPixels) + new Vector2(.89f, 0);
            _smallStone = BodyFactory.CreateRectangle(_world, smallStoneWidth / MeterInPixels, smallStoneHeight / MeterInPixels, 1f, smallStonePos);
            _smallStone.IsStatic = true;
            // bounce and friction
            _smallStone.Restitution = 0.3f;
            _smallStone.Friction = 0.5f;

            _smallStoneTexture = new Texture2D(ScreenManager.GraphicsDevice, smallStoneWidth, smallStoneHeight);
            Color[] stoneMidAircolor = new Color[smallStoneWidth * smallStoneHeight];
            //loop through all the colors setting them to whatever values we want
            for (int i = 0; i < stoneMidAircolor.Length; i++)
            {
                stoneMidAircolor[i] = new Color(255, 255, 255, 255);
            }
            //set the color data on the texture
            _smallStoneTexture.SetData(stoneMidAircolor);
            #endregion

            #region stoneGround
            Vector2 smallStoneGroundPos = (_screenCenter / MeterInPixels) + new Vector2(-2.5f, 0.75f);
            _smallStoneGround = BodyFactory.CreateRectangle(_world, smallStoneGroundWidth / MeterInPixels, smallStoneGroundHeight / MeterInPixels, 1f, smallStoneGroundPos);
            _smallStone.IsStatic = true;
            // bounce and friction
            _smallStoneGround.Restitution = 0.3f;
            _smallStoneGround.Friction = 0.5f;

            _smallStoneGroundTexture = new Texture2D(ScreenManager.GraphicsDevice, smallStoneGroundWidth, smallStoneGroundHeight);
            Color[] stoneGroundcolor = new Color[smallStoneGroundWidth * smallStoneGroundHeight];
            //loop through all the colors setting them to whatever values we want
            for (int i = 0; i < stoneGroundcolor.Length; i++)
            {
                stoneGroundcolor[i] = new Color(255, 255, 255, 255);
            }
            //set the color data on the texture
            _smallStoneGroundTexture.SetData(stoneGroundcolor);
            #endregion
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
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

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
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            
            // Draw level
            drawLevel();

            // Draw Hero
            theHero.draw(MeterInPixels, spriteBatch);

            spriteBatch.End();
        }

        private void drawLevel()
        {
            /* Ground position and origin */
            Vector2 groundPos = _groundBody.Position * MeterInPixels;
            Vector2 groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);

            //Draw Ground
            spriteBatch.Draw(_groundSprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Stone Mid Air
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStonePos = _smallStone.Position * (MeterInPixels - 0.70f);
            spriteBatch.Draw(_smallStoneTexture, smallStonePos, null, Color.White, 0f, new Vector2(0,0), 1f, SpriteEffects.None, 0f);

            //Draw Stone Ground
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStoneGroundPos = _smallStoneGround.Position * (MeterInPixels - 0.70f);
            spriteBatch.Draw(_smallStoneGroundTexture, smallStoneGroundPos, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
        }
        
        #endregion
    }
}
