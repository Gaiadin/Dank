using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class Consumable : Item
    {
        #region FIELDS
        int uses = 1;

        #endregion
        #region PROPERTIES
        public int Uses { get { return uses; } set { uses = value; } }

        #endregion

        public Consumable(Texture2D image)
            : base(image)
        {
        }
        public Consumable(Consumable item)
            : base(item)
        {
            this.uses = item.uses;
        }

        public void Use(UserControlledSprite player)
        {

            if (Name == "Potion")
            {
                //Code for potion
            }
            if (Name == "Energy Potion")
            {
                if (player.RotationEnergy < player.RotationEnergyMax)
                {
                    player.RotationEnergy += Effect;
                    if (player.RotationEnergy > player.RotationEnergyMax)
                        player.RotationEnergy = player.RotationEnergyMax;
                    uses -= 1;
                }
            }
            if (uses <= 0)
                player.Inventory.Remove(this);
        }
        
    }
}
