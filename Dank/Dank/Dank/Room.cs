using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Dank
{
    class Room
    {
        #region FIELDS
        Texture2D textureImage;
        Point frameSize;
        Rectangle currentFrame;
        int timeSinceLast = 0; //time since last update
        int timePerChange = 10; //time to wait before updating animation frame (this along with ...
                                //rotationsPer affect the speed and smoothness of the animation)
        Point location;
        List<Sprite> enemies = new List<Sprite>();
        List<Item> items = new List<Item>();
        float scale;
        Vector2 position;
        int difficulty;
        Color floorColor;
        Color wallColor;
        float rotation; //actual degrees to rotate source image
        float rotating = 0; //keeps track of mid-animation rotations
        int rotations = 0; //rotation counter for animation
        int rotationsPer = 20; //number of frames per animation
        int enemyCount = 0;
        Random rand;
        Vector4 exits;
        
        #endregion
        #region PROPERTIES
        public int EnemyCount { get { return enemyCount; } }
        public Vector4 Exits { get { return exits; } set { exits = value; } }
        public Point Location { get { return location; } set { location = value; } }
        public List<Sprite> Enemies { get { return enemies; } set { enemies = value; } }
        public List<Item> Items { get { return items; } }
        #endregion
        #region METHODS
        //constructor
        public Room(Texture2D textureImage, Point frameSize, float scale, Vector2 position, Random rand, int i, int j)
        {
            this.textureImage = textureImage;
            this.frameSize = frameSize;
            this.location = new Point(i, j);
            this.scale = scale;
            this.position = position;
            this.rand = rand;
            if (location.X >= 0 && location.X <= 8 && location.Y >= 0 && location.Y <= 8)//First grid (0,0)
            {
                difficulty = 0;

                floorColor = Color.LightBlue;
                wallColor = Color.LightGreen;
            }
            if (location.X > 8 && location.X <= 17 && location.Y >= 0 && location.Y <= 8)//Second grid (1,0)
            {
                difficulty = 1;
                floorColor = new Color(250, 200, 250);
                wallColor = new Color(255, 255, 100);
            }
            if (location.X > 17 && location.X <= 26 && location.Y >= 0 && location.Y <= 8) //Third grid (2,0)
            {
                difficulty = 2;
                floorColor = new Color(150, 150, 150);
                wallColor = new Color(250, 150, 50);
            }
            if (location.Y > 8 && location.Y <= 17 && location.X >= 0 && location.X <= 8) //Fourth grid (0,1)
            {
                difficulty = 1;
                floorColor = new Color(250, 200, 250);
                wallColor = new Color(255, 255, 100);
            }
            if (location.Y > 8 && location.Y <= 17 && location.X > 8 && location.X <= 17) //Fifth grid (1,1)
            {
                difficulty = 2;
                floorColor = new Color(150, 150, 150);
                wallColor = new Color(250, 150, 50);
            }
            if (location.Y > 8 && location.Y <= 17 && location.X > 17 && location.X <= 26) //Sixth grid (1,2)
            {
                difficulty = 3;
                floorColor = new Color(100, 130, 100);
                wallColor = new Color(180, 0, 30);
            }
            if (location.X >= 0 && location.X <= 8 && location.Y > 17 && location.Y <= 26) //Seventh grid (2,0)
            {
                difficulty = 2;
                floorColor = new Color(150, 150, 150);
                wallColor = new Color(250, 150, 50);
            }
            if (location.X > 8 && location.X <= 17 && location.Y > 17 && location.Y <= 26) // Eighth grid (2,1)
            {
                difficulty = 3;
                floorColor = new Color(100, 130, 100);
                wallColor = new Color(180, 0, 30);
            }
            if (location.X > 17 && location.X <= 26 && location.Y > 17 && location.Y <= 26) //Ninth grid (2,2)
            {
                difficulty = 4;
                floorColor = new Color(170, 60, 60);
                wallColor = new Color(200, 200, 200);
            }

            this.exits = randomDoors();
            currentFrame = roomSelector(1);

            // These two lines keep all the rooms from spinning on creation
            rotations = rotationsPer;
            rotating = rotation;

            
        }

        public virtual void Update(GameTime gameTime)
        {
            if (rotations == rotationsPer)
                return;
            timeSinceLast += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLast >= timePerChange)
            {
                if (rotations < rotationsPer)
                {
                    //Increment the mid-animation variable by a fraction of the amount originally
                    //decremented from the total rotation until it equals the total rotation
                    rotating += MathHelper.PiOver2 / rotationsPer;
                    rotations += 1;
                }

                timeSinceLast = 0;
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the floor using the mid-animation rotation: rotating
            spriteBatch.Draw(textureImage, position,
                new Rectangle(1200,400,400,400),
                    floorColor, rotating, new Vector2(frameSize.X / 2, frameSize.Y / 2),
                    scale, SpriteEffects.None, 0);

            //Draw the walls using the mid-animation rotation: rotating
            spriteBatch.Draw(textureImage, position,
                currentFrame,
                    wallColor, rotating, new Vector2(frameSize.X/2, frameSize.Y/2),
                    scale, SpriteEffects.None, 0);
        }
        public void DrawContents(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Sprite s in enemies)
            {
                s.Draw(gameTime, spriteBatch);
            }
            if (items.Count > 0)
            {
                spriteBatch.Draw(items[0].Image, new Rectangle(location.X * 300, location.Y * 300+25, items[0].Image.Width, items[0].Image.Height), Color.White);
            }
        }

        private Vector4 randomDoors()
        {
            int exitWeight = rand.Next(100);
            if (difficulty == 0 && exitWeight <= 4)
            {
                do
                {
                    exitWeight = rand.Next(100);
                } while (exitWeight <= 4);
            }
                
            int amount = 0;
            if(exitWeight <= 4)
                //Zero Exits
                amount = 0;
            if (exitWeight > 4 && exitWeight <= 9)
                //One Exits
                amount = 1;
            if (exitWeight > 9 && exitWeight <= 14)
                //Four Exits
                amount = 4;
            if(exitWeight > 14 && exitWeight <= 44)
                //Three Exits
                amount = 3;
            if (exitWeight > 44 && exitWeight <= 99)
                //Two Exits
                amount = 2;
            if (location.X == 0 && location.Y == 0)
                amount = 3;
            Vector4 newExits = Vector4.Zero;
            do
            {
                newExits = new Vector4(rand.Next(2), rand.Next(2), rand.Next(2), rand.Next(2));
            } while (newExits.X + newExits.Y + newExits.Z + newExits.W != amount);

            return newExits;
        }

        private Rectangle roomSelector(int caller)
        {
            Vector4 newVect = Vector4.Zero;
            Rectangle room = new Rectangle();
            int X;
            int Y;
            if (exits == newVect)
            {

                X = 1;
                Y = 1;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }
            newVect = new Vector4(0, 0, 1, 0);
            if (exits == newVect)
            {
                X = 0;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2 * 3f;
            }
            newVect = new Vector4(0, 1, 0, 0);
            if (exits == newVect)
            {
                X = 0;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.Pi;
            }
            newVect = new Vector4(0, 1, 1, 0);
            if (exits == newVect)
            {
                X = 1;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.Pi;
            }
            newVect = new Vector4(1, 0, 0, 0);
            if (exits == newVect)
            {
                X = 0;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2;
            }
            newVect = new Vector4(1, 0, 1, 0);
            if (exits == newVect)
            {
                X = 2;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2;
            }
            newVect = new Vector4(1, 1, 0, 0);
            if (exits == newVect)
            {
                X = 1;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2;
            }
            newVect = new Vector4(1, 1, 1, 0);
            if (exits == newVect)
            {
                X = 3;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2;
            }
            newVect = new Vector4(0, 0, 0, 1);
            if (exits == newVect)
            {
                X = 0;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }
            newVect = new Vector4(0, 0, 1, 1);
            if (exits == newVect)
            {
                X = 1;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2 * 3f;
            }
            newVect = new Vector4(0, 1, 0, 1);
            if (exits == newVect)
            {
                X = 2;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }
            newVect = new Vector4(0, 1, 1, 1);
            if (exits == newVect)
            {
                X = 3;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.Pi;
            }
            newVect = new Vector4(1, 0, 0, 1);
            if (exits == newVect)
            {
                X = 1;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }
            newVect = new Vector4(1, 0, 1, 1);
            if (exits == newVect)
            {
                X = 3;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = MathHelper.PiOver2 * 3f;
            }
            newVect = new Vector4(1, 1, 0, 1);
            if (exits == newVect)
            {
                X = 3;
                Y = 0;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }
            newVect = new Vector4(1, 1, 1, 1);
            if (exits == newVect)
            {
                X = 0;
                Y = 1;
                room = new Rectangle(X * frameSize.X,
                    Y * frameSize.Y,
                    frameSize.X, frameSize.Y);
                rotation = 0f;
            }

            //Set the mid-animation variable to 90 degrees less than the total rotation
            if (caller == 0)
                rotating = rotation - MathHelper.PiOver2;
            else
                rotating = rotation;
            return room;
        }

        //Rotate the exits in the room
        public void rotate()
        {
            Vector4 newexit = new Vector4(0, 0, 0, 0);
            
            if (exits.W == 1)
                newexit.X = 1;
            else
                newexit.X = 0;
            if (exits.X == 1)
                newexit.Y = 1;
            else
                newexit.Y = 0;
            if (exits.Y == 1)
                newexit.Z = 1;
            else
                newexit.Z = 0;
            if (exits.Z == 1)
                newexit.W = 1;
            else
                newexit.W = 0;
            exits = newexit;
            rotations = 0; //allow animation to run by setting rotations to 0
            currentFrame = roomSelector(0);
        }

        public void AddExit(int dir)
        {
            Vector4 newExit = exits;

            if (dir == 1) //Up
            {
                newExit.W = 1;
            }
            if (dir == 2) //Right
            {
                newExit.X = 1;
            }
            if (dir == 3) //Down
            {
                newExit.Y = 1;
            }
            if (dir == 4) //Left
            {
                newExit.Z = 1;
            }
            exits = newExit;
            currentFrame = roomSelector(1);
        }

        public void RemoveExit(int dir)
        {
            Vector4 newExit = exits;

            if (dir == 1) //Up
            {
                newExit.W = 0;
            }
            if (dir == 2) //Right
            {
                newExit.X = 0;
            }
            if (dir == 3) //Down
            {
                newExit.Y = 0;
            }
            if (dir == 4) //Left
            {
                newExit.Z = 0;
            }
            exits = newExit;
            currentFrame = roomSelector(1);
        }

        public bool AddContent(Sprite something)
        {
            
            //Console.WriteLine(something.GetType().ToString() + " entered room " + location.X + ", " + location.Y);
            //Console.WriteLine("Room Enemies " + location.X + ", " + location.Y);
            if (enemies.Count < 5)
            {
                enemies.Add(something);
                enemyCount++;
                return true;
            }
            return false;
        }
        public void AddContent(UserControlledSprite player)
        {
            if (location == new Point(26, 26))
            {
                //Victory
                String content = "    Contratiations, brav warriar.  You have proofed yourself a true" + Environment.NewLine +
                                 "     hero.  Not manny could have fought there way thrugh this dank," + Environment.NewLine +
                                 "   dangerous dungeon and livved too tell the tail.  You're exploits" + Environment.NewLine +
                                 "    will be told thrugh the anals of history.  Bards will sing of" + Environment.NewLine +
                                 "     your glourius deeds.  Thou you still have not found an exit," + Environment.NewLine +
                                 "      no that you're efforts were not in vane.  There is a piece" + Environment.NewLine +
                                 "     of chalk on the ground where you may right your name. If you" + Environment.NewLine +
                                 "    are lucky, someone will see it someday and be like \"whoa, this" + Environment.NewLine +
                                 "   dude made it all the way through.\" and then all that cool stuff" + Environment.NewLine +
                                 "    I said earlier about the bards and your exploits will probably" + Environment.NewLine +
                                 "                                  happen." + Environment.NewLine +
                                 "                         Thank you for playing Dank!" + Environment.NewLine +
                                 "                           Press Enter to Exit Game";
                Game1.WON = true;
                UI.ShowDialogue(content, 13, 70);
            }
            else
            {
                player.Difficulty = difficulty;
                
                if (items.Count > 0)//Items in room
                {
                    List<Item> tItems = new List<Item>(items);
                    foreach (Item i in tItems)
                    {
                        if (player.AddItem(i))
                        {
                            items.Remove(i);
                        }
                        else
                        {
                            UI.ShowMessage("Inventory Full");
                            return;
                        }
                    }
                }
            }
        }
        public void AddContent(Item item)
        {
            items.Add(item);
        }

        public void RemoveContent(Sprite something)
        {
            if (something is UserControlledSprite)
            {
            }
            else
            {
                enemyCount--;
                enemies.Remove(something);
            }
        }

        public void RemoveContent(Item item)
        {
            if(items.Contains(item))
                items.Remove(item);
        }

        public Boolean moveRequest(Sprite requester)
        {
            

            if (requester.Difficulty > difficulty && !(requester is UserControlledSprite))
                return false;

            //limits enemies per room
            if (enemies.Count() >= 5 && !(requester is UserControlledSprite))
                return false;

            if (requester is WraithChaserSprite) //Wraith
                return true;

            float difX = location.X - requester.Location.X;
            float difY = location.Y - requester.Location.Y;
            //Console.WriteLine("move request: " + requester.GetType());
            //Console.WriteLine("difX: " + difX + " difY: " + difY);
            //Console.WriteLine("Current Room Exits: " + requester.CurrentRoom.exits.X + " " + requester.CurrentRoom.exits.Y + " " + requester.CurrentRoom.exits.Z + " " + requester.CurrentRoom.exits.W);
            //Console.WriteLine("Destination Room Exits: " + exits.X + " " + exits.Y + " " + exits.Z + " " + exits.W);

            if (difX > 0) //moving Right
            {
                if (requester.CurrentRoom.exits.X != 0 && exits.Z != 0)
                    return true;
            }
            else if (difX < 0) //moving Left
            {
                if (requester.CurrentRoom.exits.Z != 0 && exits.X != 0)
                    return true;
            }
            if (difY > 0) //moving Down
            {
                if (requester.CurrentRoom.exits.Y != 0 && exits.W != 0)
                    return true;
            }
            else if (difY < 0) //moving on Up
            {
                if (requester.CurrentRoom.exits.W != 0 && exits.Y != 0)
                    return true;
            }
            
            return false;
        }
        #endregion
    }
}
