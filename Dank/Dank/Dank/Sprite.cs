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
        #region FIELDS
        Texture2D textureImage;
        Vector2 location;
        Point position;
        SpriteAnimation animations;
        Color staticTint;
        Room currentRoom;
        Vector2 offset;
        int difficulty;
        bool turn = true; //true if it is the player's turn
        public int index;
        int strength = 0;
        int defense = 0;
        int speed = 0;
        int health = 0;
        int healthMax = 0;
        #endregion
        #region PROPERTIES
        public bool Turn { get { return turn; } set { turn = value; } }
        public int Difficulty { get { return difficulty; } set { difficulty = value; } }
        public Vector2 Offset { get { return offset; } set { offset = value; } }
        public SpriteAnimation Animations { get { return animations; } }
        public Room CurrentRoom { get { return currentRoom; } set { currentRoom = value; } }
        public Point Position { get { return position; } set { position = value; } }
        public Vector2 Location { get { return location; } set { location = value; } }
        public int Strength { get { return strength; } set { strength = value; } }
        public int Defense { get { return defense; } set { defense = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public int Health { get { return health; } set { health = value; } }
        public int HealthMax { get { return healthMax; } set { healthMax = value; } }
        public Color StaticTint { get { return staticTint; } }
        #endregion
        #region METHODS
        //constructor with defined scale
        public Sprite(Texture2D textureImage, Vector2 location)
        {
            this.location = location;
            this.textureImage = textureImage;
            this.position.X = (int)location.X * 300 -15;
            this.position.Y = (int)location.Y * 300 -25;
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
            animations.Draw(spriteBatch, position.X + (int)offset.X, position.Y + (int)offset.Y);
        }
        public void SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
            switch (difficulty)
            {
                case 0:
                    staticTint = Color.White;
                    break;
                case 1:
                    Animations.Tint = Color.Yellow;
                    staticTint = Color.Yellow;
                    if (this is WraithChaserSprite)
                    {
                        Strength += 0;
                        Defense += 0;
                        Speed += 1;
                        Health += 1;
                        HealthMax += 1;
                        break;
                    }
                    Strength += 1;
                    Defense += 1;
                    Speed += 1;
                    Health += 1;
                    HealthMax += 1;
                    
                    break;
                    
                case 2:
                    Animations.Tint = Color.Orange;
                    staticTint = Color.Orange;
                    if (this is WraithChaserSprite)
                    {
                        Strength += 1;
                        Defense += 0;
                        Speed += 1;
                        Health += 1;
                        HealthMax += 1;
                        break;
                    }
                    Strength += 4;
                    Defense += 4;
                    Speed += 4;
                    Health += 2;
                    HealthMax += 2;
                    
                    break;
                case 3:
                    Animations.Tint = Color.Red;
                    staticTint = Color.Red;
                    if (this is WraithChaserSprite)
                    {
                        Strength += 1;
                        Defense += 1;
                        Speed += 2;
                        Health += 2;
                        HealthMax += 2;
                        break;
                    }
                    Strength += 6;
                    Defense += 6;
                    Speed += 6;
                    Health += 3;
                    HealthMax += 3;
                    
                    break;
                case 4:
                    Animations.Tint = Color.Black;
                    staticTint = Color.Black;
                    if (this is WraithChaserSprite)
                    {
                        Strength += 2;
                        Defense += 2;
                        Speed += 3;
                        Health += 3;
                        HealthMax += 3;
                        break;
                    }
                    Strength += 7;
                    Defense += 7;
                    Speed += 7;
                    Health += 4;
                    HealthMax += 4;
                    
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
