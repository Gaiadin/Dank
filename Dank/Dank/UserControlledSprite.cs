using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    class UserControlledSprite : Sprite
    {
        int distance = 0;
        int rotation; // time for rotation
        int rotationEnergy = 10; //current rotation energy;
        int rotationEnergyMax = 10;
        bool rotated = false;
        KeyboardState oldState;
        
        int dir = 0;
        
        bool turnTaken = false; //check if turn has been taken
        public int RotationEnergy { get { return rotationEnergy; } }
        public int RotationEnergyMax { get { return rotationEnergyMax; } }
        

        //constructor with defined scale
        public UserControlledSprite(Texture2D textureImage, Point location)
            : base(textureImage, location)
        {
            Position = new Point(-10, -50);
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, SpriteManager spriteManager)
        {
            CurrentRoom = floor[Location.X, Location.Y];
            if (distance == 0 && rotation == 0)
            {
                
                if (Animations.CurrentAnimation != "idle")
                {
                    Animations.CurrentAnimation = "idle";
                }
                
                if (turnTaken && Turn)
                {
                    if (!rotated && rotationEnergy < rotationEnergyMax)
                    {
                        rotationEnergy += 1;
                    }

                    changeTurn(false);
                    rotated = false;
                }
                
                if(Turn)
                    oldState=UpdateInput(oldState, floor, spriteManager);
               
            }
            if (rotation > 0)
            {
                rotation -= 5;
                
            }
            if (distance > 0)
            {
                
                if(dir == 1)
                    Position = new Point (Position.X, Position.Y - 5);
                if (dir == 2)
                    Position = new Point(Position.X + 5, Position.Y);
                if (dir == 3)
                    Position = new Point(Position.X, Position.Y + 5);
                if (dir == 4)
                    Position = new Point(Position.X - 5, Position.Y);
                distance -= 5;
                
            }
            
            base.Update(gameTime, clientBounds, floor);
        }

        private KeyboardState UpdateInput(KeyboardState oldState, Floor floor, SpriteManager spriteManager)
        {
            KeyboardState newState = Keyboard.GetState();
            
            //checking up
            //checking if at the top of the map
            if (Location.Y != 0)
            {
                // Is the Up key down?
                if (newState.IsKeyDown(Keys.Up))
                {
                    // If not down last update, key has just been pressed.
                    if (!oldState.IsKeyDown(Keys.Up))
                    {

                    }
                }
                else if (oldState.IsKeyDown(Keys.Up))
                {
                    // Key was down last update, but not down now, so
                    // it has just been released.
                    if (floor[Location.X, Location.Y - 1].moveRequest(this))
                    {
                        dir = 1;
                        CurrentRoom.removeContents(this);
                        distance = 300;
                        Location = new Point(Location.X, Location.Y - 1);
                        CurrentRoom = floor[Location.X, Location.Y];
                        CurrentRoom.addContents(this);
                        Animations.CurrentAnimation = "walkup";
                        turnTaken = true;
                    }
                        
                }
                
            }
            //checking right
            if (Location.X != 27)
            {
                // Is the Right key down?
                if (newState.IsKeyDown(Keys.Right))
                {
                    // If not down last update, key has just been pressed.
                    if (!oldState.IsKeyDown(Keys.Right))
                    {

                    }
                }
                else if (oldState.IsKeyDown(Keys.Right))
                {
                    // Key was down last update, but not down now, so
                    // it has just been released.
                    if (floor[Location.X + 1, Location.Y].moveRequest(this))
                    {
                        dir = 2;
                        CurrentRoom.removeContents(this);
                        distance = 300;
                        Location = new Point(Location.X + 1, Location.Y);
                        CurrentRoom = floor[Location.X, Location.Y];
                        CurrentRoom.addContents(this);
                        Animations.CurrentAnimation = "walkright";
                        turnTaken = true;
                    }
                }
            }
            //checking down
            if (Location.Y != 27)
            {
                // Is the Down key down?
                if (newState.IsKeyDown(Keys.Down))
                {
                    // If not down last update, key has just been pressed.
                    if (!oldState.IsKeyDown(Keys.Down))
                    {
                    }
                }
                else if (oldState.IsKeyDown(Keys.Down))
                {
                    // Key was down last update, but not down now, so
                    // it has just been released.
                    if (floor[Location.X, Location.Y + 1].moveRequest(this))
                    {
                        dir = 3;
                        CurrentRoom.removeContents(this);
                        distance = 300;
                        Location = new Point(Location.X, Location.Y + 1);
                        CurrentRoom = floor[Location.X, Location.Y];
                        CurrentRoom.addContents(this);
                        Animations.CurrentAnimation = "walkdown";
                        turnTaken = true;
                    }
                }
            }
            //checking left
            if (Location.X != 0)
            {
                // Is the Left key down?
                if (newState.IsKeyDown(Keys.Left))
                {
                    // If not down last update, key has just been pressed.
                    if (!oldState.IsKeyDown(Keys.Left))
                    {

                    }
                }
                else if (oldState.IsKeyDown(Keys.Left))
                {
                    // Key was down last update, but not down now, so
                    // it has just been released.
                    if (floor[Location.X - 1, Location.Y].moveRequest(this))
                    {
                        CurrentRoom.removeContents(this);
                        dir = 4;
                        distance = 300;
                        Location = new Point(Location.X - 1, Location.Y);
                        CurrentRoom = floor[Location.X, Location.Y];
                        CurrentRoom.addContents(this);
                        Animations.CurrentAnimation = "walkleft";
                        turnTaken = true;
                    }
                }
            }
            // Update saved state.

            //Enter to rotate room
            if (newState.IsKeyDown(Keys.Enter))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Enter))
                {

                }
            }
            else if (oldState.IsKeyDown(Keys.Enter))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
                //ROTATE ADJACENT ROOMS





                if (newState.IsKeyDown(Keys.A) && Location.X > 0 && rotationEnergy >= 5)
                {
                    floor[Location.X - 1, Location.Y].rotate();
                    spriteManager.PlayCue("rotate1");
                    rotated = true;
                    rotationEnergy -= 5;
                    turnTaken = true;
                    rotation = 200;
                }
                else if (newState.IsKeyDown(Keys.D) && Location.X < 27 && rotationEnergy >= 5)
                {
                    floor[Location.X + 1, Location.Y].rotate();
                    spriteManager.PlayCue("rotate1");
                    rotated = true;
                    rotationEnergy -= 5;
                    turnTaken = true;
                    rotation = 200;
                }
                else if (newState.IsKeyDown(Keys.W) && Location.Y > 0 && rotationEnergy >= 5)
                {
                    floor[Location.X, Location.Y - 1].rotate();
                    spriteManager.PlayCue("rotate1");
                    rotated = true;
                    rotationEnergy -= 5;
                    turnTaken = true;
                    rotation = 200;
                }
                else if (newState.IsKeyDown(Keys.S) && Location.Y < 27 && rotationEnergy >= 5)
                {
                    floor[Location.X, Location.Y + 1].rotate();
                    spriteManager.PlayCue("rotate1");
                    rotated = true;
                    rotationEnergy -= 5;
                    turnTaken = true;
                    rotation = 200;
                }
                else if ((newState.GetPressedKeys().Length == 0) &&  rotationEnergy >= 2)
                {
                    floor[Location.X, Location.Y].rotate(); // ROTATE CURRENT ROOM
                    spriteManager.PlayCue("rotate1");
                    rotated = true;
                    rotationEnergy -= 2;
                    turnTaken = true;
                    rotation = 200;
                }
                //else
                    //spriteManager.PlayCue("error");

                
            }
            //Space to skip turn
            if (newState.IsKeyDown(Keys.Space))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Space))
                {

                }
            }
            else if (oldState.IsKeyDown(Keys.Space))
            {
                turnTaken = true;
            }
            oldState = newState;
            return oldState;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Animations.Draw(spriteBatch, 500, 380);
        }
        private void changeTurn(bool turn)
        {
            this.Turn = turn;
            turnTaken = false;
        }
    }
}
