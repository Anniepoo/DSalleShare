//------------------------------------------------------------------------------
// <copyright file="XnaBasicsGame.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// The main Xna game implementation.
    /// </summary>
    public class TDVBasicGame : Microsoft.Xna.Framework.Game
    {
        private static TDVBasicGame defaultGame = null;

        public static TDVBasicGame Default
        {
            get
            {
                return defaultGame; // TODO CODE DEBT bad singleton implementation
            }
        }

        private const int MAX_PLAYERS = 6;

        // the player to whom keyboard input is directed
        private int current_player = 0;

        /// <summary>
        /// This is used to adjust the window size.
        /// 
        /// 
        /// </summary>
        public const int Width = 1280;
        public const int Height = 960;

        /// <summary>
        /// This controls the transition time for the resize animation.
        /// </summary>
        private const double TransitionDuration = 1.0;

        /// <summary>
        /// The graphics device manager provided by Xna.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;
        
        /// <summary>
        /// This control selects a sensor, and displays a notice if one is
        /// not connected.
        /// </summary>
        private readonly KinectChooser chooser;

        private readonly PaintersAlgorithmRenderer paintersAlgorithmRenderer;

        private TDVPlayer[] players = new TDVPlayer[MAX_PLAYERS];

        internal TDVPlayer getPlayer(int i)
        {
            if (i < 0 || i >= MAX_PLAYERS)
                throw new IndexOutOfRangeException("Illegal player number");

            return players[i];
        }

        /// <summary>
        /// This is the viewport of the streams.
        /// </summary>
        private readonly Rectangle viewPortRectangle;

        /// <summary>
        /// This is the SpriteBatch used for rendering the header/footer.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// This tracks the previous keyboard state.
        /// </summary>
        private KeyboardState previousKeyboard;

        /// <summary>
        /// This is the font for the footer.
        /// </summary>
        private SpriteFont font;


        /// <summary>
        /// Initializes a new instance of the TDVBasicGame class.
        /// </summary>
        public TDVBasicGame()
        {
            defaultGame = this;

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "Happy Birthday";

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = Width;
            this.graphics.PreferredBackBufferHeight = Height;
            this.graphics.PreparingDeviceSettings += this.GraphicsDevicePreparingDeviceSettings;
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.viewPortRectangle = new Rectangle(0,0, Width, Height);

            Content.RootDirectory = "Content";

            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new TDVPlayer();
            }

            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);

            this.paintersAlgorithmRenderer = new PaintersAlgorithmRenderer(this);

            this.Components.Add(this.chooser);

            this.previousKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Loads the Xna related content.
        /// </summary>
        protected override void LoadContent()
        {
          
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);

            this.font = Content.Load<SpriteFont>("Segoe16");

            base.LoadContent();
        }

        /// <summary>
        /// Initializes class and components
        /// </summary>
        protected override void Initialize()
        {
            this.Components.Add(this.paintersAlgorithmRenderer);
            
            base.Initialize();
        } 

        /// <summary>
        /// This method updates the game state. Including monitoring
        /// keyboard state and the transitions.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the spacebar has been pressed, toggle the focus
            KeyboardState newState = Keyboard.GetState();
            if (this.previousKeyboard.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left) && 
                this.paintersAlgorithmRenderer.Cartooner.isAnySkeletonShowing())
            {
                do
                {
                    current_player--;
                    if (current_player < 0) current_player = MAX_PLAYERS - 1;
                } while (!this.paintersAlgorithmRenderer.Cartooner.isSkeletonShowing(current_player));

            }
            if (this.previousKeyboard.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right) &&
                this.paintersAlgorithmRenderer.Cartooner.isAnySkeletonShowing())
            {
                do
                {
                    current_player++;
                    if (current_player >= MAX_PLAYERS) current_player = 0;
                } while (!this.paintersAlgorithmRenderer.Cartooner.isSkeletonShowing(current_player));
            }
            if (this.previousKeyboard.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
            {
                players[current_player].Appearance++;
            }
            if (this.previousKeyboard.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
            {
                players[current_player].Appearance--;
            }
            /////////////////////////////////////////DS


            if (newState.IsKeyDown(Keys.Space))
            {

                Texture2D screenGrab = new Texture2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                int[] backBuffer = new int[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];
                graphics.GraphicsDevice.GetBackBufferData(backBuffer);

                screenGrab.SetData(backBuffer);
            }
            /////////////////////////////////////////////DS
            this.previousKeyboard = newState;

            // Animate the stream positions and sizes
            this.paintersAlgorithmRenderer.Position = new Vector2(0, 0);
            this.paintersAlgorithmRenderer.Size = new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height);


        }

        /// <summary>
        /// This method renders the current state.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.White);

            // Render header/footer
            // at the moment no indicia stuff on screen
         //   this.spriteBatch.Begin();
        //    this.spriteBatch.DrawString(this.font, "Press [Space] to switch between color and depth.", new Vector2(10, this.viewPortRectangle.Y + this.viewPortRectangle.Height + 3), Color.Black);
        //    this.spriteBatch.End();

            // Render the streams with respect to focus
            this.paintersAlgorithmRenderer.DrawOrder = 1;

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the CartoonRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        public int PlayerID { 
            get { return current_player; }
            set { current_player = value;  } 
        }
    }
}
