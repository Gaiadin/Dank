using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    class BGLayer
    {

        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;
        protected float scale;
        protected Vector2 speed;
        protected Vector2 position;
        int clientWidth;

        Vector2 cameraPosition;
        float backGroundScroll;
        public Matrix screenMatrix;

        //constructor with default animation speed
        public BGLayer(Texture2D textureImage, Vector2 position, Point frameSize,
            Point currentFrame, Point sheetSize, Vector2 speed, float scale, float bgScroll, int clientWidth)
            : this(textureImage, position, frameSize, currentFrame, sheetSize, speed, scale, bgScroll, clientWidth, defaultMillisecondsPerFrame)
        {    
        }

        //constructor with defined animation speed
        public BGLayer(Texture2D textureImage, Vector2 position, Point frameSize,
            Point currentFrame, Point sheetSize, Vector2 speed, float scale, float bgScroll, int clientWidth, int millisecondsPerFrame)
        {
            this.backGroundScroll = bgScroll;
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.scale = scale;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.clientWidth = clientWidth;
        }

        

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            

            //if (Keyboard.GetState().IsKeyDown(Keys.Left))
            //    cameraPosition.X -= 5;
            //if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    cameraPosition.X += 5;
            //if (Keyboard.GetState().IsKeyDown(Keys.Up))
            //    cameraPosition.Y -= 5;
            //if (Keyboard.GetState().IsKeyDown(Keys.Down))
            //    cameraPosition.Y += 5;

            
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //screenMatrix = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0));
            //background stuff
            //backestground
            Rectangle layer3Rectangle = new Rectangle(
                (int)Math.Round(cameraPosition.X * backGroundScroll / scale),
                0, clientWidth, frameSize.Y);



            spriteBatch.Draw(textureImage, Vector2.Zero,textureImage.Bounds,Color.White,0,Vector2.Zero,3f,SpriteEffects.None,0);
        }
    }
}
