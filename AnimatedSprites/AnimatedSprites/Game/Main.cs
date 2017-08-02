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
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont scoreFont;
        SpriteManager spriteManager;
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        int currentScore = 0;
        int numberLivesRemaining = 3;
        Texture2D backgroundTexture, menuTexture, endGame, level2, level3, level4;
        
        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        public Random rnd { get; private set;}

        public int NumberLivesRemaining
        {
            get { return numberLivesRemaining; }
            set
            {
                numberLivesRemaining = value;
                if (numberLivesRemaining == 0)
                {
                    currentGameState = GameState.GameOver;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //Makes the game update every 50 miliseconds (20fps) instead of default 60fps
            rnd = new Random();
           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Serenity";
            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);
            spriteManager.Enabled = false;
            spriteManager.Visible = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Initilizing audio variables
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            
            //Strt the soundtrack audio
            trackCue = soundBank.GetCue("track");
            trackCue.Play();

            //play the start sound
            soundBank.PlayCue("start");

            scoreFont = Content.Load<SpriteFont>(@"fonts\score");
            backgroundTexture = Content.Load<Texture2D>(@"Images\Background\space5");
            level2 = Content.Load<Texture2D>(@"Images\Background\space4");
            level3 = Content.Load<Texture2D>(@"Images\Background\space2");
            level4 = Content.Load<Texture2D>(@"Images\Background\space6");
            menuTexture = Content.Load<Texture2D>(@"Images\Background\Menu");
            endGame = Content.Load<Texture2D>(@"Images\Background\vMenu");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Only perform certain actions based on
            //the current gameState
            switch (currentGameState)
            {
                case GameState.Start:
                    if(Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }
                    break;
                case GameState.InGame:
                    changeLevel(currentScore);
                    
                    break;
                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        Exit();
                    break;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            audioEngine.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //only draw certain items based on
            //the current game state
            switch(currentGameState)
            {
                case GameState.Start:
                     GraphicsDevice.Clear(Color.AliceBlue);

                    spriteBatch.Begin();
                    string text = "Avoid the Reavers or Die!!";
                    spriteBatch.Draw(menuTexture,new Rectangle(0, 0, Window.ClientBounds.Width,
                            Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2((Window.ClientBounds.Width/2) - (scoreFont.MeasureString(text).X/2), 50), Color.DarkBlue);
                    spriteBatch.End();
                    break;

                case GameState.InGame:
                    GraphicsDevice.Clear(Color.Teal);
                    spriteBatch.Begin();
                    //Draw background
                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null,
                            Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);

                    //Draw fonts
                    spriteBatch.DrawString(scoreFont, "Score: "+ currentScore, new Vector2(10, 10), Color.CadetBlue, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.CadetBlue);
                     spriteBatch.Begin();

                     spriteBatch.Draw(endGame, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null,
                             Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                    string gameover = "Game Over! The Reapers boarded your ship!!";
                    spriteBatch.DrawString(scoreFont, gameover,
                        new Vector2((Window.ClientBounds.Width/2) - (scoreFont.MeasureString(gameover).X / 2), 400), Color.Moccasin);

                    gameover = "Your Score:" + currentScore;
                    spriteBatch.DrawString(scoreFont, gameover, new Vector2(10, 10), Color.Moccasin);

                    gameover = "(Press ENTER to Exit)";
                    spriteBatch.DrawString(scoreFont, gameover, 
                        new Vector2((Window.ClientBounds.Width/2) - (scoreFont.MeasureString(gameover).X / 2), 430), Color.Moccasin);
                    spriteBatch.End();
                    break;
             }
            base.Draw(gameTime);
        }

        public void PlayCue(string cueName)
        {
            soundBank.PlayCue(cueName);
        }
        /// <summary>
        /// This is where the players score is updated and kept track of
        /// </summary>
        /// <param name="score">adds to the players score</param>
        public void AddScore(int score)
        {
            currentScore += score;
        }
        public void changeLevel(int score)
        {
            //Needs more control 
            //Pause Menu press enter to continue
            //Text for 5 secs after level changes
            if (score >= 100 && score <= 200)
                backgroundTexture = level2;
            else if (score >= 200 && score <= 500)
                backgroundTexture = level3;
            else if (score >= 500)
                backgroundTexture = level4;
        }
        
    }
}
