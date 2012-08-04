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
        
        #endregion

        public gameWorld(Vector2 gravity, float MIP)
        {
            MeterInPixels = MIP;
            _world = new World(gravity);
        }

        public void createDemoLevel(ContentManager content, Vector2 _screenCenter)
        {
            _demoLevel = new DemoLevel(this, content, _screenCenter);
        }

        public void drawDemoLevel(SpriteBatch spriteBatch)
        {
            _demoLevel.Draw(spriteBatch);
        }
    }
}
