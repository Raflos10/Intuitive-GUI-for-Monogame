using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

using MonoGame.Extended.BitmapFonts;

using Intuitive_GUI_for_Monogame;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MenuSystem menuSystem;

        public const int VirtualWidth = 1920;
        public const int VirtualHeight = 1080;
        public float ResolutionScale { get; private set; }
        public Matrix ResolutionMatrix { get; private set; }

        public enum GameStates { active, paused }
        public GameStates GameState { get; set; } = GameStates.active;

        public Demo_Menu.Menu1 Menu1 { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        void ToggleFullscreen()
        {
            if (graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 450;
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                UpdateResolutionMatrix();
                menuSystem.ResolutionMatrix = ResolutionMatrix;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
                UpdateResolutionMatrix();
                menuSystem.ResolutionMatrix = ResolutionMatrix;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            UpdateResolutionMatrix();

            IsMouseVisible = true;

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

            menuSystem = new MenuSystem(this, spriteBatch);
            menuSystem.ResolutionMatrix = ResolutionMatrix;
            Components.Add(menuSystem);

            Menu1 = new Demo_Menu.Menu1(Content.Load<Texture2D>("test_menu"), Content.Load<Texture2D>("test_button"),
                Content.Load<Texture2D>("test_button_s"), Content.Load<Texture2D>("test_image_s"), Content.Load<BitmapFont>("Font/Calibri48"));
            menuSystem.OpenMenu(Menu1);
            Menu1.Position = new Vector2(VirtualWidth * .5f, VirtualHeight * .5f);
            //Menu1.Position = new Vector2((VirtualWidth - Menu1.Width) * .5f, 100);
            //font = Content.Load<BitmapFont>("Fonts/Calibri48");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            Input.Update();

            if (Input.IsGamePadBackTriggered() || Input.IsKeyTriggered(Keys.Escape))
                Exit();

            if (Input.IsKeyPressed(Keys.LeftAlt) && Input.IsKeyTriggered(Keys.Enter))
                ToggleFullscreen();

            if (Input.IsRightMouseButtonDown())
                Menu1.Position = Vector2.Transform(new Vector2(Input.MouseState.Position.X, Input.MouseState.Position.Y), Matrix.Invert(ResolutionMatrix));

            if (Input.IsKeyPressed(Keys.Q))
                Menu1.Rotation -= .03f;

            if (Input.IsKeyPressed(Keys.E))
                Menu1.Rotation += .03f;

            if (Input.IsKeyTriggered(Keys.Left))
                menuSystem.InputTrigger(Menu.MenuInputs.Left);

            if (Input.IsKeyTriggered(Keys.Right))
                menuSystem.InputTrigger(Menu.MenuInputs.Right);

            if (Input.IsKeyTriggered(Keys.Up))
                menuSystem.InputTrigger(Menu.MenuInputs.Up);

            if (Input.IsKeyTriggered(Keys.Down))
                menuSystem.InputTrigger(Menu.MenuInputs.Down);

            if (Input.IsKeyTriggered(Keys.Enter))
                menuSystem.InputTrigger(Menu.MenuInputs.OK);

            if (Input.IsKeyTriggered(Keys.P))
                if (Menu1.Item is Intuitive_GUI_for_Monogame.Items.Selectable selectable)
                {
                    selectable.PersistantHighlight = !selectable.PersistantHighlight;
                    Debug.WriteLine("Persistant Highlighting " + (selectable.PersistantHighlight ? "On" : "Off"));
                }

            base.Update(gameTime);
        }

        void UpdateResolutionMatrix()
        {
            ResolutionMatrix = Matrix.CreateScale(new Vector3((float)graphics.PreferredBackBufferWidth / VirtualWidth,
                (float)graphics.PreferredBackBufferHeight / VirtualHeight, 1));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
