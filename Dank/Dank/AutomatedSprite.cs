using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class AutomatedSprite : Sprite
    {
        int distance = 0;
        int dir;
        bool turnTaken = false;
        //bool turn = false;
        //public bool Turn { get { return turn; } set { turn = value; } }


        public AutomatedSprite(Texture2D textureImage, Point location)
            : base(textureImage, location)
        {
            this.Offset = new Point(50, 50);
            Difficulty = 0;
            Turn = false;
        }

        public  override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, UserControlledSprite player, Random rand)
        {
            //turnTaken = true;
            if (Turn && !turnTaken)
            {
                
                dir = rand.Next(1, 5);
                //Console.WriteLine("dir " + dir);

                //checking up
                if (Location.Y != 0)
                {
                    if (dir == 1)
                    {
                        if (floor[Location.X, Location.Y - 1].moveRequest(this))
                        {
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X, Location.Y - 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                }
                //checking right
                if (Location.X != 27)
                {
                    if (dir == 2)
                    {
                        if (floor[Location.X + 1, Location.Y].moveRequest(this))
                        {
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X + 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                }
                //checking down
                if (Location.Y != 27)
                {
                    if (dir == 3)
                    {
                        if (floor[Location.X, Location.Y + 1].moveRequest(this))
                        {
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X, Location.Y + 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                }
                //checking left
                if (Location.X != 0)
                {
                    if (dir == 4)
                    {
                        if (floor[Location.X - 1, Location.Y].moveRequest(this))
                        {
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X - 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                }
                turnTaken = true;
            }

            
            if (distance == 0)
            {
                
                if (turnTaken)
                {
                    changeTurn(false);
                }
                if (Animations.CurrentAnimation != "idle")
                {
                    Animations.CurrentAnimation = "idle";
                }

            }
            

            if (distance > 0)
            {
                if (dir == 1)
                    Position = new Point(Position.X, Position.Y - 5);
                if (dir == 2)
                    Position = new Point(Position.X + 5, Position.Y);
                if (dir == 3)
                    Position = new Point(Position.X, Position.Y + 5);
                if (dir == 4)
                    Position = new Point(Position.X - 5, Position.Y);
                distance -= 5;
            }
            CurrentRoom = floor[Location.X, Location.Y];
            base.Update(gameTime, clientBounds, floor);
        }

        private void changeTurn(bool turn)
        {
            this.Turn = turn;
            turnTaken = false;
        }
    }
}
