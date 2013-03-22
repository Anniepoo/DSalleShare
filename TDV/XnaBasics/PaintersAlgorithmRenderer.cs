using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class PaintersAlgorithmRenderer : Object2D
    {
        /// <summary>
        /// This child responsible for rendering the drawn portions of the avatar.
        /// </summary>
        private readonly CartoonElements cartoonElements;  // DS refactored from skeletonRenderer

        private readonly PlayerImageRenderer playerImageRenderer;


        private static Random rand = new Random();

        /// <summary>
        /// The back buffer where color frame is scaled as requested by the Size.
        /// The cartoon materials are also rendered into this
        /// </summary>
        private RenderTarget2D backBuffer;

        /// <summary>
        /// List of subRenderers for this frame
        /// contents recreated each frame
        /// </summary>
        private List<SubRenderer> subRenderers = new List<SubRenderer>();


        /// <summary>
        /// Initializes a new instance of the PaintersAlgorithmRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        public PaintersAlgorithmRenderer(Game game)
            : base(game)
        {
            this.playerImageRenderer = new PlayerImageRenderer(game);
            this.cartoonElements = new CartoonElements(game, this.playerImageRenderer.SkeletonToColorMapViewPortCorrection, this.playerImageRenderer);
        }

        /// <summary>
        /// Initializes the necessary children.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);

            this.backBuffer = new RenderTarget2D(
                        this.Game.GraphicsDevice,
                        // was frame.Width & frame.Height Annie Fix to stop drawing cartoons 640x480 3/6/13 11:12am
                        this.Game.GraphicsDevice.Viewport.Width, 
                        this.Game.GraphicsDevice.Viewport.Height,
                        false, 
                        SurfaceFormat.Color, 
                        DepthFormat.None,
                        this.Game.GraphicsDevice.PresentationParameters.MultiSampleCount, 
                        RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        /// The update method where the new color frame is retrieved.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the sensor is not found, not running, or not connected, stop now
            if (null == this.Chooser.Sensor ||
                false == this.Chooser.Sensor.IsRunning ||
                KinectStatus.Connected != this.Chooser.Sensor.Status)
            {
                return;
            }

            this.playerImageRenderer.Update(gameTime);
            this.cartoonElements.Update(gameTime);
        }

        /// <summary>
        /// This method renders the color and skeleton frame.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            this.Game.GraphicsDevice.SetRenderTarget(this.backBuffer);
            this.Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.subRenderers.Clear();

            this.cartoonElements.addSubRenderers(this);
          //  this.playerImageRenderer.addSubRenderers(this); // his only adds non skeleton stuff - null for the moment

            this.subRenderers.Sort(ZOrderIComparer.defaultComparer());

            this.SharedSpriteBatch.Begin();

            foreach (SubRenderer s in this.subRenderers)
            {
                s.Draw(SharedSpriteBatch);
                
            }

            // next line is temporary until the skeleton based image stuff works
           // this.playerImageRenderer.Draw(gameTime);

            this.SharedSpriteBatch.End();
            this.Game.GraphicsDevice.SetRenderTarget(null);

            // Draw the back buffer to the actual game
            this.SharedSpriteBatch.Begin();
            this.SharedSpriteBatch.Draw(
                this.backBuffer,
                new Rectangle((int)Position.X, (int)Position.Y + DirtAndScratchesSubrenderer.Shake, (int)Size.X, (int)Size.Y),
                null,
                Color.White);

            this.SharedSpriteBatch.End();
          
            base.Draw(gameTime);

        }

        /// <summary>
        /// This method loads the Xna effect.
        /// </summary>
        protected override void LoadContent()
        {
            TDVGUISubRenderer.causeLoadContent(Game);
            base.LoadContent();
        }

        internal void addSubRenderer(SubRenderer sr)
        {
            this.subRenderers.Add(sr);
        }
    }
}
