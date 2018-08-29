using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dank
{
    class Item
    {
        #region FIELDS
        String name = "Generic Object";
        String type = "consumable"; //Item Type 0-Consumable, 1-Weapon, 2-Armor, 3-Accessory
        String description = "";
        //int subType = 0; //Values depend on type.  type 0, subType 0-Health Potion; type 0, subType 1-Energy Potion; type 1, subType 0-Sword;
        int value = 0;
        int effect = 1;
        int uses = 1;
        Texture2D image;
        Buff buff;

        #endregion
        #region PROPERTIES
        public String Name { get { return name; } set { name = value; } }
        public String Description { get { return description; } set { description = value; } }
        public int Value { get { return value; } set { this.value = value; } }
        public int Effect { get { return effect; } set { effect = value; } }
        public int Uses { get { return uses; } set { uses = value; } }
        public Texture2D Image { get { return image; } }
        public string Type { get { return type; } }
        public Buff Buff { get { return buff; } set { buff = value; } }
        #endregion

        public Item(Texture2D image)
        {
            this.image = image;
        }
        public Item(Item item)
        {
            this.image = item.image;
            this.type = item.type;
            this.effect = item.effect;
            this.value = item.value;
            this.name = item.name;
            this.uses = item.uses;
            this.description = item.description;
            this.buff = item.buff;
        }

        public int Use(UserControlledSprite player)
        {
            if (type == "consumable")
            {
                if (Name == "Potion")
                {
                    if (player.Health < player.HealthMax)
                    {
                        SpriteManager.PlayCue("use");
                        player.AddHealth(this.effect);
                        uses -= 1;
                    }
                }
                if (Name == "Energy Potion")
                {
                    
                    if (player.RotationEnergy < player.RotationEnergyMax)
                    {
                        SpriteManager.PlayCue("use");
                        player.RotationEnergy += Effect;
                        if (player.RotationEnergy > player.RotationEnergyMax)
                            player.RotationEnergy = player.RotationEnergyMax;
                        uses -= 1;
                    }
                }
                if (Name == "Elixir of Strength")
                {
                    if (!player.CheckBuffs(buff))
                    {
                        SpriteManager.PlayCue("use");
                        player.AddBuff(buff);
                        uses--;
                    }
                }
                if (Name == "Stoneskin Potion")
                {
                    if (!player.CheckBuffs(buff))
                    {
                        SpriteManager.PlayCue("use");
                        player.AddBuff(buff);
                        uses--;
                    }
                }
                if (uses <= 0)
                {
                    player.Inventory.Remove(this);
                    Console.WriteLine("Consumable Gone");
                }
                return uses;
            }
            return -1;
        }
    }
}
