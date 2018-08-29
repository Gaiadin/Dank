using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    class Camera
    {
        public Matrix transform;
        //Viewport view;
        Vector2 view;
        Vector2 center;

        public Camera(Rectangle newView)
        {
            view = new Vector2(newView.Height * .5f, newView.Width * .5f);
        }

        public void Update(GameTime gameTime ,UserControlledSprite player)
        {
            center = new Vector2(player.Position.X, player.Position.Y);
            transform = Matrix.CreateTranslation(view.X - player.Position.X+100, view.Y - player.Position.Y-125, 0f);
        }
    }
}
