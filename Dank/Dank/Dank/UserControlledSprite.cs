using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    class UserControlledSprite : Sprite
    {
        #region FIELDS
        List<Item> inventory = new List<Item>();
        List<Buff> buffs = new List<Buff>();
        int inventoryMax = 10;
        int distance = 0;
        int rotation; // time for rotation
        int rotationEnergy = 10; //current rotation energy;
        int rotationEnergyMax = 10;
        int dir = 0;
        bool turnTaken = false; //check if turn has been taken
        bool rotated = false;
        KeyboardState oldState;
        Dictionary<Vector2, String> dictDirections = new Dictionary<Vector2, String>();
        int turnCount = 0;
        int exp = 0;
        int level = 1;
        #endregion
        #region PROPERTIES
        public int TurnCount { get { return turnCount; } }
        public int RotationEnergy { get { return rotationEnergy; } set { rotationEnergy = value; } }
        public int RotationEnergyMax { get { return rotationEnergyMax; } set { rotationEnergyMax = value; } }
        public List<Item> Inventory { get { return inventory; } set { inventory = value; } }
        public List<Buff> Buffs { get { return buffs; } set { buffs = value; } }
        public int Exp { get { return exp; } set { exp = value; } }
        public int Level { get { return level; } set { level = value; } }
        #endregion
        #region METHODS
        //constructor with defined scale
        public UserControlledSprite(Texture2D textureImage, Vector2 location)
            : base(textureImage, location)
        {
            Position = new Point(-10, -50);
            Health = 10;
            HealthMax = 10;
            Strength = 4;
            Defense = 1;
            Speed = 2;
            //Setup dictionary directions for movement
            dictDirections.Add(new Vector2(0, 1), "walkup");
            dictDirections.Add(new Vector2(0, -1), "walkdown");
            dictDirections.Add(new Vector2(1, 0), "walkleft");
            dictDirections.Add(new Vector2(-1, 0), "walkright");
        }

        public void AddHealth(int amount)
        {
            if (Health + amount > HealthMax)
                Health = HealthMax;
            else
                Health += amount;
        }
        public void SubtractHealth(int amount)
        {
            if (Health - amount <= 0)
            {
                //Death
                Health = 0;
                Game1.GAMEOVER = true;
            }
            else
                Health -= amount;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds, Floor floor, SpriteManager spriteManager)
        {
            if (Game1.PAUSED)
            {
                base.Update(gameTime, clientBounds, floor);
                
                return;
            }

            if (!turnTaken && Turn)
            {
                Game1.BATTLE = spriteManager.BattleChecker();
                //oldState = Keyboard.GetState();
            }
            if (distance == 0 && rotation == 0)
            {
                
                if (Animations.CurrentAnimation != "idle")
                {
                    Animations.CurrentAnimation = "idle";
                }
                
                if (turnTaken && Turn)
                {
                    if (!rotated && rotationEnergy < rotationEnergyMax)
                    {
                        rotationEnergy += 1;
                    }
                    if (buffs.Count > 0)
                    {
                        for (int i = 0; i < buffs.Count; i++)
                        {
                            buffs[i].Duration--;
                            if (buffs[i].Duration == 0)
                            {
                                RemoveBuff(buffs[i]);
                            }
                        }
                    }
                    CurrentRoom = floor[Location.X, Location.Y];
                    CurrentRoom.AddContent(this);
                    spriteManager.NewSpawn();
                    //Console.WriteLine("end of turn");
                    Game1.BATTLE = spriteManager.BattleChecker();
                    if (CurrentRoom.Enemies.Count == 0)
                    {
                        changeTurn(false);
                        rotated = false;
                    }
                }
                
                if(Turn && !Game1.BATTLE)
                    oldState=UpdateInput(oldState, floor, spriteManager);
               
            }
            if (rotation > 0)
            {
                rotation -= 5;
            }
            if (distance > 0)
            {
                
                if(dir == 1)
                    Position = new Point (Position.X, Position.Y - 5);
                if (dir == 2)
                    Position = new Point(Position.X + 5, Position.Y);
                if (dir == 3)
                    Position = new Point(Position.X, Position.Y + 5);
                if (dir == 4)
                    Position = new Point(Position.X - 5, Position.Y);
                distance -= 5;
                
            }
            
            base.Update(gameTime, clientBounds, floor);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Animations.CurrentAnimation != "fight")
            {
                Animations.Draw(spriteBatch, 500, 380);
            }
            else
            {
                Animations.Draw(spriteBatch, 470, 380);
            }
        }

        private KeyboardState UpdateInput(KeyboardState oldState, Floor floor, SpriteManager spriteManager)
        {
            KeyboardState newState = Keyboard.GetState();
            //checking if at the top of the map
            if (Location.Y != 0)
            {
                if (oldState.IsKeyDown(Keys.Up) && !(newState.IsKeyDown(Keys.Up)))
                {
                    dir = 1;
                    move(floor, new Vector2(0, 1));
                }
            }
            //checking right
            if (Location.X != 26)
            {
                if (oldState.IsKeyDown(Keys.Right) && !(newState.IsKeyDown(Keys.Right)))
                {
                    dir = 2;
                    move(floor, new Vector2(-1, 0));
                }
            }
            //checking down
            if (Location.Y != 26)
            {
                if (oldState.IsKeyDown(Keys.Down) && !(newState.IsKeyDown(Keys.Down)))
                {
                    dir = 3;
                    move(floor, new Vector2(0, -1));
                }
            }
            //checking left
            if (Location.X != 0)
            {
                if (oldState.IsKeyDown(Keys.Left) && !(newState.IsKeyDown(Keys.Left)))
                {
                    dir = 4;
                    move(floor, new Vector2(1, 0));
                }
            }
            if (oldState.IsKeyDown(Keys.Enter) && !(newState.IsKeyDown(Keys.Enter)))
            {
                //ROTATE ADJACENT ROOMS

                if (newState.IsKeyDown(Keys.A) && Location.X > 0 && rotationEnergy >= 5)
                {
                    rotate(floor, spriteManager, new Vector2(1, 0));
                }
                else if (newState.IsKeyDown(Keys.D) && Location.X < 26 && rotationEnergy >= 5)
                {
                    rotate(floor, spriteManager, new Vector2(-1, 0));
                }
                else if (newState.IsKeyDown(Keys.W) && Location.Y > 0 && rotationEnergy >= 5)
                {
                    rotate(floor, spriteManager, new Vector2(0, 1));
                }
                else if (newState.IsKeyDown(Keys.S) && Location.Y < 26 && rotationEnergy >= 5)
                {
                    rotate(floor, spriteManager, new Vector2(0, -1));
                }
                else if ((newState.GetPressedKeys().Length == 0) &&  rotationEnergy >= 2)
                {
                    //Rotate current room
                    rotate(floor, spriteManager, Vector2.Zero);
                }
                //else
                    //spriteManager.PlayCue("error");

                
            }

            if (oldState.IsKeyDown(Keys.Back) && !(newState.IsKeyDown(Keys.Back)) && level >= 5)
            {
                if (newState.IsKeyDown(Keys.A) && Location.X > 0)
                {
                    if (SubtractEnergy(10))
                    {
                        CurrentRoom.AddExit(4);
                        floor[Location.X - 1, Location.Y].AddExit(2);
                    }
                }
                else if (newState.IsKeyDown(Keys.D) && Location.X < 26)
                {
                    if (SubtractEnergy(10))
                    {
                        CurrentRoom.AddExit(2);
                        floor[Location.X + 1, Location.Y].AddExit(4);
                    }
                }
                else if (newState.IsKeyDown(Keys.W) && Location.Y > 0)
                {
                    if (SubtractEnergy(10))
                    {
                        CurrentRoom.AddExit(1);
                        floor[Location.X, Location.Y - 1].AddExit(3);
                    }
                }
                else if (newState.IsKeyDown(Keys.S) && Location.Y < 26)
                {
                    if (SubtractEnergy(10))
                    {
                        CurrentRoom.AddExit(3);
                        floor[Location.X, Location.Y + 1].AddExit(1);
                    }
                }
                
            }
            //if (oldState.IsKeyDown(Keys.Delete) && !(newState.IsKeyDown(Keys.Delete)))
            //{
            //    if (newState.IsKeyDown(Keys.A) && Location.X > 0)
            //    {
            //        CurrentRoom.RemoveExit(4);
            //        floor[Location.X - 1, Location.Y].RemoveExit(2);
            //    }
            //    else if (newState.IsKeyDown(Keys.D) && Location.X < 26)
            //    {
            //        CurrentRoom.RemoveExit(2);
            //        floor[Location.X + 1, Location.Y].RemoveExit(4);
            //    }
            //    else if (newState.IsKeyDown(Keys.W) && Location.Y > 0)
            //    {
            //        CurrentRoom.RemoveExit(1);
            //        floor[Location.X, Location.Y - 1].RemoveExit(3);
            //    }
            //    else if (newState.IsKeyDown(Keys.S) && Location.Y < 26)
            //    {
            //        CurrentRoom.RemoveExit(3);
            //        floor[Location.X, Location.Y + 1].RemoveExit(1);
            //    }

            //}
            if (oldState.IsKeyDown(Keys.Home) && !(newState.IsKeyDown(Keys.Home)))
            {
                SpriteManager.SkipTurn = !SpriteManager.SkipTurn;
            }
            if (oldState.IsKeyDown(Keys.E) && !(newState.IsKeyDown(Keys.E)))
            {
                Item check = checkInventory(ItemList.Get("Energy Potion"));
                if (check != null)
                    check.Use(this);
            }
            if (oldState.IsKeyDown(Keys.P) && !(newState.IsKeyDown(Keys.P)))
            {
                Item check = checkInventory(ItemList.Get("Potion"));
                if (check != null)
                    check.Use(this);
            }
            //if (oldState.IsKeyDown(Keys.F) && !newState.IsKeyDown(Keys.F))
            //{
            //    AddItem(ItemList.Get("Elixir of Strength"));
            //    AddItem(ItemList.Get("Stoneskin Potion"));
            //}
            if (oldState.IsKeyDown(Keys.Escape) && !newState.IsKeyDown(Keys.Escape))
            {
                SpriteManager.ShowOptions();
            }
            if (oldState.IsKeyDown(Keys.Space) && !newState.IsKeyDown(Keys.Space))
            {
                turnTaken = true;
            }
            if (oldState.IsKeyDown(Keys.I) && !newState.IsKeyDown(Keys.I))
            {
                ShowInventory();
            }
            if (oldState.IsKeyDown(Keys.C) && !newState.IsKeyDown(Keys.C))
            {
                ShowStatus();
            }
            if (oldState.IsKeyDown(Keys.H) && !newState.IsKeyDown(Keys.H))
            {
                UI.ShowMessage("Controls:  ");
                UI.ShowMessage("Skip Turn: SPACE  Quick Use Energy Potion: E  Quick Use Potion: P");
                UI.ShowMessage("Status: C  Inventory: I  Volume Controls: ESC  Help: H  Move: ARROWS");
                UI.ShowMessage("Rotate Room: ENTER  Rotate Adjacent room: W,A,S,D + ENTER");
                UI.ShowMessage("Rotating rooms consumes your energy. 2 for your room, 5 for adjacent");
                if (level >= 5)
                {
                    UI.ShowMessage("(Level 5)Alternate Exit blasts a hole into an adjacent room");
                    UI.ShowMessage("Alternate Exit: W,A,S,D + BACKSPACE  Costs 10 energy");
                }
            }
            oldState = newState;
            return oldState;
        }
        
        private void changeTurn(bool turn)
        {
            this.Turn = turn;
            turnTaken = false;
        }

        private void move(Floor floor, Vector2 direction)
        {
            Vector2 checkDir = Location - direction;
            if (floor[checkDir.X, checkDir.Y].moveRequest(this))
            {
                String animDirection;
                dictDirections.TryGetValue(direction, out animDirection);
                CurrentRoom.RemoveContent(this);
                distance = 300;
                Location = checkDir;
                //CurrentRoom = floor[Location.X, Location.Y];
                
                Animations.CurrentAnimation = animDirection;
                turnTaken = true;
            }
        }

        private void rotate(Floor floor, SpriteManager spriteManager, Vector2 direction)
        {
            Vector2 rotateDirection = Location - direction;
            floor[rotateDirection.X, rotateDirection.Y].rotate();
            SpriteManager.PlayCue("rotate1");
            rotated = true;
            if (direction == Vector2.Zero) { rotationEnergy -= 2; } else { rotationEnergy -= 5; }
            //turnTaken = true;
            rotation = 200;
        }

        bool SubtractEnergy(int amount)
        {
            if (rotationEnergy - amount < 0)
            {
                return false;
            }
            else
            {
                rotationEnergy -= amount;
                return true;
            }
        }

        //public void AddItem(Item item)
        //{
        //    if (inventory.Count < inventoryMax)
        //    {
        //        inventory.Add(item);
        //        String content = "You receive " + item.Name + "!";
        //        Window.SetContent(content);
        //        Window.SetPosition();
        //        Window.DrawWindow();
        //    }
        //}
        public bool AddItem(Item item)
        {
            if (inventory.Count < inventoryMax)
            {
                inventory.Add(item);
                String content = "You receive " + item.Name + "!";
                SpriteManager.PlayCue("pickup");
                UI.ShowMessage(content);
                return true;
            }
            else
            {
                return false;
            }
        }

        private Item checkInventory(Item item)
        {
            String check = item.Name;
            foreach (Item i in inventory)
            {
                if (i.Name == check)
                {
                    return i;
                }
            }
            return null;
        }
        public int UseItem(int index)
        {
            if (inventory.Count >= index) //Index in bounds
            {
                if (inventory[index].Type == "consumable") //item can be used
                {
                    
                    return inventory[index].Use(this);
                }
            }
            return -1;
        }
        public void ShowInventory()
        {
            string content = "";
            String nameUse = "";
            int length = 0;
            int lines = 0;
            int charCount = 20;
            int characters = 0;
            foreach (Item i in inventory)
            {
                nameUse = i.Name + i.Uses;
                characters = charCount - nameUse.Length;
                nameUse = i.Name;
                if (characters > 0)
                {
                    for (int x = 0; x < characters; x++)
                    {
                        nameUse += " ";
                    }
                }
                nameUse += i.Uses;
                content +=  nameUse + Environment.NewLine;
                length = charCount;
                lines++;
            }
            if (inventory.Count == 0)
            {
                content = "Empty";
                lines = 1;
                length = content.Length;
            }
            UI.ShowInventory(content, lines, length);
        }
        public void ShowStatus()
        {
            int nextLevel = level * 10;
            UI.ShowStatus(
                    "Level       " + Level + Environment.NewLine +
                    "Health      " + Health + "/" + HealthMax + Environment.NewLine +
                    "Strength        " + Strength + Environment.NewLine +
                    "Defense         " + Defense + Environment.NewLine +
                    "Speed           " + Speed + Environment.NewLine +
                    "Exp         " + exp + "/" + nextLevel + Environment.NewLine, 6, 18);
        }
        public void GainExp(int amount, Random rand)
        {
            exp += amount;
            if (exp >= level * 10)
            {
                UI.ShowMessage("Level up!");
                exp = exp % amount;
                level += 1;
                if (level == 5)
                {
                    UI.ShowMessage("Learned Skill: Alternate Route!");
                    UI.ShowMessage("Press H for controls");
                }
                Strength += rand.Next(1, 3);
                Defense += rand.Next(1, 3);
                Speed += rand.Next(1, 3);
                HealthMax += rand.Next(2, 5);
                rotationEnergyMax += rand.Next(2, 5);
                Health = HealthMax;
                rotationEnergy = rotationEnergyMax;
            }
        }
        public void AddBuff(Buff buff)
        {
            if (buff.Stat == "strength")
                Strength += buff.Effect;
            if (buff.Stat == "speed")
                Speed += buff.Effect;
            if (buff.Stat == "health")
                Health += buff.Effect;
            if (buff.Stat == "healthmax")
                HealthMax += buff.Effect;
            if (buff.Stat == "defense")
                Defense += buff.Effect;
            buffs.Add(buff);
        }
        public void RemoveBuff(Buff buff)
        {
            if (buff.Stat == "strength")
                Strength -= buff.Effect;
            if (buff.Stat == "speed")
                Speed -= buff.Effect;
            if (buff.Stat == "health")
                Health -= buff.Effect;
            if (buff.Stat == "healthmax")
                HealthMax -= buff.Effect;
            if (buff.Stat == "defense")
                Defense -= buff.Effect;
            buffs.Remove(buff);
        }
        public bool CheckBuffs(Buff buff)
        {
            foreach (Buff b in buffs)
            {
                if (b.Stat == buff.Stat)
                {
                    if (b.Effect >= buff.Effect)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
