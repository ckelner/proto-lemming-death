using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics.Contacts;

namespace PLD.Hero
{
    class Hero
    {
        #region varvarvarrrrrrriables

        // the hero objects
        private Texture2D _heroSprite;
        private Body _heroBody;
        private bool hasJumpedTwice = false;

        //static vars
        private static string _userData = "Hero";

        #endregion

        /// <summary>
        /// Creates the hero on the screen (doesn't do the actual drawing, but creates the hero object
        /// and places him in the physics world.  Defines his restitution, friction, linear damping,
        /// angular damping, Body Type, width, height, and density
        /// </summary>
        /// <param name="_screenCenter">Center of the screen - currently where our hero starts</param>
        /// <param name="MeterInPixels">conversion value to go from meters (physics) to pixels (screen)</param>
        /// <param name="_world">the world physics object</param>
        /// <param name="content">Graphics contents</param>
        /// <param name="width">hero width</param>
        /// <param name="height">hero height</param>
        /// <param name="density">hero density</param>
        /// <param name="bodyType">hero farseer body type</param>
        /// <param name="restitution">The coefficient of restitution (COR) of two colliding objects is a fractional value representing the 
        /// ratio of speeds after and before an impact, taken along the line of the impact. Pairs of objects with COR = 1 collide elastically, 
        /// while objects with COR less than 1 collide inelastically. For a COR = 0, the objects effectively "stop" at the collision, not bouncing at all.</param>
        /// <param name="friction">Slipe and Slide or stick like glue</param>
        /// <param name="lineardamping">In physics, damping is an effect that reduces the amplitude of oscillations in an oscillatory system: http://en.wikipedia.org/wiki/Damping </param>
        /// <param name="angulardamping">In physics, damping is an effect that reduces the amplitude of oscillations in an oscillatory system: http://en.wikipedia.org/wiki/Damping </param>
        public void createHero(Vector2 _screenCenter, float MeterInPixels, World _world, ContentManager content,
            float width, float height, float density, BodyType bodyType, float restitution, float friction,
            float lineardamping, float angulardamping)
        {
            // Load Sprite
            // TODO: Expand...
            _heroSprite = content.Load<Texture2D>("rectangleSprite"); //  33px x 63px => 0.52m x 1m

            // Vector2 heroPosition = (_screenCenter / MeterInPixels) + new Vector2(0, -1.5f);
            Vector2 heroPosition = (_screenCenter / MeterInPixels);

            // Create the circle fixture
            // Kelner - This is World, width, height Body Density (for PHYSICS!), and starting position)
            _heroBody = BodyFactory.CreateRectangle(_world, width / MeterInPixels, height / MeterInPixels, density, heroPosition);

            // Kelner - Dynamic Bodytype is Positive Mass, non-zero velocity determined by forces, moved by solver
            // This is something that can move versus something that can not be moved
            _heroBody.BodyType = bodyType;

            // Give it some bounce and friction
            _heroBody.Restitution = restitution;
            _heroBody.Friction = friction;

            // Kelner - dampening when in the air
            _heroBody.LinearDamping = lineardamping;
            _heroBody.AngularDamping = angulardamping;

            _heroBody.FixedRotation = true;

            _heroBody.OnCollision += handleCollision;

            _heroBody.UserData = _userData;

            foreach (Fixture fix in _heroBody.FixtureList)
            {
                fix.UserData = _userData;
            }
        }

        /// <summary>
        /// Takes in input from the game screen and applies it to the hero - generally means move, act, attack, jump, etc
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <param name="_oldKeyState">Old keyboard state - important to know what we were doing one cycle before</param>
        public void handleInput(KeyboardState keyboardState, KeyboardState _oldKeyState)
        {
            // We make it possible to rotate the hero (NOT) [old]
            // We have prevented this behavior - the hero should stay upright
            // This now only moves the hero left or right (A or D)
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _heroBody.ApplyForce(new Vector2(-10));
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _heroBody.ApplyForce(new Vector2(10, 0));
            }

            // Kelner - This allows us to double jump!
            // TODO: Limit to a single double jump
            if (keyboardState.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
            {
                if (!hasJumpedTwice)
                {
                    _heroBody.ApplyLinearImpulse(new Vector2(0, -2));
                    hasJumpedTwice = true;
                }
            }
            Data.xPhyPos = _heroBody.Position.X;
            Data.yPhyPos = _heroBody.Position.Y;
        }

        /// <summary>
        /// Draws the hero
        /// </summary>
        /// <param name="MeterInPixels">Meters to pixels conversion</param>
        /// <param name="spriteBatch">ref (is it?  do we need "ref" here?? google was not my friend -- TODO: Find out)</param>
        public void draw(float MeterInPixels, SpriteBatch spriteBatch)
        {
            // get hero starting position
            Vector2 heroOrigin = new Vector2(_heroSprite.Width / 2f, _heroSprite.Height / 2f);
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 heroPos = _heroBody.Position * MeterInPixels;
            // Update data
            Data.xPixPos = heroPos.X;
            Data.yPixPos = heroPos.Y;
            // gets the hero angle (should never change)
            float heroRotation = _heroBody.Rotation;
            //Draw hero
            spriteBatch.Draw(_heroSprite, heroPos, null, Color.White, heroRotation, heroOrigin, 1f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// gives the hero's position in the physics world
        /// </summary>
        /// <returns></returns>
        public Vector2 getPos()
        {
            return _heroBody.Position;
        }

        /// <summary>
        /// gives the hero's position in the view
        /// </summary>
        /// <returns></returns>
        public Vector2 getPosView(float MeterInPixels)
        {
            return _heroBody.Position * MeterInPixels;
        }

        public bool handleCollision(Fixture b1, Fixture b2, Contact c)
        {
            // IT FUCKING SHOULD
            if(b1.UserData.ToString().ToLower() == "hero")
            {
                //THEN WHAT THE HELL DID WE COLLIDE WITH
                switch(b2.UserData.ToString().ToLower())
                {
                    case "ground":
                        resetShitHittingGround();
                        break;
                    case "ground-wall": // the fuck we figure out if it hit the tops?
                        // ? Chris Kelner no fucking clues TODO
                        resetShitHittingGround();
                        break;
                }
            }
            // make that fucking collidisplosion happens yo
            // Chris Kelner DRINK A FUCKING THON - GET OFF MY BALLZ
            return true;
        }

        public void resetShitHittingGround()
        {
            hasJumpedTwice = false;
        }
    }
}
