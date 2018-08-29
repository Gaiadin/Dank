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
        public Room this[int index1, int index2] { get { return floor[index1, index2]; } set { floor[index1, index2] = value; } }
        

        //constructor
        public Floor() 
        {
            floor = new Room[28, 28];     
        }
        
    }
}
