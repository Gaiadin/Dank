using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class WraithChaserSprite: ChaserSprite
    {
        public WraithChaserSprite(Texture2D textureImage, Vector2 location)
            : base(textureImage, location)
        {
            Health = 3;
            HealthMax = 3;
            Strength = 2;
            Defense = 1;
            Speed = 2;
            Turn = false;
            this.Animations.Tint = this.Animations.Tint * .5f;
        }
    }
}
