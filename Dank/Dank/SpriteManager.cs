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
using Microsoft.Xna.Framework.Audio;


namespace Dank
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        KeyboardState oldState;

        SpriteBatch spriteBatch;
        UserControlledSprite player;
        //BGLayer scrollBGSprite;
        Texture2D battleBG;
        List<Sprite> spriteList = new List<Sprite>();
        Floor floor;
        Camera camera;
        Random rand = new Random();
        //AutomatedSprite enemy;
        bool eTurnSet = false;
        int eTurnCount = 0;
        bool enemyTurn = false;
        List<Sprite> enemies = new List<Sprite>();
        SpriteFont font;
        SpriteFont rotFont;
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue trackCue;

        Song song;

        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            camera = new Camera(Game.Window.ClientBounds);
            battleBG = Game.Content.Load<Texture2D>(@"Images/battleBG");
            font = Game.Content.Load<SpriteFont>(@"Images/font");
            rotFont = Game.Content.Load<SpriteFont>(@"Images/font");
            floor = new Floor();
            Point enemyStart;
            for (int i = 0; i <= 27; i++)
            {
                for (int j = 0; j <= 27; j++)
                {

                    floor[i, j] = new Room(Game.Content.Load<Texture2D>(@"Images/dungeonrooms"),
                        new Point(400, 400), .75f, new Vector2(i * 300, j * 300), rand, i, j);
                }
            }
            
            player = new UserControlledSprite(Game.Content.Load<Texture2D>(@"Images/herowalk"), new Point(0, 0));
            player.Animations.AddAnimation("idle", 0, 140, 23, 35, 1, 0.3f);
            player.Animations.AddAnimation("walkleft", 0, 0, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkright", 0, 35, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkdown", 0, 70, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkup", 0, 105, 23, 35, 6, 0.15f);
            player.Animations.Scale = 2f;
            floor[0, 0].addContents(player);

            for (int i = 0; i < 500; i++)
            {
                enemyStart = new Point(rand.Next(1, 27), rand.Next(1, 27));
                enemies.Add(new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/player"), enemyStart));
                enemies[i].Animations.AddAnimation("idle", 60, 0, 20, 33, 3, 0.3f);
                enemies[i].Animations.Scale = 2f;
                floor[enemyStart.X, enemyStart.Y].addContents(enemies[i]);
            }
            for (int i = 500; i < 1000; i++)
            {
                enemyStart = new Point(rand.Next(1, 27), rand.Next(1, 27));
                enemies.Add(new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/rabite"), enemyStart));
                enemies[i].Animations.AddAnimation("idle", 0, 0, 30, 41, 11, 0.1f);
                enemies[i].Animations.Scale = 2f;
                floor[enemyStart.X, enemyStart.Y].addContents(enemies[i]);
            }

            //enemyStart = new Point(3, 3);
            //enemies.Add(new WraithChaserSprite(Game.Content.Load<Texture2D>(@"Images/player"), enemyStart));
            //enemies[0].Animations.AddAnimation("idle", 60, 0, 20, 33, 3, 0.3f);
            //enemies[0].Animations.Scale = 2f;
            //floor[enemyStart.X, enemyStart.Y].addContents(enemies[0]);
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            oldState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            if (MediaPlayer.State == MediaState.Stopped)
            {
                int randSongNum = rand.Next(1, 2); //pick value 1..4
                song = Game.Content.Load<Song>(@"Audio\dungeon" + randSongNum);
                MediaPlayer.Play(song);
            }

            // TODO: Add your update code here


            if (!enemyTurn) { player.Turn = true; eTurnSet = false; }
            
            player.Update(gameTime, Game.Window.ClientBounds, floor,this);

            if (!player.Turn && !eTurnSet)
            {
                foreach (Sprite e in enemies)
                {
                    e.Turn = true;
                }
                eTurnSet = true;
            }

            eTurnCount = 0;
            foreach (Sprite e in enemies)
            {
                e.Update(gameTime, Game.Window.ClientBounds, floor, player, rand);
                if (e.Turn)
                {
                    eTurnCount += 1;
                }
            }

            //Console.WriteLine(eTurnCount);
            if (eTurnCount > 0)
            {
                enemyTurn = true;
            }

            else
            {
                enemyTurn = false;
            }

            
            int x = player.Location.X;
            int y = player.Location.Y;
            int xMax = x + 1;
            int yMax = y + 1;
            if (x - 2 <= 0) x = 0; else x = x - 1;
            if (y - 2 <= 0) y = 0; else y = y - 1;
            if (xMax > 27) xMax = 27;
            if (yMax > 27) yMax = 27;
            for (int i = x; i <= xMax; i++)
                for (int j = y; j <= yMax; j++)
                    floor[i, j].Update(gameTime);


            camera.Update(gameTime, player);

            base.Update(gameTime);
        }

        //draws stuff
        public override void Draw(GameTime gameTime)
        {
            //stuff drawn in this spritebatch doesnt move with camera
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.transform);
                //drawing the dungeon itself
                int x = player.Location.X;
                int y = player.Location.Y;
                int xMax = x + 3;
                int yMax = y + 3;
                if (x - 2 <= 0) x = 0; else x = x - 3;
                if (y - 2 <= 0) y = 0; else y = y - 3;
                if (xMax > 27) xMax = 27;
                if (yMax > 27) yMax = 27;
                for (int i = x; i <= xMax; i++)
                    for (int j = y; j <= yMax; j++)
                        floor[i,j].Draw(gameTime, spriteBatch);
            //drawing the enemies
                foreach (Sprite e in enemies)
                {
                    e.Draw(gameTime, spriteBatch);
                }
            //drawing events

            spriteBatch.End();


            
            //this stuff moves with the camera
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //draw the player
                player.Draw(gameTime, spriteBatch);
            //drawing UI stuff
                spriteBatch.DrawString(rotFont, "Rotation Energy:" + player.RotationEnergy + "/" + player.RotationEnergyMax, new Vector2(0, 0), Color.White);
                if (!player.Turn)
                    spriteBatch.DrawString(font, "Processing Enemy Turns", new Vector2(1000-300, 800-50), Color.White);
                //Battle Code to be used later
                //if (player.CurrentRoom.Battle != null)
                //    spriteBatch.Draw(battleBG, new Vector2(80,400), new Rectangle(0, 0, battleBG.Width, battleBG.Height), Color.White,0f,Vector2.Zero,2f,SpriteEffects.None,0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        public void PlayCue(String cueName) 
        {
            soundBank.PlayCue(cueName);
        }
    }
}
