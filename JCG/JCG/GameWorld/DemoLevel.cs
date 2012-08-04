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
                       
        public DemoLevel(gameWorld gameWorld, ContentManager content, Vector2 _screenCenter)
        {
            // get the world
            gWorld = gameWorld;

            #region staticObjects

            //First/Starting ground/level
            // sprite = 505px x 58px =>   7.89m x 0.91m
            _startGround = new staticObject(content.Load<Texture2D>("groundSprite"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(0, 1.25f),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                505f,
                58f,
                1f);

            // MidAir Object (?)
            _midAirObject = new staticObject(content.Load<Texture2D>("stoneSprite"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(.89f, 0),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                10f,
                5f,
                1f);

            // Ground Object (?)
            _groundObject = new staticObject(content.Load<Texture2D>("stoneSprite"),
                (_screenCenter / gWorld.MeterInPixels) + new Vector2(-2.5f, 0.75f),
                gWorld._world,
                gWorld.MeterInPixels,
                true,
                0.3f,
                0.5f,
                10f,
                5f,
                1f);

            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            /* Ground position and origin */
            Vector2 groundPos = _startGround._body.Position * gWorld.MeterInPixels;
            Vector2 groundOrigin = new Vector2(_startGround._sprite.Width / 2f, _startGround._sprite.Height / 2f);

            //Draw Ground
            spriteBatch.Draw(_startGround._sprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Stone Mid Air
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStonePos = _midAirObject._body.Position * (gWorld.MeterInPixels - 0.70f);
            spriteBatch.Draw(_midAirObject._sprite, smallStonePos, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //Draw Stone Ground
            // Kelner - For some reason you need to subtract from the value you are drawing
            // to get accurate with the physics object.  I am unsure if this scales well with larger objects
            // or not...  What I do know is that there is probably a better way to do this, maybe by 
            // refining the 'MeterInPixels' value or changing the physics object instead of the 
            // object that gets drawn.  Further investigation should be done...
            Vector2 smallStoneGroundPos = _groundObject._body.Position * (gWorld.MeterInPixels - 0.70f);
            spriteBatch.Draw(_groundObject._sprite, smallStoneGroundPos, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
        }
    }
}
