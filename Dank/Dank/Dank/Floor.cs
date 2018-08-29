using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class Floor
    {
        Room[,] floor;
        public Room this[float index1, float index2] { get { return floor[(int)index1, (int)index2]; } set { floor[(int)index1, (int)index2] = value; } }
        

        //constructor
        public Floor() 
        {
            floor = new Room[27, 27];     
        }
        
    }
}
