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
        #region FIELDS
        int distance = 0;
        int dir;
        bool turnTaken = false;
        //for centering
        bool center = true;
        Vector2 centering = new Vector2(0,0);
        Vector2 oldCentering;
        Vector2 newLoc;
        
        int centerDistance = 0;
        int outsideDistance = 0;

        #endregion
        #region METHODS
        public AutomatedSprite(Texture2D textureImage, Vector2 location)
            : base(textureImage, location)
        {
            Health = 4;
            HealthMax = 4;
            Strength = 3;
            Defense = 2;
            Speed = 1;
            
            Turn = false;
        }

        public  override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, UserControlledSprite player, Random rand)
        {
            
            if (Turn && !turnTaken) //Once per turn
            {
                CurrentRoom = floor[Location.X, Location.Y];
                dir = rand.Next(1, 5); //Choose direction
                if(dir == 1 && Location.Y != 0) //Up and not at top
                    if (floor[Location.X, Location.Y - 1].moveRequest(this))//Allowed to move up
                    {
                        distance = 60;
                        newLoc = new Vector2(0, -1);
                    }
                if (dir == 2 && Location.X != 26) //Right and not at right edge
                {
                    if (floor[Location.X + 1, Location.Y].moveRequest(this)) //Allowed to move right
                    {
                        distance = 60;
                        newLoc = new Vector2(1, 0);
                    }
                }
                if (dir == 3 && Location.Y != 26) //Down and not at bottom edge
                {
                    if (floor[Location.X, Location.Y + 1].moveRequest(this))
                    {
                        distance = 60;
                        newLoc = new Vector2(0, 1);
                    }
                }
                if (dir == 4 && Location.X != 0) //Down and not at bottom edge
                {
                    if (floor[Location.X - 1, Location.Y].moveRequest(this))
                    {
                        distance = 60;
                        newLoc = new Vector2(-1, 0);
                    }
                }
                if (distance == 0) { newLoc = Vector2.Zero; }
                else
                {
                    CurrentRoom.RemoveContent(this);
                    Location += newLoc;
                    CurrentRoom = floor[Location.X, Location.Y];
                    CurrentRoom.AddContent(this);
                } //Remove from room if it is moving
                if (!center) { centerDistance = 20; centering = oldCentering; }
                
                turnTaken = true;
            }//^^Once per turn^^

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
                    if (dir == 1)
                        Position = new Point(Position.X, Position.Y - 5);
                    if (dir == 2)
                        Position = new Point(Position.X + 5, Position.Y);
                    if (dir == 3)
                        Position = new Point(Position.X, Position.Y + 5);
                    if (dir == 4)
                        Position = new Point(Position.X - 5, Position.Y);
                    distance--;
                    if (distance == 0) //Sprite is in center of new room
                    {
                        
                        
                        
                        outsideDistance = 20;
                        center = false;
                        centering = getRate(CurrentRoom.Enemies.IndexOf(this));
                        oldCentering = centering; //Always go back the same way it came
                    }
                }
                if (newLoc == Vector2.Zero && center) // Didn't change rooms but still in the center
                {
                    outsideDistance = 20;
                    center = false;
                    centering = getRate(CurrentRoom.Enemies.IndexOf(this));
                    oldCentering = centering; //Always go back the same way it came
                }
                if (outsideDistance > 0) //Sprite has moved into the new room but the turn isn't over
                {
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
        private void changeTurn(bool turn)
        {
            this.Turn = turn;
            turnTaken = false;
            //distance = 0;
            //centerDistance = 0;
            //outsideDistance = 0;
            //isOutside = true;
            //center = false;
        }
        #endregion
    }
}
