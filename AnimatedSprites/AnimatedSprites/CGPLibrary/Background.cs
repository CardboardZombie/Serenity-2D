using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class Background
    {
        public Texture2D texture;
        public Rectangle rectangle;
        public int speed;
        public float zDepth;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
        
    }
    class Scrolling : Background
    {
        public Scrolling(Texture2D newTexture, Rectangle newRectangle, int newSpeed, float newDepth)
        {
            texture = newTexture;
            rectangle = newRectangle;
            speed = newSpeed;
            zDepth = newDepth;
        }

        public void Update()
        {
            rectangle.X -= speed;
        }
    }
}
