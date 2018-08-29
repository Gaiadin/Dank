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
        public WraithChaserSprite(Texture2D textureImage, Point location)
            : base(textureImage, location)
        {
            this.Offset = new Point(-50, -50);
            Difficulty = 0;
            Turn = false;
        }
    }
}
