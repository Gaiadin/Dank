using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dank
{
    class Window
    {
        Rectangle wPosition;
        String wContent = "";
        Texture2D wBg;
        SpriteFont wFont;
        bool drawWindow = false;
        Rectangle fgLeft;
        Rectangle fgRight;
        Rectangle fgUp;
        Rectangle fgDown;
        KeyboardState oldState;
        Texture2D wFg;
        int borderWidth=5;
        int wLength;
        int wLines;
         int adjHeight;
         string wType;
         Rectangle selection;
         int wIndex;
         Texture2D wSelectionTexture;
         int volumePer = 0;
         int volumeMax = 30;
         float oldVolume = 0f;
         float oldSEVolume = 0f;
         int contentIndex = 0;
         int messageSpeedMax = 50;
         int messageSpeed = 0;
         String partContent = "";
         bool unpause = false;
         int pauseTime = 0;
         bool speedText = false;

         public String Type { get { return wType; } }
        //public Window(Texture2D bg, SpriteFont font, Rectangle position, String content)
        //{
        //    wPosition = position;
        //    wContent = content;
        //    wBg = bg;
        //    wFont = font;
        //}
        //public Window(Texture2D bg, SpriteFont font, Point position, String content)
        //{
        //    wPosition.X = position.X;
        //    wPosition.Y = position.Y;
        //    wPosition.Width = content.Length * 3;
        //    wPosition.Height = 15;
        //    wContent = content;
        //    wBg = bg;
        //    wFont = font;
        //}
        //public Window(Texture2D bg, SpriteFont font)
        //{
        //    wBg = bg;
        //    wFont = font;
        //}

        public  void SetContent(String content)
        {
            wLines = 1;
            wContent = content;
            adjHeight = 30;
            wLength = content.Length * 12;
        }
        public void AppendContent(String content)
        {
            wLines++;
            wContent += Environment.NewLine + content;
            adjHeight = 26 * wLines;
            wLength = wContent.Length * 15 / wLines;
        }

        public  void SetContent(String content, int lines, int length)
        {
            wLines = lines;
            wContent = content;
            adjHeight = wLines * 26;
            wLength = length * 12;
        }

        public  void SetType(String type)
        {
            wType = type;
        }

        public  void SetPosition(Rectangle position)
        {
            wPosition = position;
            buildFgRects();
        }

        public  void SetPosition(Point position)
        {
            wPosition.X = position.X;
            wPosition.Y = position.Y;
            wPosition.Width = wLength + borderWidth;
            wPosition.Height = adjHeight;
            buildFgRects();
        }
        public  void SetPosition()
        {
            wPosition.Width = wLength +borderWidth;
            wPosition.Height = adjHeight;
            wPosition.X = 515 - wPosition.Width / 2;
            wPosition.Y = 410;
            buildFgRects();
        }
        public  void SetBg(Texture2D bg)
        {
            wBg = bg;
        }
        public  void SetFg(Texture2D fg)
        {
            wFg = fg;
        }
        public  void SetFont(SpriteFont font)
        {
            wFont = font;
        }

        public  void Update(GameTime gameTime, UserControlledSprite player)
        {
            if (wContent != "")
            {
                Game1.PAUSED = true;
                unpause = false;
            }
            KeyboardState newState = Keyboard.GetState();
            volumePer += gameTime.ElapsedGameTime.Milliseconds;

            if (unpause)
            {
                pauseTime += gameTime.ElapsedGameTime.Milliseconds;
                if (pauseTime >= 200)
                {
                    Game1.PAUSED = false;
                    unpause = false;
                    pauseTime = 0;
                }
            }
            
                
          
            if (wType == "inventory")
            {
                Window description = new Window();
                if(wContent != "Empty")
                    description = UI.ShowMessage(player.Inventory[wIndex].Description + "  Worth: " + player.Inventory[wIndex].Value.ToString(), new Point(this.wPosition.X + this.wPosition.Width + borderWidth*2 + 5, this.wPosition.Y), "description");
                
                setSelection();
                if (oldState.IsKeyDown(Keys.Up) && !newState.IsKeyDown(Keys.Up))
                {
                    if (wIndex != 0)
                    {
                        wIndex--;
                    }
                }
                if (oldState.IsKeyDown(Keys.Down) && !newState.IsKeyDown(Keys.Down))
                {
                    if (wIndex < wLines-1)
                    {
                        wIndex++;
                    }
                }
                if (oldState.IsKeyDown(Keys.Enter) && !newState.IsKeyDown(Keys.Enter))
                {
                    //Use Item
                    if (wContent != "Empty")
                    {
                        if (player.UseItem(wIndex) <= 0 && wIndex != 0)
                            wIndex--;
                        player.ShowInventory();
                        if (wContent == "Empty")
                            description.closeWindow();
                    }
                    
                }
                if (oldState.IsKeyDown(Keys.I) && !newState.IsKeyDown(Keys.I))
                {
                    closeWindow();
                    description.closeWindow();
                }
                if (oldState.IsKeyDown(Keys.Escape) && !newState.IsKeyDown(Keys.Escape))
                {
                    closeWindow();
                    description.closeWindow();
                }
            }
            else if (wType == "options")
            {
                setSelection();
                if (oldState.IsKeyDown(Keys.Up) && !newState.IsKeyDown(Keys.Up))
                {
                    if (wIndex != 0)
                    {
                        wIndex--;
                    }
                }
                if (oldState.IsKeyDown(Keys.Down) && !newState.IsKeyDown(Keys.Down))
                {
                    if (wIndex < wLines - 1)
                    {
                        wIndex++;
                    }
                }
                if (wIndex == 0) //Music
                {
                    if (newState.IsKeyDown(Keys.Left))
                    {
                        if (Game1.Volume > 0.0f || oldVolume > 0.0f)
                        {
                            
                            if (volumePer >= volumeMax)
                            {
                                if (oldVolume > 0)
                                {
                                    Game1.Volume = oldVolume;
                                    oldVolume = 0;
                                }
                                if (Game1.Volume > 0.0f)
                                    Game1.Volume -= .01f;
                                volumePer = 0;
                                SpriteManager.ShowOptions();
                            }
                        }
                        
                    }
                    if (newState.IsKeyDown(Keys.Right))
                    {
                        if (Game1.Volume < 1.0f)
                        {
                            //volumePer += gameTime.ElapsedGameTime.Milliseconds;
                            if (volumePer >= volumeMax)
                            {
                                if (oldVolume > 0)
                                {
                                    Game1.Volume = oldVolume;
                                    oldVolume = 0;
                                }
                                if (Game1.Volume < 1.0f)
                                    Game1.Volume += .01f;
                                volumePer = 0;
                                SpriteManager.ShowOptions();
                            }
                        }
                    }
                    if (oldState.IsKeyDown(Keys.Enter) && !newState.IsKeyDown(Keys.Enter))
                    {
                        if (oldVolume > 0)
                        {
                            Game1.Volume = oldVolume;
                            oldVolume = 0f;
                            SpriteManager.ShowOptions();
                        }
                        else
                        {
                            oldVolume = Game1.Volume;
                            Game1.Volume = 0.0f;
                            SpriteManager.ShowOptions();
                        }
                    }
                }
                else if (wIndex == 1)
                {
                    if (newState.IsKeyDown(Keys.Left))
                    {
                        if (Game1.SEVolume > 0.0f || oldSEVolume > 0.0f)
                        {
                            if (volumePer >= volumeMax)
                            {
                                if (oldSEVolume > 0)
                                {
                                    Game1.SEVolume = oldSEVolume;
                                    Game1.SE = true;
                                    oldSEVolume = 0;
                                }
                                if (Game1.SEVolume > 0.0f)
                                    Game1.SEVolume -= .01f;
                                volumePer = 0;
                                SpriteManager.SetVolumeSE();
                                SpriteManager.ShowOptions();
                            }
                        }
                        
                    }
                    if (newState.IsKeyDown(Keys.Right))
                    {
                        if (Game1.SEVolume < 1.0f)
                        {
                            //volumePer += gameTime.ElapsedGameTime.Milliseconds;
                            if (volumePer >= volumeMax)
                            {
                                if (oldSEVolume > 0)
                                {
                                    Game1.SEVolume = oldSEVolume;
                                    Game1.SE = true;
                                    oldSEVolume = 0;
                                }
                                if (Game1.SEVolume < 1.0f)
                                    Game1.SEVolume += .01f;
                                volumePer = 0;
                                SpriteManager.SetVolumeSE();
                                SpriteManager.ShowOptions();
                            }
                        }
                    }
                    if (oldState.IsKeyDown(Keys.Enter) && !newState.IsKeyDown(Keys.Enter))
                    {
                        if (oldSEVolume > 0)
                        {
                            Game1.SEVolume = oldSEVolume;
                            Game1.SE = !Game1.SE;
                            SpriteManager.ShowOptions();
                        }
                        else
                        {
                            oldSEVolume = Game1.SEVolume;
                            Game1.SE = !Game1.SE;
                            SpriteManager.ShowOptions();
                        }
                    }
                }
                if (oldState.IsKeyDown(Keys.Escape) && !newState.IsKeyDown(Keys.Escape))
                {
                    closeWindow();
                }
            }
            if(wType == "message")
            {
                if (oldState.GetPressedKeys().Length > 0 && newState.GetPressedKeys().Length <= 0)
                {
                    closeWindow();
                }
            }
            if (wType == "status")
            {
                if (oldState.GetPressedKeys().Length > 0 && newState.GetPressedKeys().Length <= 0)
                {
                    closeWindow();
                }
                
            }
            if (wType == "shop")
            {
                setSelection();
                if (oldState.IsKeyDown(Keys.Up) && !newState.IsKeyDown(Keys.Up))
                {
                    if (wIndex != 0)
                    {
                        wIndex--;
                    }
                }
                if (oldState.IsKeyDown(Keys.Down) && !newState.IsKeyDown(Keys.Down))
                {
                    if (wIndex < wLines - 1)
                    {
                        wIndex++;
                    }
                }
                if (oldState.IsKeyDown(Keys.Enter) && !newState.IsKeyDown(Keys.Enter))
                {
                    if (wIndex == 0) //Buy
                    {
                    }
                    if (wIndex == 1) //Sell
                    {
                    }
                }
            }
            if (wType == "dialogue")
            {
                if (oldState.IsKeyDown(Keys.Escape) && !newState.IsKeyDown(Keys.Escape) && !Game1.WON)
                    closeWindow();
                if (newState.IsKeyDown(Keys.Enter) && contentIndex < wContent.Length)
                {
                    messageSpeedMax = 15;
                    speedText = true;
                }
                if (!oldState.IsKeyUp(Keys.Enter) && newState.IsKeyUp(Keys.Enter) && contentIndex >= wContent.Length && !speedText)
                {
                    if (Game1.WON)
                        Game1.CLOSEGAME = true;
                    else
                        closeWindow();
                }
                if (!newState.IsKeyDown(Keys.Enter))
                {
                    messageSpeedMax = 50;
                    speedText = false;
                }
                
                
            }
            oldState = newState;
        }

        private  void closeWindow()
        {
            drawWindow = false;
            
            wType = "";
            wIndex = 0;
            wContent = "";
            selection = new Rectangle(0, 0, 0, 0);
            wPosition = new Rectangle(0, 0, 0, 0);
            adjHeight = 0;
            wLength = 0;
            wLines = 0;
            partContent = "";
            contentIndex = 0;

            unpause = true;
        }
        public  void DrawWindow()
        {
            drawWindow = true;
        }
        public  void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            messageSpeed += gameTime.ElapsedGameTime.Milliseconds;
            if (messageSpeed >= messageSpeedMax && wType == "dialogue")
            {
                messageSpeed = 0;
                
                if (contentIndex <= wContent.Length)
                {
                    partContent = wContent.Substring(0, contentIndex);
                    contentIndex++;
                }
            }
            if (drawWindow)
            {
                Game1.PAUSED = true;
                //Console.WriteLine("drawing window at " + wPosition + " Content: " + wContent);
                spriteBatch.Draw(wBg, wPosition, Color.White * .75f);
                spriteBatch.Draw(wFg, fgUp, Color.White * .75f);
                spriteBatch.Draw(wFg, fgLeft, Color.White * .75f);
                spriteBatch.Draw(wFg, fgRight, Color.White * .75f);
                spriteBatch.Draw(wFg, fgDown, Color.White * .75f);
                if (wType == "inventory" || wType == "options")
                    spriteBatch.Draw(wSelectionTexture, selection, Color.White * .4f);
                spriteBatch.Draw(wFg, fgDown, Color.White * .75f);
                if (wType == "dialogue")
                {
                    spriteBatch.DrawString(wFont, partContent, new Vector2(wPosition.X + borderWidth , wPosition.Y), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(wFont, wContent, new Vector2(wPosition.X + borderWidth, wPosition.Y), Color.White);
                }
                
            }
        }

        private  void setSelection()
        {
            selection.X = wPosition.X ;
            selection.Y = wPosition.Y + wIndex * 26;
            selection.Width = wPosition.Width;
            selection.Height = 26;
        }
        public  void SetSelectionTexture(Texture2D selectionTexture)
        {
            wSelectionTexture = selectionTexture;
        }
        private  void buildFgRects()
        {
            fgUp.X = wPosition.X - borderWidth;
            fgUp.Y = wPosition.Y - borderWidth;
            fgUp.Width = wPosition.Width + borderWidth *2;
            fgUp.Height = borderWidth;

            fgLeft.X = wPosition.X - borderWidth;
            fgLeft.Y = wPosition.Y - borderWidth;
            fgLeft.Width = borderWidth;
            fgLeft.Height = wPosition.Height + borderWidth *2; ;

            fgRight.X = wPosition.X + wPosition.Width;
            fgRight.Y = wPosition.Y - borderWidth;
            fgRight.Width = borderWidth;
            fgRight.Height = wPosition.Height + borderWidth *2;

            fgDown.X = wPosition.X - borderWidth;
            fgDown.Y = wPosition.Y + wPosition.Height;
            fgDown.Width = wPosition.Width + borderWidth*2;
            fgDown.Height = borderWidth;
        }

    }
}
