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



namespace AnimatedSprites
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        UserControlledSprite player;
        List<Sprite> spriteList = new List<Sprite>();
        List<AutomatedSprite> livesList = new List<AutomatedSprite>();

        int likelihoodAutomated = 75;
        int likelihoodChasing = 20;

        int automatedSpritePointValue = 10;
        int chasingSpritePointValue = 20;
        int catchingEvadingSpriteValue = 100;

        //random enemy spawns
        int enemySpawnMinMilliseconds = 1000;
        int enemySpawnMaxMilliseconds = 2000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 6;

        int nextSpawnTime = 0;

        int powerUpExpiration = 0;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            ResetSpawnTime();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>(@"Images/Sprites/Serenity"),
                new Vector2 (Game.Window.ClientBounds.Width/2,
                    Game.Window.ClientBounds.Height/2), new Point(150, 70), 10, new Point(0, 0),
                new Point(10, 1), new Vector2(6, 6));

            for (int i = 0; i < ((Main)Game).NumberLivesRemaining; ++i)
            {
                int offset = 10 + i * 80;
                livesList.Add(new AutomatedSprite(
                    Game.Content.Load<Texture2D>(@"Images/Sprites/Serenity"),
                    new Vector2(offset, 35), new Point(150, 70), 10,
                    new Point(0,0), new Point(1, 1), Vector2.Zero, null,0, .5f));
            }

                base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                SpawnEnemy();
               
                //Reset the spawn timer
                ResetSpawnTime();
            }
            UpdateSprites(gameTime);
            CheckPowerUpExpiration(gameTime);
            base.Update(gameTime);
        }
        protected void UpdateSprites(GameTime gameTime)
        {
             player.Update(gameTime, Game.Window.ClientBounds);

            //Update all sprites
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];

                s.Update(gameTime, Game.Window.ClientBounds);

                //check for collision
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    if (s.collisionCueName != null)
                        ((Main)Game).PlayCue(s.collisionCueName);
                    

                    //If collided with an automatedSprite
                    //remove a life from the player
                    if (s is AutomatedSprite)
                    {
                        if (livesList.Count > 0)
                            livesList.RemoveAt(livesList.Count - 1);
                        --((Main)Game).NumberLivesRemaining;
                    }
                    else if (s.collisionCueName == "skullcollision")
                    {
                        // Collided with plus - start plus power-up
                        powerUpExpiration = 5000;
                        player.ModifyScale(2);
                    }
                    else if (s.collisionCueName == "pluscollision")
                    {
                        // Collided with skull - start skull power-up
                        powerUpExpiration = 5000;
                        player.ModifySpeed(.5f);
                    }
                    else if (s.collisionCueName == "boltcollision")
                    {
                        //Collided with bolt - start bolt power-up
                        powerUpExpiration = 5000;
                        player.ModifySpeed(2);
                    }
                    else if (s is EvadingSprite)
                    {
                       ((Main)Game).AddScore(spriteList[i].scoreValue);
                    }
                   
                    //Remove the collided sprite from the game
                    spriteList.RemoveAt(i);
                    --i;
                }

                //Remove the object if its outofBounds
                if (s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    ((Main)Game).AddScore(spriteList[i].scoreValue);
                    spriteList.RemoveAt(i);
                    --i;
                }
                foreach (Sprite sprite in livesList)
                {
                    sprite.Update(gameTime, Game.Window.ClientBounds);
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //Draw Player
            player.Draw(gameTime, spriteBatch);

            //Draw all sprites
            foreach (Sprite s in spriteList)
            {
                s.Draw(gameTime, spriteBatch);
            }

            foreach (Sprite sprite in livesList)
            {
                sprite.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void ResetSpawnTime()
        {
            nextSpawnTime = ((Main)Game).rnd.Next(
                enemySpawnMinMilliseconds,
                enemySpawnMaxMilliseconds);
        }
        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;

            //Default frameSize
            Point frameSize = new Point(72, 84);

            //Randomly choose which side of the screen to place the enemy
            //then randomly create a position along that side of the screen
            //and randomly choose a speed for the enemy
            switch (((Main)Game).rnd.Next(4))
            {
                case 0://LEFT TO RIGHT
                    position = new Vector2(
                        -frameSize.X, ((Main)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                        - frameSize.Y));

                    speed = new Vector2(((Main)Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed), 0);
                    break;

                case 1:
                    position = new Vector2(
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                        ((Main)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight
                        - frameSize.Y));

                    speed = new Vector2(-((Main)Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed), 0);
                    break;

               case 2:
                    position = new Vector2(((Main)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth
                        - frameSize.X),
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

                    speed = new Vector2(0, -((Main)Game).rnd.Next(
                        enemyMinSpeed, enemyMaxSpeed));
                    break;

                case 3:
                    position = new Vector2(((Main)Game).rnd.Next(0,
                        Game.GraphicsDevice.PresentationParameters.BackBufferWidth - frameSize.X),
                        -frameSize.Y);

                    speed = new Vector2(0, ((Main)Game).rnd.Next(enemyMinSpeed,
                        enemyMaxSpeed));
                    break;
               
            }
            //Get Random Number
            int random = ((Main)Game).rnd.Next(100);
            if (random < likelihoodAutomated)
            {
                //Create AutomatedSprite
                //Get new Random number to determine whether to
                //create a three-blade or four-blade sprite
                if (((Main)Game).rnd.Next(2) == 0)
                {
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/Sprites/ReaverSprite"),
                            position, new Point(150, 70), 10, new Point(0, 0), new Point(3, 1),
                            speed, "fourbladescollision", automatedSpritePointValue));
                }
                else 
                {
                    spriteList.Add(
                        new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/Sprites/meteorEnemy"),
                            position, new Point(89, 84), 10, new Point(0, 0), new Point(6, 1), 
                            speed, "threebladescollision", automatedSpritePointValue));
                }
            }
            else if (random < likelihoodAutomated + likelihoodChasing)
            {
                //Create ChasingSprite
                spriteList.Add(
                    new ChasingSprite(Game.Content.Load<Texture2D>(@"Images/Sprites/RocketSprite-3"),
                        position, new Point(68, 33), 10, new Point(0, 0), new Point(6, 1),
                        speed, "skullcollision", this, chasingSpritePointValue));
            }
            else
            {
                //Create EvadingSprite
                spriteList.Add(
                    new EvadingSprite(Game.Content.Load<Texture2D>(@"Images/Sprites/SpeedSprite"),
                        position, new Point(55, 42), 10, new Point(0, 0), new Point(6, 1),
                        speed, "pluscollision", this, .75f, 150, catchingEvadingSpriteValue));
            }
        }

        public Vector2 GetPlayerPosition()
        {
            return player.GetPosition;
        }

        protected void CheckPowerUpExpiration(GameTime gameTime)
        {
            //Is a power-up active?
            if (powerUpExpiration > 0)
            {
                //Decrement power-up timer
                powerUpExpiration -= gameTime.ElapsedGameTime.Milliseconds;
                if (powerUpExpiration <= 0)
                {
                    //If power-up timer has expired, end all power-ups
                    powerUpExpiration = 0;
                    player.ResetScale();
                    player.ResetSpeed();
                }
            }
        }
    }
}
