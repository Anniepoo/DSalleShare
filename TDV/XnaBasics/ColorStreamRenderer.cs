﻿//------------------------------------------------------------------------------
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
    public class ColorStreamRenderer : Object2D
    {
        /// <summary>
        /// This child responsible for rendering the drawn portions of the avatar.
        /// </summary>
        private readonly CartoonRenderer cartoonRenderer;  // DS refactored from skeletonRenderer
        
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
        DepthImagePixel[] depthPixels = null;

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
            if (null == this.greenScreenColorTexture)
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

                if (depthPixels != null)   // at startup we might get color frame first
                {
                    int cdw = this.Chooser.Sensor.ColorStream.FrameWidth;
                    int cdh = this.Chooser.Sensor.ColorStream.FrameHeight;
                    int ddw = this.Chooser.Sensor.DepthStream.FrameWidth;

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

                    for (int y = 0; y < depthHeight; ++y)
                    {
                        for (int x = 0; x < depthWidth; ++x)
                        {
                            // calculate index into depth array
                            int depthIndex = x + (y * depthWidth);

                            DepthImagePixel depthPixel = this.depthPixels[depthIndex];

                            int player = depthPixel.PlayerIndex;

                            // if we're tracking a player for the current pixel, do green screen
                            if (player != 0)
                            {
                                // retrieve the depth to color mapping for the current depth pixel
                                ColorImagePoint colorImagePoint = this.colorCoordinates[depthIndex];

                                if(KinectSensor.IsKnownPoint(colorImagePoint))
                                {
                                    // 4 bytes/pixel
                                    int cIndex = (colorImagePoint.Y * this.Chooser.Sensor.ColorStream.FrameWidth + colorImagePoint.X) * 4;

                                    greenScreenMaskedColorData[cIndex] = colorData[cIndex]; // b
                                    greenScreenMaskedColorData[cIndex + 1] = colorData[cIndex + 1];
                                    greenScreenMaskedColorData[cIndex + 2] = colorData[cIndex + 2];
                                    greenScreenMaskedColorData[cIndex + 3] = 0xFF;
                                }

                            }  // player > 0
                        }  // x
                    } // y
                }  // depthPixels != null

                this.greenScreenColorTexture.SetData<byte>(this.greenScreenMaskedColorData);

                // Draw the color image
                // DS commented because the backfield will cover the entire color image
                // Annie reenables to make colors work
                this.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.kinectColorVisualizer);

                // Annie changed to expand the greenScreenColorTexture, which is 640x480 and contains the color frame from the kinect,
                // out the entire viewport 

                // Annie sez we're going to change this to mask for the depth data
                
                
                this.SharedSpriteBatch.Draw(this.greenScreenColorTexture, this.Game.GraphicsDevice.Viewport.Bounds , Color.White);
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
