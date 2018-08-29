using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Dank
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //public static float GlobalScale = 0.0f;
        public static bool PAUSED = false;
        public static bool BATTLE = false;
        public static bool GAMEOVER = false;
        public static bool WON = false;
        public static bool CLOSEGAME = false;
        public static bool SE = true;
        public static float Volume;
        public static float SEVolume;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteManager spriteManager;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1000;

            

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GAMEOVER)
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0)
                {
                    Exit();
                }
            }
            if (CLOSEGAME)
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        
    }

    static class ItemList
    {
        private static Dictionary<String, Item> itemList = new Dictionary<string, Item>();
        //public static Dictionary<String, Item> ItemList { get { return itemList; } set { itemList = value; } }

        public static void Add(String key, Item item)
        {
            itemList.Add(key, item);
        }

        public static Item Get(String name)
        {
            Item item;
            itemList.TryGetValue(name, out item);
            return new Item(item);
        }
    }
    static class UI
    {
        private static Window inventory = new Window();
        private static Window options = new Window();
        private static Window message = new Window();
        private static Window status = new Window();

        public static void ShowOptions(String music, string se, int lines, int length)
        {
            options.SetType("options");
            options.SetContent("Music          " + music + Environment.NewLine + "Sound Effects  " + se, lines, length);
            options.SetPosition();
            options.DrawWindow();
        }
        public static void ShowInventory(String content, int lines, int length)
        {
            inventory.SetType("inventory");
            inventory.SetContent(content, lines, length);
            inventory.SetPosition(new Point(5, 50));
            inventory.DrawWindow();
        }
        public static void ShowStatus(String content, int lines, int length)
        {
            inventory.SetType("status");
            inventory.SetContent(content, lines, length);
            inventory.SetPosition(new Point(400, 50));
            inventory.DrawWindow();
        }

        public static void ShowMessage(String content)
        {
            if (message.Type == "message")
            {
                message.AppendContent(content);
            }
            else
            {
                message.SetType("message");
                message.SetContent(content);
            }
            message.SetPosition();
            message.DrawWindow();
        }
        public static void ShowDialogue(String content)
        {
            message.SetType("dialogue");
            message.SetContent(content);
            message.SetPosition();
            message.DrawWindow();
        }
        public static void ShowDialogue(String content,int lines, int length)
        {
            message.SetType("dialogue");
            message.SetContent(content, lines, length);
            message.SetPosition(new Point(100,100));
            message.DrawWindow();
        }
        public static Window ShowMessage(String content, Point position, String type)
        {
            message.SetType(type);
            message.SetContent(content);
            message.SetPosition(position);
            message.DrawWindow();
            return message;
        }
        public static void Update(GameTime gameTime, UserControlledSprite player)
        {
            message.Update(gameTime, player);
            options.Update(gameTime, player);
            inventory.Update(gameTime, player);
            
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            message.Draw(gameTime, spriteBatch);
            options.Draw(gameTime, spriteBatch);
            inventory.Draw(gameTime, spriteBatch);
        }

        public static void SetupUI(Texture2D background, Texture2D border, Texture2D selection, SpriteFont font)
        {
            inventory.SetBg(background);
            inventory.SetFg(border);
            inventory.SetSelectionTexture(selection);
            inventory.SetFont(font);

            message.SetBg(background);
            message.SetFg(border);
            message.SetSelectionTexture(selection);
            message.SetFont(font);

            options.SetBg(background);
            options.SetFg(border);
            options.SetSelectionTexture(selection);
            options.SetFont(font);

        }
    }
}
