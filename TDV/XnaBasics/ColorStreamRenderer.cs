//------------------------------------------------------------------------------
// <copyright file="ColorStreamRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This class renders the current color stream frame.
    /// </summary>
    public class ColorStreamRenderer : Object2D
    {
        /// <summary>
        /// This child responsible for rendering the color stream's skeleton.
        /// </summary>
        private readonly CartoonRenderer cartoonRenderer;  // DS refactored from skeletonRenderer
        
        /// <summary>
        /// The last frame of color data.
        /// </summary>
        private byte[] colorData;

        /// <summary>
        /// The color frame as a texture.
        /// </summary>
        private Texture2D colorTexture;


        /// <summary>
        /// The back buffer where color frame is scaled as requested by the Size.
        /// </summary>
        private RenderTarget2D backBuffer;

        private short[] depthData = null;
        
        /// <summary>
        /// This Xna effect is used to swap the Red and Blue bytes of the color stream data.
        /// </summary>
        private Effect kinectColorVisualizer;

        /// <summary>
        /// Whether or not the back buffer needs updating.
        /// </summary>
        private bool needToRedrawBackBuffer = true;

        /// <summary>
        /// Initializes a new instance of the ColorStreamRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        public ColorStreamRenderer(Game game)
            : base(game)
        {
            this.cartoonRenderer = new CartoonRenderer(game, this.SkeletonToColorMap);
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

            // Grab the next depth buffer if it's available
            using (var depthFrame = this.Chooser.Sensor.DepthStream.OpenNextFrame(0))
            {
                if (depthFrame != null)
                {
                    if (depthData == null || depthData.Length != depthFrame.PixelDataLength)
                    {
                        depthData = new short[depthFrame.PixelDataLength];

                    }
                    depthFrame.CopyPixelDataTo(depthData);
                }
            }
            // AO - end of grabbing depth data

            using (var frame = this.Chooser.Sensor.ColorStream.OpenNextFrame(0))
            {
                // Sometimes we get a null frame back if no data is ready
                if (frame == null)
                {
                    return;
                }

                // Reallocate values if necessary
                if (this.colorData == null || this.colorData.Length != frame.PixelDataLength)
                {
                    this.colorData = new byte[frame.PixelDataLength];

                    this.colorTexture = new Texture2D(
                        this.Game.GraphicsDevice, 
                        frame.Width, 
                        frame.Height, 
                        false, 
                        SurfaceFormat.Color);

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

                frame.CopyPixelDataTo(this.colorData);
                this.needToRedrawBackBuffer = true;
            }

            // Update the skeleton renderer
            this.cartoonRenderer.Update(gameTime);

           
        }

        /// <summary>
        /// This method renders the color and skeleton frame.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If we don't have the effect load, load it
            if (null == this.kinectColorVisualizer)
            {
                this.LoadContent();
            }

            // If we don't have a target, don't try to render
            if (null == this.colorTexture)
            {
                return;
            }

            if (this.needToRedrawBackBuffer)
            {
                // Set the backbuffer and clear
                this.Game.GraphicsDevice.SetRenderTarget(this.backBuffer);
                this.Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

                // try the simplest thing that can possibly work. We'll set the alpha transparent
                // Diana sez also have to set rgb values to zero

                if (depthData != null)   // at startup we might get color frame first
                {
                    int cdw = this.Chooser.Sensor.ColorStream.FrameWidth;
                    int cdh = this.Chooser.Sensor.ColorStream.FrameHeight;
                    int ddw = this.Chooser.Sensor.DepthStream.FrameWidth;

                    float xColorToDepth = ddw /  (float)cdw;
                    float yColorToDepth = this.Chooser.Sensor.DepthStream.FrameHeight /
                        (float)cdh;
                    for (int cy = 0; cy < cdh; cy++)
                    {
                        for (int cx = 0; cx < cdw; cx++)
                        {
                            int dIndex = (int)(xColorToDepth * cx + yColorToDepth * cy * ddw);

                            int cIndex = 4 * (cdw * cy + cx);

                            

                            // player number is whatever. They don't necessarily start at 1
                            if ((depthData[dIndex] & 0x0007) == 0)
                            {
                                colorData[cIndex] = 0; // b
                                colorData[cIndex + 1] = 0;
                                colorData[cIndex + 2] = 0;
                                colorData[cIndex + 3] = 0;
                            }
                                /*
                            else
                            {
                                colorData[cIndex] = 0; // b
                                colorData[cIndex + 1] = 0xFF;
                                colorData[cIndex + 2] = 0;
                                colorData[cIndex + 3] = 0xFF;
                            }
                                 */
                        }
                    }
                }

                this.colorTexture.SetData<byte>(this.colorData);

                // Draw the color image
                // DS commented because the backfield will cover the entire color image
                // Annie reenables to make colors work
                this.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.kinectColorVisualizer);

                // Annie changed to expand the colorTexture, which is 640x480 and contains the color frame from the kinect,
                // out the entire viewport 

                // Annie sez we're going to change this to mask for the depth data
                
                
                this.SharedSpriteBatch.Draw(this.colorTexture, this.Game.GraphicsDevice.Viewport.Bounds , Color.White);
                this.SharedSpriteBatch.End();

                // Draw the skeleton
                this.cartoonRenderer.Draw(gameTime);

                // Reset the render target and prepare to draw scaled image
                this.Game.GraphicsDevice.SetRenderTargets(null);

                // No need to re-render the back buffer until we get new data
                this.needToRedrawBackBuffer = false; 
            }

            // Draw the scaled texture
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

            // This effect is necessary to remap the BGRX byte data we get
            // to the XNA color RGBA format.
            this.kinectColorVisualizer = Game.Content.Load<Effect>("KinectColorVisualizer");
        }

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        private Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != Chooser.Sensor) && (null != Chooser.Sensor.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = Chooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(point, Chooser.Sensor.ColorStream.Format);

                // AO - changed to scale positions by the amount we blow up the colorstream image when we render it
                // so the skeletons don't end up at 640 x 480
                Vector2 returnpt = new Vector2(colorPt.X, colorPt.Y);
                returnpt.X *= Game.GraphicsDevice.Viewport.Width / Chooser.Sensor.ColorStream.FrameWidth;
                returnpt.Y *= Game.GraphicsDevice.Viewport.Height / Chooser.Sensor.ColorStream.FrameHeight;
                return returnpt;
            }

            return Vector2.Zero;
        }
    }
}
