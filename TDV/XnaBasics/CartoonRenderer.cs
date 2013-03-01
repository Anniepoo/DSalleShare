//------------------------------------------------------------------------------
// <copyright file="CartoonRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// annie sez hi
namespace Microsoft.Samples.Kinect.XnaBasics
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A delegate method explaining how to map a SkeletonPoint from one space to another.
    /// </summary>
    /// <param name="point">The SkeletonPoint to map.</param>
    /// <returns>The Vector2 representing the target location.</returns>
    public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

    /// <summary>
    /// This class is responsible for rendering a skeleton stream.
    /// </summary>
    public class CartoonRenderer : Object2D
    {
        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap mapMethod;

        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        private static Skeleton[] skeletonData;


        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        private static bool skeletonDrawn = true;

        private Texture2D frontPlayfield;

        /// <summary>
        /// The origin (center) location of the joint texture.  howdy 
        /// </summary>
        private Vector2 jointOrigin;

        /// <summary>
        /// The joint texture.
        /// </summary>
        private Texture2D jointTexture;

        /// <summary>
        /// The origin (center) location of the bone texture.
        /// </summary>
        private Vector2 boneOrigin;

        /// <summary>
        /// The bone texture.
        /// </summary>
        private Texture2D boneTexture;

        private Vector2 pelvisOrigin;
        private Texture2D pelvisTexture;

        private Vector2 torsoOrigin;
        private Texture2D torsoTexture;

        private const float PIN_FROM_END = 0.1f;
        private const float FOCAL_PLANE_DIST = 1.5f;  // dist at which the scale is 1
        private const float NEAR_PLANE = 1.0f;  // minimum dist we assume so we don't balloon near kinect

        /// <summary>
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the CartoonRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="map">The method used to map the SkeletonPoint to the target space.</param>
        public CartoonRenderer(Game game, SkeletonPointMap map)
            : base(game)
        {
            this.mapMethod = map;
        }

        /// <summary>
        /// This method initializes necessary values.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);


            this.initialized = true;
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
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

            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            if (skeletonDrawn)
            {
                using (var skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    // Sometimes we get a null frame back if no data is ready
                    if (null == skeletonFrame)
                    {
                        return;
                    }

                    // Reallocate if necessary
                    if (null == skeletonData || skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    skeletonDrawn = false;
                }
            }
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            if (null == this.jointTexture)
            {
                this.LoadContent();
            }

            // If we don't have data, lets leave
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }

            this.SharedSpriteBatch.Begin();

            foreach (var skeleton in skeletonData)
            {
                switch (skeleton.TrackingState)
                {
                    case SkeletonTrackingState.Tracked:
                        // trivial change by annie
                        // Draw Bones
                        this.DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, this.torsoTexture);
                        this.DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter, this.pelvisTexture);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight, this.boneTexture);
                        /* left of screen and left arm of player, NOT stage left */
                        this.DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, this.boneTexture);

                        this.DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight, this.boneTexture);

                        this.DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, this.boneTexture);

                        this.DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, this.boneTexture);
                        this.DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, this.boneTexture);

                        this.DrawSkirtBone(skeleton.Joints);

                        // Now draw the joints

                        foreach (Joint j in skeleton.Joints)
                        {
                            
                            Color jointColor = Color.Green;
                            if (j.TrackingState != JointTrackingState.Tracked)
                            {
                                jointColor = Color.Yellow;
                            }

                            this.SharedSpriteBatch.Draw(
                                this.jointTexture,
                                this.mapMethod(j.Position),
                                null,
                                jointColor,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        }


                        break;
                    case SkeletonTrackingState.PositionOnly:
                        // If we are only tracking position, draw a blue dot
                        this.SharedSpriteBatch.Draw(
                                this.jointTexture,
                                this.mapMethod(skeleton.Position),
                                null,
                                Color.Blue,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        break;
                }
            }
            // this is STOOPID - why won't it draw right size?
            Rectangle r = new Rectangle(0, 0, (int)this.Size.X, (int)this.Size.Y);
            Rectangle s = new Rectangle(0, 0, this.frontPlayfield.Width, this.frontPlayfield.Height);
            this.SharedSpriteBatch.Draw(this.frontPlayfield, r, s, Color.White);

            this.SharedSpriteBatch.End();
            skeletonDrawn = true;

            base.Draw(gameTime);
        }

        private void DrawSkirtBone(JointCollection jointCollection)
        {
            if (jointCollection[JointType.HipCenter].TrackingState != JointTrackingState.Tracked)return;
            Vector2 start = this.mapMethod(jointCollection[JointType.HipCenter].Position);
            Vector2 end;

            if (jointCollection[JointType.KneeRight].TrackingState != JointTrackingState.Tracked &&
                jointCollection[JointType.KneeLeft].TrackingState != JointTrackingState.Tracked
                )
            {
                end = this.mapMethod(jointCollection[JointType.HipCenter].Position);
                end.Y += 150; // make the skirt directly below in worst case
            }
            else if (jointCollection[JointType.KneeRight].TrackingState != JointTrackingState.Tracked)
            {
                end = this.mapMethod(jointCollection[JointType.KneeLeft].Position);
                end.X = start.X;
            }
            else if (jointCollection[JointType.KneeLeft].TrackingState != JointTrackingState.Tracked)
            {
                end = this.mapMethod(jointCollection[JointType.KneeRight].Position);
                end.X = start.X;
            }
            else
            {
                end = this.mapMethod(jointCollection[JointType.KneeLeft].Position);
                end += this.mapMethod(jointCollection[JointType.KneeRight].Position);

                end.X /= 2;
                end.Y /= 2;
            }

            float depth = jointCollection[JointType.HipCenter].Position.Z;
            DrawBoneLike(depth, start, end, boneTexture);
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            frontPlayfield = Game.Content.Load<Texture2D>("FrontPlayField");

            this.jointTexture = Game.Content.Load<Texture2D>("Joint");
            this.jointOrigin = new Vector2(this.jointTexture.Width / 2, this.jointTexture.Height / 2);

            this.boneTexture = Game.Content.Load<Texture2D>("Bone");
            this.boneOrigin = new Vector2(8, 1200);

            this.torsoTexture = Game.Content.Load<Texture2D>("Torso");
            this.torsoOrigin = new Vector2(0.5f, 0.5f);

            this.pelvisTexture = Game.Content.Load<Texture2D>("Pelvis");
            this.pelvisOrigin = new Vector2(0, 0);
        }

        /// <summary>
        /// This method draws a bone.
        /// </summary>
        /// <param name="joints">The joint data.</param>
        /// <param name="startJoint">The starting joint.</param>
        /// <param name="endJoint">The ending joint.</param>
        /// <param name="boneTexture">The texture to use for the joint</param>
        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint, Texture2D boneTexture)
        {
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                    joints[endJoint].TrackingState != JointTrackingState.Tracked)
                return;

            float depth = joints[startJoint].Position.Z;
            Vector2 start = this.mapMethod(joints[startJoint].Position);

            Vector2 end = this.mapMethod(joints[endJoint].Position);
            DrawBoneLike(depth, start, end, boneTexture);
        }

        private void DrawBoneLike(float depth, Vector2 start, Vector2 end, Texture2D boneTexture)
        {
                Vector2 diff = end - start;
            Vector2 scale = new Vector2((float)(FOCAL_PLANE_DIST / Math.Max(NEAR_PLANE, depth)), 
                diff.Length() / boneTexture.Height * (1.0f / (1.0f - 2 * PIN_FROM_END)));

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Vector2 center = new Vector2((int)(start.X), (int)(start.Y));

            // Scale the dest, not the source!
            Rectangle sourceRect = new Rectangle(0, 0, boneTexture.Width, boneTexture.Height);
            Rectangle destRect = new Rectangle(0, 0, 0, 0);

            destRect.Offset((int)(start.X), (int)(start.Y));

            // just for development
            Color color = Color.White;
      
       
            Vector2 destctr = new Vector2(destRect.X, destRect.Y);
            this.SharedSpriteBatch.Draw(boneTexture, destctr, null, color, angle,
                new Vector2(boneTexture.Width/2 , boneTexture.Height * PIN_FROM_END),
               scale, SpriteEffects.None, 1.0f);
        }
    }
}
