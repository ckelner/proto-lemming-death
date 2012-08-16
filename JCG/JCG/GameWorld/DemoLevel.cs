using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace PLD.GameWorld
{
    class DemoLevel
    {
        #region vars

        private staticObject _startGround;
        private staticObject _midAirObject;
        private staticObject _groundObject;
        private gameWorld gWorld;

        #endregion
        
        /// <summary>
        /// Intializes the Demo Level
        /// Takes in gameWorld which is an instance of "gameWorld"
        /// content is the content manager from gameplayscreen
        /// screencenter is the center of the screen
        /// </summary>
        /// <param name="gameWorld"></param>
        /// <param name="content"></param>
        /// <param name="_screenCenter"></param>
        public DemoLevel(gameWorld gameWorld, ContentManager content, Vector2 _screenCenter)
        {
            // get the world
            gWorld = gameWorld;

            #region staticObjects

            //First/Starting ground/level
            float w = 5505f; // THIS IS PIXELS - GETS CONVERTED IN STATIC OBJECT
            float wHalf = (5505f / gWorld.MeterInPixels) / 2;
            float viewX = _screenCenter.X / gWorld.MeterInPixels;
            float offset = wHalf - viewX;

            // gfx 512 x 360 - What we gotta do?
            // 360 / 64 ? = 317??? how's that work... ?
            // TODO: FIGURE IT THE FUCK OUT
            // KELNER - It also ain't getting drawn right... too far left... (starting position)
            _startGround = new staticObject(content.Load<Texture2D>("World/ground2"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(offset, 1.25f),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                w,
                318f,
                1f,
                "ground");

            // MidAir Object (?)
            _midAirObject = new staticObject(content.Load<Texture2D>("World/stoneSprite"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(.89f, 0),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                10f,
                5f,
                1f,
                "ground-wall");

            // Ground Object (?)
            _groundObject = new staticObject(content.Load<Texture2D>("World/stoneSprite"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(-2.5f, 0.75f),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                10f,
                5f,
                1f,
                "ground");

            #endregion
        }

        /// <summary>
        /// Draws the DemoLevel
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            /* Ground position and origin */
            // OK - Kelner - Aug 15 2012 - Something about this doesn't require the -.70f... whatever
            // THIS THE MOTHER FUCKING CENTER YALL
            // BUT WE WANT THE TOP LEFT FOR THE DRAW YO, YO, YO (I think?)
            Vector2 groundPos = _startGround._body.Position * (gWorld.MeterInPixels);
            //Vector2 groundOrigin = new Vector2(_startGround._sprite.Width / 2f, _startGround._sprite.Height / 2f);
            // INCLUDE THAT SHIT THAT IS WRONG .70 FLOATIES - NOPE - NOPE - NOPE - DON'T NEED IT
            Vector2 groundTopLeft = new Vector2(groundPos.X - ((_startGround._width / 2)), groundPos.Y - ((_startGround._height / 2) + 1f));

            //Draw Ground
            Rectangle ground = new Rectangle(((int)(groundPos.X - _startGround._width)), ((int)(groundPos.Y - _startGround._height)),
                (int)_startGround._width, (int)_startGround._height);
            spriteBatch.Draw(_startGround._sprite, groundTopLeft, ground, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //Draw Stone Mid Air
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStonePos = _midAirObject._body.Position * (gWorld.MeterInPixels - 0.70f);
            spriteBatch.Draw(_midAirObject._sprite, smallStonePos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //Draw Stone Ground
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStoneGroundPos = _groundObject._body.Position * (gWorld.MeterInPixels - 0.70f);
            spriteBatch.Draw(_groundObject._sprite, smallStoneGroundPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
