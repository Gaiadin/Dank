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



namespace Dank
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region FIELDS
        public static bool SkipTurn = false; // debug tool for skipping enemy turn
        KeyboardState oldState;
        SpriteBatch spriteBatch;
        UserControlledSprite player;
        Texture2D battleBG;
        List<Sprite> spriteList = new List<Sprite>();
        Floor floor;
        Camera camera;
        Random rand = new Random();
        bool eTurnSet = false;
        int eTurnCount = 0;
        bool enemyTurn = false;
        List<Sprite> enemies = new List<Sprite>();
        SpriteFont font;
        SpriteFont rotFont;
        SpriteFont wFont;
        SpriteFont gameOver;
        static AudioEngine audioEngine;
        static SoundBank soundBank;
        WaveBank waveBank;
        //Cue trackCue;
        Song song;
        #endregion
        #region METHODS
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
            wFont = Game.Content.Load<SpriteFont>(@"Images/font");
            UI.SetupUI(Game.Content.Load<Texture2D>(@"Images/windowbg"), Game.Content.Load<Texture2D>(@"Images/windowfg"), Game.Content.Load<Texture2D>(@"Images/selection"), wFont);
            #region ITEMS
            //**ITEM CREATION** **ITEM CREATION** **ITEM CREATION**
            Item item;
            item = new Item(Game.Content.Load<Texture2D>(@"Images/chest"));
            item.Name = "Potion";
            item.Value = 10;
            item.Effect = 5;
            item.Uses = 3;
            item.Description = "Restores 5 health";
            ItemList.Add(item.Name, item);
            item = new Item(Game.Content.Load<Texture2D>(@"Images/chest"));
            item.Name = "Energy Potion";
            item.Description = "Restores 5 Rotation Energy";
            item.Value = 20;
            item.Effect = 5;
            item.Uses = 2;
            ItemList.Add(item.Name, item);
            item = new Item(Game.Content.Load<Texture2D>(@"Images/chest"));
            item.Name = "Elixir of Strength";
            item.Description = "Increase Strength for 5 turns";
            item.Value = 50;
            item.Effect = 5;
            item.Buff = new Buff("strength", 5, 5);
            item.Uses = 1;
            ItemList.Add(item.Name, item);
            item = new Item(Game.Content.Load<Texture2D>(@"Images/chest"));
            item.Name = "Stoneskin Potion";
            item.Description = "Increase Defense for 5 turns";
            item.Value = 50;
            item.Effect = 5;
            item.Buff = new Buff("defense", 5, 5);
            item.Uses = 1;
            ItemList.Add(item.Name, item);
            //**END ITEM CREATION** **END ITEM CREATION** **END ITEM CREATION**
