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
        private readonly CartoonRenderer cartoonRenderer;  // DS refactored from skeletonRenderer

        private readonly PlayerImageRenderer playerImageRenderer;

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
            // this.playerImageRenderer = new PlayerImageRenderer(game);
            // this.cartoonRenderer = new CartoonRenderer(game, this.SkeletonToColorMap);
        }

        /// <summary>
        /// Initializes the necessary children.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
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
            this.cartoonRenderer.Update(gameTime);
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

            this.cartoonRenderer.addSubRenderers(this);
            this.playerImageRenderer.addSubRenderers(this);

            this.subRenderers.Sort(ZOrderIComparer.defaultComparer());

            foreach (SubRenderer s in this.subRenderers)
            {
                s.Draw(gameTime);
            }


            // Draw the back buffer to the actual game
            this.SharedSpriteBatch.Begin();
            this.SharedSpriteBatch.Draw(
                this.backBuffer,
                new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
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
            base.LoadContent();
        }
    }
}
