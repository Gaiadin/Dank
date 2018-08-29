using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    abstract class Sprite
    {
        Texture2D textureImage;
        Point location;
        Point position;
        SpriteAnimation animations;
        Room currentRoom;
        Point offset;
        int difficulty;
        bool turn = true; //true if it is the player's turn

        public bool Turn { get { return turn; } set { turn = value; } }
        public int Difficulty { get { return difficulty; } set { difficulty = value; } }
        public Point Offset { get { return offset; } set { offset = value; } }
        public SpriteAnimation Animations { get { return animations; } }
        public Room CurrentRoom { get { return currentRoom; } set { currentRoom = value; } }
        public Point Position { get { return position; } set { position = value; } }
        public Point Location { get { return location; } set { location = value; } }

        //constructor with defined scale
        public Sprite(Texture2D textureImage, Point location)
        {
            this.location = location;
            this.textureImage = textureImage;
            this.position.X = location.X * 300 -15;
            this.position.Y = location.Y * 300 -25;
            animations = new SpriteAnimation(textureImage);
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds, Floor floor)
        {
            animations.Update(gameTime);  
        }
        public virtual void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, SpriteManager spriteManager)
        {
            animations.Update(gameTime);
        }
        public virtual void Update(GameTime gameTime, Rectangle clientBounds, Floor floor,UserControlledSprite player, Random rand)
        {
            animations.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animations.Draw(spriteBatch, position.X + offset.X, position.Y + offset.Y);
        }

    }
}