#endregion
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            camera = new Camera(Game.Window.ClientBounds);
            battleBG = Game.Content.Load<Texture2D>(@"Images/battleBG");
            
            font = Game.Content.Load<SpriteFont>(@"Images/font");
            rotFont = Game.Content.Load<SpriteFont>(@"Images/font");
            gameOver = Game.Content.Load<SpriteFont>(@"Images/gameover");
            floor = new Floor();
            int itemCheck;
            Vector2 enemyStart;
            for (int i = 0; i <= 26; i++)
            {
                for (int j = 0; j <= 26; j++)
                {

                    floor[i, j] = new Room(Game.Content.Load<Texture2D>(@"Images/dungeonrooms"),
                        new Point(400, 400), .75f, new Vector2(i * 300, j * 300), rand, i, j);
                    if ((i != 0 && j != 0) || (i != 26 && j != 26))
                    {
                        itemCheck = rand.Next(15);
                        if (itemCheck == 1)
                            floor[i, j].AddContent(ItemList.Get("Energy Potion"));
                        if (itemCheck == 2)
                            floor[i, j].AddContent(ItemList.Get("Potion"));
                        itemCheck = rand.Next(100);
                        if (i > 8 || j > 8)
                        {
                            if (itemCheck == 1)
                                floor[i, j].AddContent(ItemList.Get("Stoneskin Potion"));
                            if (itemCheck == 2)
                                floor[i, j].AddContent(ItemList.Get("Elixir of Strength"));
                        }
                    }
                }
            }
            
            
            player = new UserControlledSprite(Game.Content.Load<Texture2D>(@"Images/herowalk"), Vector2.Zero);
            player.Animations.AddAnimation("idle", 0, 140, 23, 35, 1, 0.3f);
            player.Animations.AddAnimation("walkleft", 0, 0, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkright", 0, 35, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkdown", 0, 70, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("walkup", 0, 105, 23, 35, 6, 0.15f);
            player.Animations.AddAnimation("fight", 0, 180, 68, 36, 3, 0.15f);
            player.Animations.AddAnimation("victory", 0, 217, 26, 35, 7, 0.15f);
            player.Animations.Scale = 2f;
            floor[0, 0].AddContent(player);
            player.CurrentRoom = floor[0, 0];

            for (int i = 0; i < 100; i++)
            {
                enemyStart = new Vector2(rand.Next(1, 27), rand.Next(1, 27));
                enemies.Add(new ChaserSprite(Game.Content.Load<Texture2D>(@"Images/player"), enemyStart));
                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[i]))
                {
                    enemies[i].Animations.AddAnimation("idle", 60, 0, 20, 33, 3, 0.3f);
                    enemies[i].Animations.Scale = 2f;
                    enemies[i].index = i;
                    setDifficulty(enemies[i]);
                }
                else
                {
                    enemies.Remove(enemies[i]);
                    i--;
                }
            }
            for (int i = 100; i < 200; i++)
            {
                enemyStart = new Vector2(rand.Next(1, 27), rand.Next(1, 27));
                enemies.Add(new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/rabite"), enemyStart));
                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[i]))
                {
                    enemies[i].Animations.AddAnimation("idle", 0, 0, 30, 41, 11, 0.1f);
                    enemies[i].Animations.Scale = 2f;
                    setDifficulty(enemies[i]);
                }
                else
                {
                    enemies.Remove(enemies[i]);
                    i--;
                }
            }
            for (int i = 200; i < 300; i++)
            {
                enemyStart = new Vector2(rand.Next(1, 27), rand.Next(1, 27));
                enemies.Add(new WraithChaserSprite(Game.Content.Load<Texture2D>(@"Images/ghost"), enemyStart));
                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[i]))
                {
                    enemies[i].Animations.AddAnimation("idle", 0, 0, 16, 30, 7, 0.1f);
                    enemies[i].Animations.AddAnimation("walkup", 0, 120, 16, 30, 5, 0.1f);
                    enemies[i].Animations.AddAnimation("walkright", 0, 60, 16, 30, 5, 0.1f);
                    enemies[i].Animations.AddAnimation("walkdown", 0, 30, 16, 30, 5, 0.1f);
                    enemies[i].Animations.AddAnimation("walkleft", 0, 90, 16, 30, 5, 0.1f);
                    enemies[i].Animations.Scale = 2f;
                    setDifficulty(enemies[i]);
                }
                else
                {
                    enemies.Remove(enemies[i]);
                    i--;
                }
            }
            

            floor[1, 0].AddContent(ItemList.Get("Energy Potion"));
            floor[1, 0].AddContent(ItemList.Get("Potion"));

            //enemyStart = new Point(3, 3);
            //enemies.Add(new WraithChaserSprite(Game.Content.Load<Texture2D>(@"Images/player"), enemyStart));
            //enemies[0].Animations.AddAnimation("idle", 60, 0, 20, 33, 3, 0.3f);
            //enemies[0].Animations.Scale = 2f;
            //floor[enemyStart.X, enemyStart.Y].AddContent(enemies[0]);
            Game1.Volume = .3f;
            Game1.SEVolume = 1f;
            String content = "Welcome to Dank.  Actually, it's not really a very welcoming place." + Environment.NewLine +
                             "It's full of monsters that get more difficult the deeper you delve," + Environment.NewLine +
                             "but delve deep you must!  For your salvation lies in the bottom-" + Environment.NewLine +
                             "right room.  You may rotate your own room or those adjacent to it." + Environment.NewLine +
                             "Press H for a list of the game's controls.";
            UI.ShowDialogue(content, 5, 70);
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
            if (Game1.GAMEOVER)
            {
                base.Update(gameTime);
                return;
            }
            #region bgm
            //control the bgm
            MediaPlayer.Volume = Game1.Volume;
            audioEngine.Update();
            if (MediaPlayer.State == MediaState.Stopped)
            {
                int randSongNum = rand.Next(1, 2); //pick value 1..6
                song = Game.Content.Load<Song>(@"Audio\dungeon" + randSongNum);
                MediaPlayer.Play(song);

            }
            #endregion

            UI.Update(gameTime, player);
            // TODO: Add your update code here

            if (Game1.BATTLE)
            {
                
                List<Sprite> enemies = new List<Sprite>(player.CurrentRoom.Enemies);
                /**DEBUG - No Battles**/
                //foreach (Sprite e in enemies)
                //{
                //    player.CurrentRoom.Enemies.Remove(e);
                //    this.enemies.Remove(e);
                //}
                //Game1.BATTLE = false;
                //return;
                player.Animations.CurrentAnimation = "fight";
                int exp = 0;
                int health = 0;
                int eHealth = 0;
                int eMaxHealth = 0;
                int eStrength = 0;
                int eDefense = 0;
                int eSpeed = 0;
                int eCount = player.CurrentRoom.Enemies.Count;
                int eDifficulty = 0;
                foreach (Sprite e in enemies)
                {
                    eHealth += e.Health;
                    eSpeed += e.Speed;
                    eStrength += e.Strength;
                    eDefense += e.Defense;
                    eDifficulty += e.Difficulty;
                }
                eDifficulty += eCount;
                //bonus exp for fighting groups
                if (eCount > 1) { eDifficulty += eCount; }
                eSpeed = eSpeed - eCount + 1;
                eDefense = eDefense - eCount + 1;
                eStrength = eStrength - eCount + 1;
                eMaxHealth = eHealth;
                do
                {
                        //Player deals damage first
                        if (player.Speed > eSpeed)
                        {
                            if (player.Strength >= eDefense)
                            {
                                eHealth -= player.Strength - eDefense ;
                                if (eHealth <= 0)
                                {
                                    exp += eDifficulty;
                                    player.GainExp(eDifficulty, rand);
                                    foreach (Sprite e in enemies)
                                    {
                                        player.CurrentRoom.Enemies.Remove(e);
                                        this.enemies.Remove(e);
                                    }
                                }
                                else if (eHealth <= eMaxHealth / eCount)
                                {
                                    eStrength /= eCount;
                                    eDefense /= eCount;
                                    eSpeed /= eCount;
                                    eMaxHealth /= eCount;
                                    eCount--;
                                    
                                }
                            }
                            if (eStrength >= player.Defense)
                            {
                                
                                player.SubtractHealth(eStrength - player.Defense);
                                health += eStrength - player.Defense;
                            }
                        }
                        else
                        {
                            if (eStrength >= player.Defense)
                            {
                                player.SubtractHealth(eStrength - player.Defense);
                                health += eStrength - player.Defense;
                            }
                            if (player.Strength >= eDefense)
                            {
                                eHealth -= player.Strength - eDefense;
                                if (eHealth <= 0)
                                {
                                    exp += eDifficulty ;
                                    player.GainExp(eDifficulty, rand);
                                    foreach (Sprite e in enemies)
                                    {
                                        player.CurrentRoom.Enemies.Remove(e);
                                        this.enemies.Remove(e);
                                    }
                                }
                                else if (eHealth <= eMaxHealth / eCount)
                                {
                                    eStrength /= eCount;
                                    eDefense /= eCount;
                                    eSpeed /= eCount;
                                    eMaxHealth /= eCount;
                                    eCount--;
                                }
                            }
                        }
                    enemies = new List<Sprite>(player.CurrentRoom.Enemies);
                } while (player.CurrentRoom.Enemies.Count > 0 && !Game1.GAMEOVER);
                UI.ShowMessage("You cleared the room!");
                UI.ShowMessage("Lost " + health.ToString() + " health!");
                UI.ShowMessage("Gained " + exp.ToString() + " experience!");
                

                Game1.BATTLE = BattleChecker();
            }
            else
            {

                
                if (!enemyTurn)
                {
                    player.Turn = true;
                    eTurnSet = false;
                    //Game1.BATTLE = BattleChecker(player);
                }

                //player turn


                player.Update(gameTime, Game.Window.ClientBounds, floor, this);

                if (!player.Turn && !eTurnSet)
                {
                    //setting the enemy turns

                    if (!SpriteManager.SkipTurn)
                    {
                        foreach (Sprite e in enemies)
                        {
                            e.Turn = true;
                        }
                        eTurnSet = true;
                    }
                    //before players turn taken for battle
                    //Battle(player);
                }

                eTurnCount = 0;
                foreach (Sprite e in enemies)
                {
                    //enemies take their turns
                    e.Update(gameTime, Game.Window.ClientBounds, floor, player, rand);
                    if (e.Turn)
                    {
                        eTurnCount += 1;
                    }
                }


                if (eTurnCount > 0) { enemyTurn = true; }
                else { enemyTurn = false; }


                float x = player.Location.X;
                float y = player.Location.Y;
                float xMax = x + 1;
                float yMax = y + 1;
                if (x - 1 <= 0) x = 0; else x = x - 1;
                if (y - 1 <= 0) y = 0; else y = y - 1;
                if (xMax > 26) xMax = 26;
                if (yMax > 26) yMax = 26;
                for (int i = (int)x; i <= xMax; i++)
                    for (int j = (int)y; j <= yMax; j++)
                        floor[i, j].Update(gameTime);


                camera.Update(gameTime, player);
            }

            base.Update(gameTime);
        }

        //draws stuff
        public override void Draw(GameTime gameTime)
        {
            if (!Game1.GAMEOVER)
            {
                //stuff drawn in this spritebatch doesnt move with camera
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.transform);
                //drawing the dungeon itself
                float x = player.Location.X;
                float y = player.Location.Y;
                float xMax = x + 3;
                float yMax = y + 3;
                if (x - 3 <= 0) x = 0; else x = x - 3;
                if (y - 3 <= 0) y = 0; else y = y - 3;
                if (xMax > 26) xMax = 26;
                if (yMax > 26) yMax = 26;
                for (int i = (int)x; i <= (int)xMax; i++)
                    for (int j = (int)y; j <= (int)yMax; j++)
                        floor[i, j].Draw(gameTime, spriteBatch);

                for (int i = (int)x; i <= (int)xMax; i++)
                    for (int j = (int)y; j <= (int)yMax; j++)
                        floor[i, j].DrawContents(gameTime, spriteBatch);
                //drawing the enemies
                //foreach (Sprite e in enemies)
                //{
                //    e.Draw(gameTime, spriteBatch);
                //}
                //drawing events

                spriteBatch.End();



                //this stuff moves with the camera
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                //draw the player

                player.Draw(gameTime, spriteBatch);
                //drawing UI stuff
                spriteBatch.DrawString(rotFont, "HP:    " + player.Health + "/" + player.HealthMax, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(rotFont, "Energy:" + player.RotationEnergy + "/" + player.RotationEnergyMax, new Vector2(0, 15), Color.White);
                if (!player.Turn)
                    spriteBatch.DrawString(font, "Processing Enemy Turns", new Vector2(1000 - 300, 800 - 50), Color.White);

                //Battle Code to be used later
                //if (player.CurrentRoom.Battle != null)
                //    spriteBatch.Draw(battleBG, new Vector2(80,400), new Rectangle(0, 0, battleBG.Width, battleBG.Height), Color.White,0f,Vector2.Zero,2f,SpriteEffects.None,0);
                UI.Draw(gameTime, spriteBatch);
                //Window.DrawContent(gameTime, spriteBatch);
                spriteBatch.End();

                base.Draw(gameTime);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                spriteBatch.DrawString(gameOver, "GAME OVER", new Vector2(300, 350), Color.White);
                spriteBatch.End();
                
            }

        }

        public static void ShowOptions()
        {
            String se;
            String music;
            if (!Game1.SE)
                se = "Mute";
            else
                se = ((int)(Game1.SEVolume * 100)).ToString();
            if (Game1.Volume < .01f)
                music = "Mute";
            else
                music = ((int)(Game1.Volume * 200)).ToString();
            UI.ShowOptions(music, se, 2, 18);
        }
        public static void PlayCue(String cueName) 
        {
            if (Game1.SE)
                soundBank.PlayCue(cueName);
        }
        public static void SetVolumeSE()
        {
            if (Game1.SEVolume < 0)
                Game1.SEVolume = 0;
            audioEngine.GetCategory("Default").SetVolume(Game1.SEVolume);
        }

        public bool BattleChecker()
        {
            
            if (player.CurrentRoom.Enemies.Count > 0)
            {
                
                return true;
            }
            return false;
        }
        public void NewSpawn()
        {
            int monCheck = 0;
            int itemCheck = 0;
            Vector2 enemyStart;
            for (int i = 0; i <= 26; i++)
            {
                for (int j = 0; j <= 26; j++)
                {
                    if ((i != 0 && j != 0) || (i != 26 && j != 26))
                    {
                        if (i < player.CurrentRoom.Location.X - 2 || i > player.CurrentRoom.Location.X + 2 || j < player.CurrentRoom.Location.Y - 2 || j > player.CurrentRoom.Location.Y + 2)
                        {
                            monCheck = rand.Next(1500);
                            if (monCheck == 1)
                            {
                                enemyStart = new Vector2(i,j);
                                enemies.Add(new AutomatedSprite(Game.Content.Load<Texture2D>(@"Images/rabite"), enemyStart));
                                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[enemies.Count - 1]))
                                {
                                    enemies[enemies.Count - 1].Animations.AddAnimation("idle", 0, 0, 30, 41, 11, 0.1f);
                                    enemies[enemies.Count - 1].Animations.Scale = 2f;
                                    setDifficulty(enemies[enemies.Count - 1]);
                                }
                                else
                                {
                                    enemies.Remove(enemies[enemies.Count - 1]);
                                }
                            }
                            if (monCheck == 2)
                            {
                                enemyStart = new Vector2(i, j);
                                enemies.Add(new WraithChaserSprite(Game.Content.Load<Texture2D>(@"Images/ghost"), enemyStart));
                                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[enemies.Count - 1]))
                                {
                                    enemies[enemies.Count - 1].Animations.AddAnimation("idle", 0, 0, 16, 30, 7, 0.1f);
                                    enemies[enemies.Count - 1].Animations.AddAnimation("walkup", 0, 120, 16, 30, 5, 0.1f);
                                    enemies[enemies.Count - 1].Animations.AddAnimation("walkright", 0, 60, 16, 30, 5, 0.1f);
                                    enemies[enemies.Count - 1].Animations.AddAnimation("walkdown", 0, 30, 16, 30, 5, 0.1f);
                                    enemies[enemies.Count - 1].Animations.AddAnimation("walkleft", 0, 90, 16, 30, 5, 0.1f);
                                    enemies[enemies.Count - 1].Animations.Scale = 2f;
                                    setDifficulty(enemies[enemies.Count - 1]);
                                }
                                else
                                {
                                    enemies.Remove(enemies[enemies.Count - 1]);
                                }
                            }
                            if (monCheck == 3)
                            {
                                enemyStart = new Vector2(i, j);
                                enemies.Add(new ChaserSprite(Game.Content.Load<Texture2D>(@"Images/player"), enemyStart));
                                if (floor[enemyStart.X, enemyStart.Y].AddContent(enemies[enemies.Count - 1]))
                                {
                                    enemies[enemies.Count - 1].Animations.AddAnimation("idle", 60, 0, 20, 33, 3, 0.3f);
                                    enemies[enemies.Count - 1].Animations.Scale = 2f;
                                    setDifficulty(enemies[enemies.Count - 1]);
                                }
                                else
                                {
                                    enemies.Remove(enemies[enemies.Count - 1]);
                                }
                            }
                            itemCheck = rand.Next(2000);
                            if (itemCheck == 1)
                                floor[i, j].AddContent(ItemList.Get("Energy Potion"));
                            if (itemCheck == 2)
                                floor[i, j].AddContent(ItemList.Get("Potion"));
                            itemCheck = rand.Next(3000);
                            if (i > 8 || j > 8)
                            {
                                if (itemCheck == 1)
                                {
                                    floor[i, j].AddContent(ItemList.Get("Stoneskin Potion"));
                                    Console.WriteLine("def" + i + " " + j);
                                }
                                if (itemCheck == 2)
                                {
                                    floor[i, j].AddContent(ItemList.Get("Elixir of Strength"));
                                    Console.WriteLine("str" + i + " " + j);
                                }
                            }
                        }
                    }
                }
            }
        }
        void setDifficulty(Sprite enemy)
        {
            if (enemy.Location.X >= 0 && enemy.Location.X <= 8 && enemy.Location.Y >= 0 && enemy.Location.Y <= 8)//First grid (0,0)
            {
                enemy.SetDifficulty(0);

            }
            if (enemy.Location.X > 8 && enemy.Location.X <= 17 && enemy.Location.Y >= 0 && enemy.Location.Y <= 8)//Second grid (1,0)
            {
                enemy.SetDifficulty(1);
                
            }
            if (enemy.Location.X > 17 && enemy.Location.X <= 26 && enemy.Location.Y >= 0 && enemy.Location.Y <= 8) //Third grid (2,0)
            {
                enemy.SetDifficulty(2);
                
            }
            if (enemy.Location.Y > 8 && enemy.Location.Y <= 17 && enemy.Location.X >= 0 && enemy.Location.X <= 8) //Fourth grid (0,1)
            {
                enemy.SetDifficulty(1);
                
            }
            if (enemy.Location.Y > 8 && enemy.Location.Y <= 17 && enemy.Location.X > 8 && enemy.Location.X <= 17) //Fifth grid (1,1)
            {
                enemy.SetDifficulty(2);
                
            }
            if (enemy.Location.Y > 8 && enemy.Location.Y <= 17 && enemy.Location.X > 17 && enemy.Location.X <= 26) //Sixth grid (1,2)
            {
                enemy.SetDifficulty(3);
                
            }
            if (enemy.Location.X >= 0 && enemy.Location.X <= 8 && enemy.Location.Y > 17 && enemy.Location.Y <= 26) //Seventh grid (2,0)
            {
                enemy.SetDifficulty(2);
                
            }
            if (enemy.Location.X > 8 && enemy.Location.X <= 17 && enemy.Location.Y > 17 && enemy.Location.Y <= 26) // Eighth grid (2,1)
            {
                enemy.SetDifficulty(3);
                
            }
            if (enemy.Location.X > 17 && enemy.Location.X <= 26 && enemy.Location.Y > 17 && enemy.Location.Y <= 26) //Ninth grid (2,2)
            {
                enemy.SetDifficulty(4);
                
            }
        }
        #endregion
    }
}
