using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class ChaserSprite : Sprite
    {
        int distance = 0;
        int dir;
        bool turnTaken = false;
        //bool turn = false;
        int lastDir;
        int count = 0;
        //public bool Turn { get { return turn; } set { turn = value; } }


        public ChaserSprite(Texture2D textureImage, Point location)
            : base(textureImage, location)
        {
            this.Offset = new Point(-50, 50);
            Difficulty = 1;
            Turn = false;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, UserControlledSprite player, Random rand)
        {
            
            //turnTaken = true;
            if (Turn && !turnTaken)
            {
                if (player.Difficulty == Difficulty)
                {
                    int xDif = player.Location.X - Location.X;
                    int yDif = player.Location.Y - Location.Y;
                   
                    if (Math.Abs(xDif) > Math.Abs(yDif)) //If player is farther in X than Y
                    {
                        //Try to move X
                        if (xDif > 0) //Right
                        {
                            move(2, floor);
                        }
                        else  //Left
                        {
                            move(4, floor);
                        }
                    }
                    if (Math.Abs(xDif) < Math.Abs(yDif)) // Farther on Y than X 
                    {
                        if (yDif > 0) //Down
                        {
                            move(3, floor);
                        }
                        else //Up
                        {
                            move(1, floor);
                        }
                    }
                    if (Math.Abs(xDif) == Math.Abs(yDif)) //Same distance X and Y
                    {
                        if (xDif > 0) //Right
                        {
                            move(2, floor);
                        }
                        else if(xDif < 0) //Left
                        {
                            move(4, floor);
                        }
                        else if (yDif > 0) //Down
                        {
                            move(3, floor);
                        }
                        else //Up
                        {
                            move(1, floor);
                        }
                    }
                }
                
                if (!(distance > 0))
                {
                    do
                    {
                        dir = rand.Next(1, 5);
                        Console.WriteLine("dir " + dir);
                        if (!(dir == lastDir))
                        {
                            lastDir = dir;
                            move(dir, floor);
                        }
                        count += 1;
                        Console.WriteLine("Count " + count);
                        Console.WriteLine("Distance " + distance);
                    } while (!(distance > 0) && count <= 10);
                }
                if (!(distance > 0))
                    lastDir = 0;
                turnTaken = true;
            }


            if (distance == 0)
            {

                if (turnTaken)
                {
                    count = 0;
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

        private void move(int dir, Floor floor)
        {
            switch (dir)
            {
                case 1: //Up
                    if (Location.Y != 0)
                    {
                        if (floor[Location.X, Location.Y - 1].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X, Location.Y - 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                    break;
                case 2: //Right
                    if (Location.X != 27)
                    {
                        if (floor[Location.X + 1, Location.Y].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X + 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                    break;
                case 3: //Down
                    if (Location.Y != 27)
                    {
                        if (floor[Location.X, Location.Y + 1].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X, Location.Y + 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                    break;
                case 4: //Left
                    if (Location.X != 0)
                    {
                        if (floor[Location.X - 1, Location.Y].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.removeContents(this);
                            distance = 300;
                            Location = new Point(Location.X - 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.addContents(this);
                        }
                    }
                    break;
            
            }
        }
    }
}

