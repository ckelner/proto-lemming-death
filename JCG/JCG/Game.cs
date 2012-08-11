#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;
#endregion

namespace PLD
{
    public class LemmingDeath : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        SpriteBatch spriteBatch;

        //added by Lawrence
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        //added by Lawrence
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }
        
        #region Helper Functions
        // added by Lawrence
        // a handy little function that gives a random float between two
        // values. This will be used in several places for the particle sytem, in particilar in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        #endregion

        public LemmingDeath()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // 720p window
            graphics.PreferredBackBufferWidth = 854;
            graphics.PreferredBackBufferHeight = 480;
            Window.Title = "Lemming Death Prototype";

            // Create the screen manager component
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screens
            screenManager.AddScreen(new BackgroundScreen("background"), null);
            screenManager.AddScreen(new MainMenuScreen("mainmenu"), null);

            #if(DEBUG)
            // Framerate adjustment
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            #endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }

#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LemmingDeath game = new LemmingDeath())
            {
                game.Run();
            }
        }
    }
#endif
}
