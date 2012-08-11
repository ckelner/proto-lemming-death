using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace PLD
{
    class Animation
    {
        Texture2D spriteStrip;
        float scale;
        //time since last update, time of frame, number of frames, index of current frame
        int elapsedTime, frameTime, frameCount, currentFrame;
        //color of frame displayed
        Color color;
        //area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();
        //area in the game we want to display the image strip
        Rectangle destinationRect = new Rectangle();
        public int frameWidth;
        public int frameHeight;
        public bool active;
        //determines if animation will keep playing or deactivate after one run
        public bool looping;
        public Vector2 pos;

        public void Update() { }
        public void Draw() { }

        public void Initialize(Texture2D t, Vector2 p, int fwidth, 
            int fheight, int fcount, int ftime, Color c, float s, bool loop)
        {
            //keep a local copy of the values passed in
            this.color = c;
            this.frameWidth = fwidth;
            this.frameHeight = fheight;
            this.frameCount = fcount;
            this.frameTime = ftime;
            this.scale = s;

            looping = loop;
            pos = p;
            spriteStrip = t;

            //set time to zero
            elapsedTime = 0;
            currentFrame = 0;

            //active is true by default
            active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (active == false)
                return;

            //update elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //if elapsed time is larger than frame time, switch frames
            if (elapsedTime > frameTime)
            {
                currentFrame++;

                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (looping == false)
                        active = false;
                }
                elapsedTime = 0;
            }
            //grab the correct frame in image strip by (currentFrame index * frameWidth)
            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            //grab the correct frame in image strip by (currentFrame index * frame width)
            destinationRect = new Rectangle((int)pos.X - (int)(frameWidth * scale) / 2, 
                (int)pos.Y - (int)(frameHeight * scale) / 2, (int)(frameWidth * scale),
                (int)(frameHeight * scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);//might be "c" instead of "color"
        }
    }
}
