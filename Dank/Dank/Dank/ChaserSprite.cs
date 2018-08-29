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
        #region FIELDS
        int distance = 0;
        int dir;
        bool turnTaken = false;
        int lastDir;
        int count = 0;
        bool center = true;
        Vector2 centering = new Vector2(0, 0);
        Vector2 oldCentering;
        bool noMove = true;
        int centerDistance = 0;
        int outsideDistance = 0;
        #endregion
        #region METHODS
        public ChaserSprite(Texture2D textureImage, Vector2 location)
            : base(textureImage, location)
        {
            Health = 4;
            HealthMax = 4;
            Strength = 2;
            Defense = 1;
            Speed = 3;
            Difficulty = 0;
            Turn = false;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, UserControlledSprite player, Random rand)
        {
            
            //turnTaken = true;
            if (Turn && !turnTaken)
            {
                CurrentRoom = floor[Location.X, Location.Y];
                if (player.Difficulty == Difficulty)
                {
                    float xDif = player.Location.X - Location.X;
                    float yDif = player.Location.Y - Location.Y;
                   
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
                        //Console.WriteLine("dir " + dir);
                        if (!(dir == lastDir))
                        {
                            lastDir = dir;
                            move(dir, floor);
                        }
                        count += 1;
                        //Console.WriteLine("Count " + count);
                        //Console.WriteLine("Distance " + distance);
                    } while (!(distance > 0) && count <= 10);
                }
                if (!(distance > 0))
                {
                    lastDir = 0;
                    noMove = true;
                }
                if (!center) { centerDistance = 20; centering = oldCentering; }
                turnTaken = true;
            }


            if (Turn) //Cycled until turn is over
            {
                if (centerDistance > 0) //Sprite needs to move towards center
                {
                    Offset += centering;
                    centerDistance--;
                    //Console.WriteLine(index + " Offset: " + Offset + " Rate: " + centering + " EnemyCount: " + CurrentRoom.EnemyCount);
                    if (centerDistance == 0)
                    {
                        center = true;
                    }

                }
                if (center && distance > 0) //Sprite has moved to middle of room and is ready to move to new room
                {
                    if (this is WraithChaserSprite)
                    {
                        this.Animations.Tint = StaticTint * .2f;
                    }
                    if (dir == 1)
                    {
                        if (this is WraithChaserSprite && Animations.CurrentAnimation != "walkup")
                            Animations.CurrentAnimation = "walkup";
                        Position = new Point(Position.X, Position.Y - 5);
                    }
                    if (dir == 2)
                    {
                        if (this is WraithChaserSprite && Animations.CurrentAnimation != "walkright")
                            Animations.CurrentAnimation = "walkright";
                        Position = new Point(Position.X + 5, Position.Y);
                    }
                    if (dir == 3)
                    {
                        if (this is WraithChaserSprite && Animations.CurrentAnimation != "walkdown")
                            Animations.CurrentAnimation = "walkdown";
                        Position = new Point(Position.X, Position.Y + 5);
                    }
                    if (dir == 4)
                    {
                        if (this is WraithChaserSprite && Animations.CurrentAnimation != "walkleft")
                            Animations.CurrentAnimation = "walkleft";
                        Position = new Point(Position.X - 5, Position.Y);
                    }
                    distance--;
                    if (distance == 0) //Sprite is in center of new room
                    {
                        outsideDistance = 20;
                        center = false;
                        centering = getRate(CurrentRoom.Enemies.IndexOf(this));
                        oldCentering = centering; //Always go back the same way it came
                    }
                }
                if (noMove && center) // Didn't change rooms but still in the center
                {
                    outsideDistance = 20;
                    center = false;
                    centering = getRate(CurrentRoom.Enemies.IndexOf(this));
                    oldCentering = centering; //Always go back the same way it came
                }
                if (outsideDistance > 0) //Sprite has moved into the new room but the turn isn't over
                {
                    if (this is WraithChaserSprite)
                    {
                        this.Animations.Tint = StaticTint * .5f;
                    }
                    Offset += -centering;
                    outsideDistance--;
                }
                if (outsideDistance == 0 && distance == 0 && centerDistance == 0) //END OF TURN Sprite has changed rooms and walked to its offset
                {
                    //Console.WriteLine("Turn End " + index + " Offset: " + Offset + " Rate: " + centering + " EnemyCount: " + CurrentRoom.EnemyCount);
                    if (Animations.CurrentAnimation != "idle")
                    {
                        Animations.CurrentAnimation = "idle";
                    }
                    changeTurn(false);
                }
            }
            
            base.Update(gameTime, clientBounds, floor);
        }

        private void changeTurn(bool turn)
        {
            this.Turn = turn;
            turnTaken = false;
        }

        private void move(int dir, Floor floor)
        {
            noMove = false;
            switch (dir)
            {
                case 1: //Up
                    if (Location.Y != 0)
                    {
                        if (floor[Location.X, Location.Y - 1].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.RemoveContent(this);
                            distance = 60;
                            Location = new Vector2(Location.X, Location.Y - 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.AddContent(this);
                        }
                    }
                    break;
                case 2: //Right
                    if (Location.X != 26)
                    {
                        if (floor[Location.X + 1, Location.Y].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.RemoveContent(this);
                            distance = 60;
                            Location = new Vector2(Location.X + 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.AddContent(this);
                        }
                    }
                    break;
                case 3: //Down
                    if (Location.Y != 26)
                    {
                        if (floor[Location.X, Location.Y + 1].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.RemoveContent(this);
                            distance = 60;
                            Location = new Vector2(Location.X, Location.Y + 1);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.AddContent(this);
                        }
                    }
                    break;
                case 4: //Left
                    if (Location.X != 0)
                    {
                        if (floor[Location.X - 1, Location.Y].moveRequest(this))
                        {
                            this.dir = dir;
                            CurrentRoom.RemoveContent(this);
                            distance = 60;
                            Location = new Vector2(Location.X - 1, Location.Y);
                            CurrentRoom = floor[Location.X, Location.Y];
                            CurrentRoom.AddContent(this);
                        }
                    }
                    break;
            
            }
        }

        private Vector2 getRate(int index)
        {
            if (index == 0)
            {
                return Vector2.Zero;
            }
            if (index == 1)
            {
                return new Vector2(5, 5);
            }
            if (index == 2)
            {
                return new Vector2(-5, 5);
            }
            if (index == 3)
            {
                return new Vector2(5, -5);
            }
            if (index == 4)
            {
                return new Vector2(-5, -5);
            }
            return Vector2.Zero;
        }
        #endregion
    }
}

