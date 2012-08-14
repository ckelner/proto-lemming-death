using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PLD.GameWorld
{
    class gameWorld
    {
        #region classVars

        public World _world;
        private DemoLevel _demoLevel;
        public float MeterInPixels;
        // Chris Kelner - these are some outrageous values
        public float width;
        public float height;
        
        #endregion

        /// <summary>
        /// Creates a new game world.  Gameworld contains a Farseer world which is created
        /// when the class is instantiated with the specified gravity.  MIP is the meter
        /// in pixels value that Farseer runs in.
        /// </summary>
        /// <param name="gravity"></param>
        /// <param name="MIP"></param>
        public gameWorld(Vector2 gravity, float MIP, float wWidth, float wHeight)
        {
            MeterInPixels = MIP;
            _world = new World(gravity);
            width = wWidth;
            height = wHeight;
        }

        /// <summary>
        /// Creates a demo level.  Content is the graphics content manager.
        /// Screen center is a calculated value for the center of the screen (where the hero is at)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="_screenCenter"></param>
        public void createDemoLevel(ContentManager content, Vector2 _screenCenter)
        {
            _demoLevel = new DemoLevel(this, content, _screenCenter);
        }

        /// <summary>
        /// Called every time draw occurs during gameplay
        /// Spritebatch the current spritebatch (from GameplayScreen)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void drawDemoLevel(SpriteBatch spriteBatch)
        {
            _demoLevel.Draw(spriteBatch);
        }
    }
}
