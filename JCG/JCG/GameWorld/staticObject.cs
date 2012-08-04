using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace PLD.GameWorld
{
    class staticObject
    {
        #region vars

        public Texture2D _sprite;
        public Body _body;

        #endregion

        public staticObject(Texture2D aSprite, Vector2 pos, World _world, float MeterInPixels,
            Boolean _static, float res, float fric, float width, float height, float _density)
        {
            _sprite = aSprite;
             // Kelner - World, width, height, density, and position
            _body = BodyFactory.CreateRectangle(_world, width / MeterInPixels, height / MeterInPixels, _density, pos);
            // unmoving
            _body.IsStatic = _static;
            // bounce and friction
            _body.Restitution = res;
            _body.Friction = fric;
        }
    }
}
