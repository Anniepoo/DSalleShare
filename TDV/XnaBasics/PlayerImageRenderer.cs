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
    using System;

    /// <summary>
    /// This class renders the current color stream frame.
    /// </summary>
    public class PlayerImageRenderer : Object2D
    {
        const float FRACTION_TO_POINT = 0.75f;   // fractional distance down the rect we start reducing width
        /// <summary>
        /// The last frame of raw color data from the Kinect
        /// </summary>
        private byte[] colorData;

        /// <summary>
        /// a copy of last frame of raw color with transparent in all player=0 coordinates.
        /// This is same size as Kinect color frame
        /// </summary>
        private byte[] greenScreenMaskedColorData;

        /// <summary>
        /// The color frame as a texture.
        /// </summary>
        private Texture2D greenScreenColorTexture;

        /// <summary>
        /// The back buffer where color frame is scaled as requested by the Size.
        /// The cartoon materials are also rendered into this
        /// </summary>
        private RenderTarget2D backBuffer;

        /// <summary>
        /// The last frame of depth data
        /// </summary>
        public DepthImagePixel[] depthPixels = null;

        /// <summary>
        /// A map from a point in the depth frame 2D space to a point
        /// in the color frame 2D space for the last frame
        /// 
        /// </summary>
        ColorImagePoint[] colorCoordinates = null;

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
        public PlayerImageRenderer(Game game)
            : base(game)
        {
          //  this.cartoonElements = new CartoonRenderer(game, this.SkeletonToColorMapViewPortCorrection);
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
                    if (depthPixels == null || depthPixels.Length != depthFrame.PixelDataLength)
                    {
                        depthPixels = new DepthImagePixel[depthFrame.PixelDataLength];
                        this.colorCoordinates = new ColorImagePoint[this.Chooser.Sensor.DepthStream.FramePixelDataLength];
                    }
                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);
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
                    this.greenScreenMaskedColorData = new byte[frame.PixelDataLength];

                    this.greenScreenColorTexture = new Texture2D(
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
        }

        /// <summary>
        /// This method renders the color and skeleton frame.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        internal void Draw(SpriteBatch ssb, int playerID, Rectangle[] rects)
        {

            // If we don't have the effect load, load it
            // TODO this is dead now I think
            if (null == this.kinectColorVisualizer)
            {
                this.LoadContent();
            }

            // If we don't have a target, don't try to render
            if (null == this.greenScreenColorTexture)
            {
                return;
            }

            // Set the backbuffer and clear
            //      this.Game.GraphicsDevice.SetRenderTarget(this.backBuffer);
            //      this.Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            // try the simplest thing that can possibly work. We'll set the alpha transparent
            // Diana sez also have to set rgb values to zero

            if (depthPixels == null)
                return;

            int cdw = this.Chooser.Sensor.ColorStream.FrameWidth;
            int cdh = this.Chooser.Sensor.ColorStream.FrameHeight;
            int ddw = this.Chooser.Sensor.DepthStream.FrameWidth;

            // TODO this only needs happen once per frame
            this.Chooser.Sensor.CoordinateMapper.MapDepthFrameToColorFrame(
                this.Chooser.Sensor.DepthStream.Format,
                this.depthPixels,
                this.Chooser.Sensor.ColorStream.Format,
                this.colorCoordinates);

            Array.Clear(this.greenScreenMaskedColorData, 0, this.greenScreenMaskedColorData.Length);

            // loop over each row and column of the depth
            int depthHeight = this.Chooser.Sensor.DepthStream.FrameHeight;
            int depthWidth = this.Chooser.Sensor.DepthStream.FrameWidth;

            int colorToDepthDivisor = this.Chooser.Sensor.ColorStream.FrameWidth / depthWidth;

            for (int rectnum = 0; rectnum < rects.Length; rectnum++) // is it ok to modify incoming rect? what about repeated draw?
            {
                rects[rectnum].Width = (int)(rects[rectnum].Width * depthWidth / this.Chooser.Sensor.ColorStream.FrameWidth);
                rects[rectnum].Height = (int)(rects[rectnum].Height * depthHeight / this.Chooser.Sensor.ColorStream.FrameHeight);
                rects[rectnum].X = (int)(rects[rectnum].X * depthWidth / this.Chooser.Sensor.ColorStream.FrameWidth);
                rects[rectnum].Y = (int)(rects[rectnum].Y * depthHeight / this.Chooser.Sensor.ColorStream.FrameHeight);

                float pointyinset = 0.0f;

                for (int y = Math.Max(0, rects[rectnum].Y) ; y < Math.Min(rects[rectnum].Y + rects[rectnum].Height, depthHeight - 1) ; ++y)
                {
                    if ((y - rects[rectnum].Y) / (float)rects[rectnum].Height > FRACTION_TO_POINT)
                        pointyinset += rects[rectnum].Width / 2.0f /
                            ((float)rects[rectnum].Height * (1.0f - FRACTION_TO_POINT));

                    for (int x = Math.Max(0, rects[rectnum].X + (int)pointyinset) ; x < Math.Min(rects[rectnum].X + rects[rectnum].Width - (int)pointyinset, depthWidth - 1) ; ++x)
                    {
                        // calculate index into depth array
                        int depthIndex = x + (y * depthWidth);

                        DepthImagePixel depthPixel = this.depthPixels[depthIndex];

                        int player = depthPixel.PlayerIndex;

                        // if we're tracking a player for the current pixel, do green screen
                        if (player == playerID)
                        {
                            // retrieve the depth to color mapping for the current depth pixel
                            ColorImagePoint colorImagePoint = this.colorCoordinates[depthIndex];

                            if (KinectSensor.IsKnownPoint(colorImagePoint))
                            {
                                // 4 bytes/pixel
                                int cIndex = (colorImagePoint.Y * this.Chooser.Sensor.ColorStream.FrameWidth + colorImagePoint.X) * 4;

                                // for tighter cropping on left comment this first word out

                                /*
                                greenScreenMaskedColorData[cIndex] = colorData[cIndex]; // b
                                greenScreenMaskedColorData[cIndex + 1] = colorData[cIndex + 1];
                                greenScreenMaskedColorData[cIndex + 2] = colorData[cIndex + 2];
                                greenScreenMaskedColorData[cIndex + 3] = 0x60;
                                    * */

                                // doing the pixel to theright as well often looks better
                                greenScreenMaskedColorData[cIndex + 4] = colorData[cIndex + 6]; // flip b and r
                                greenScreenMaskedColorData[cIndex + 5] = colorData[cIndex + 5];
                                greenScreenMaskedColorData[cIndex + 6] = colorData[cIndex + 4];
                                greenScreenMaskedColorData[cIndex + 7] = 0xFF;
                            }

                        }  // player == playerID
                    }  // x
                } // y

                greenScreenColorTexture.SetData<byte>(this.greenScreenMaskedColorData);

                ssb.Draw(greenScreenColorTexture, this.Game.GraphicsDevice.Viewport.Bounds, Color.White);
            }
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
        internal Vector2 SkeletonToColorMapViewPortCorrection(SkeletonPoint point)
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

        internal Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != Chooser.Sensor) && (null != Chooser.Sensor.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = Chooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(point, Chooser.Sensor.ColorStream.Format);

                // AO - changed to scale positions by the amount we blow up the colorstream image when we render it
                // so the skeletons don't end up at 640 x 480
                Vector2 returnpt = new Vector2(colorPt.X, colorPt.Y);
              
                return returnpt;
            }

            return Vector2.Zero;
        }


        internal void addSubRenderers(PaintersAlgorithmRenderer paintersAlgorithmRenderer)
        {
            // No direct renderers here, the skeleton does it
        }
    }
}
