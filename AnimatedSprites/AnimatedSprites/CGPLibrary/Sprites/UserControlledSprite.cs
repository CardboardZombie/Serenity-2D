using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnimatedSprites
{
    class UserControlledSprite: Sprite
    {
        private bool flipped = false;

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            :base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, null, 0)
        {
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, null, 0)
        { 
        }

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    inputDirection.X -= 1;
                    flipped = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    inputDirection.X += 1;
                    flipped = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    inputDirection.Y += 1;

                return inputDirection * speed;
            }
        }
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite based on direction
            position += direction;
            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;

            base.Update(gameTime, clientBounds);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!flipped)
                base.Draw(gameTime, spriteBatch);
            else
                spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                    Color.White, 0, Vector2.Zero,
                    scale, SpriteEffects.FlipHorizontally, 0);
        }

    }
}
