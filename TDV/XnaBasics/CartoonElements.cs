//------------------------------------------------------------------------------
// <copyright file="CartoonRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
    public class CartoonElements : Object2D
    {
        // Diana - values you gave me are too high by factor of two,
        // roughly. In my space I vary from 1.03 to 2.3
        private const float MIDFIELD_Z = 2.3f;
        private const float FRONT_MIDFIELD_Z =1.75f;
        private const float FRONT_FIELD_Z = 01.5f;
        private const float BACK_FIELD_Z = 4.0f;    // leave this one at 4.0, that's the actual farthest distance
        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap mapMethod;

        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        private static Skeleton[] skeletonData;

        private PlayerImageRenderer playerImageRenderer;

        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        private static bool skeletonDrawn = true;
 
        private const float PIN_FROM_END = 0.1f;
        private const float FOCAL_PLANE_DIST = 1.5f;  // dist at which the scale is 1
        private const float NEAR_PLANE = 1.0f;  // minimum dist we assume so we don't balloon near kinect

        /// <summary>
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;
        // TODO terrible encapsulation!  FIX ME ANNIE!!!!!
        private TextureProvider tp;

        public TextureProvider TP
        {
            get { return tp; }
        }

        /*
        internal Texture2D boneTexture;
        internal Texture2D pelvisTexture;
        internal Texture2D torsoTexture;
        internal Texture2D waistTexture;
        internal Texture2D footLeftTexture;
        internal Texture2D footRightTexture;
        internal Texture2D leftHandTexture;
        internal Texture2D rightHandTexture;
        internal Texture2D headTexture;
        internal Texture2D shoulderRightTexture;
        internal Texture2D shoulderLeftTexture;
        internal Texture2D upperArmRightTexture;
        internal Texture2D lowerArmRightTexture;
        internal Texture2D upperArmLeftTexture;
        internal Texture2D lowerArmLeftTexture;
        internal Texture2D upperLegRightTexture;
        internal Texture2D lowerLegRightTexture;
        internal Texture2D upperLegLeftTexture;
        internal Texture2D lowerLegLeftTexture;
        public Texture2D hairDoTexture;
        public Texture2D hairBottomTexture;
        */

        private Texture2D frontField;
        private Texture2D frontMidField;
        private Texture2D midField;
        private Texture2D backField;
        /// <summary>
        /// Initializes a new instance of the CartoonRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="map">The method used to map the SkeletonPoint to the target space.</param>
        public CartoonElements(Game game, SkeletonPointMap map, PlayerImageRenderer pir)
            : base(game)
        {
            this.mapMethod = map;
            this.playerImageRenderer = pir;
            this.tp = new TextureProvider(game, "f1");
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
            /*  Moved to addSubRenderers
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
            */

            // not making more spritebatches this.SharedSpriteBatch.Begin();
            /* moved to BillboardSubrenderer
            this.SharedSpriteBatch.Draw(midField, new Vector2(0, 0), Color.White);
            this.SharedSpriteBatch.Draw(frontMidField, new Vector2(0, 0), Color.White);
            this.SharedSpriteBatch.Draw(frontField, new Vector2(0, 0), Color.White);
             */
            // ================  DONE addSubRenderers TO HERE ===============
            /* moved to SkeletonSubrenderer
            foreach (var skeleton in skeletonData)
            { 
                switch (skeleton.TrackingState)
                {
                      
                    case SkeletonTrackingState.Tracked:
                       
                        // Draw Bones using painters algorithm
                 

                        this.DrawCharacterPart(skeleton.Joints, JointType.Spine, JointType.HipCenter, this.waistTexture);  // Waist
                        // DS commented out drawing the small pelvis joints
                //        this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, this.partTexture);
                  //      this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipRight, this.partTexture);
                        // left of screen and left arm of player, NOT stage left 
                        this.DrawCharacterPart(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, this.upperArmRightTexture);  // Upper Arm Right 
                        this.DrawCharacterPart(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, this.upperArmLeftTexture);   // Upper Arm Left 
                        this.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, this.shoulderRightTexture); // Shoulder Right
                        this.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, this.shoulderLeftTexture); // Shoulder Left
                        this.DrawCharacterPart(skeleton.Joints, JointType.Head, JointType.ShoulderCenter, this.headTexture); // Head  
                        this.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, this.torsoTexture); // Torso
                        this.DrawCharacterPart(skeleton.Joints, JointType.WristRight, JointType.HandRight, this.rightHandTexture); // Right hand 
                        this.DrawCharacterPart(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, this.lowerArmRightTexture); // Lower Arm Right
                        this.DrawCharacterPart(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, this.partTexture); // Left hand
                        this.DrawCharacterPart(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, this.lowerArmLeftTexture); // Lower Arm Left
                        this.DrawCharacterPart(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, this.upperLegRightTexture); // Upper Leg Right       
                        this.DrawCharacterPart(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, this.lowerLegRightTexture); //Lower Leg Right
                        this.DrawCharacterPart(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, this.footRightTexture);    // Foot Right    
                        this.DrawCharacterPart(skeleton.Joints, JointType.HipRight, JointType.KneeRight, this.upperLegLeftTexture); // Upper Leg Left
                        this.DrawCharacterPart(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, this.lowerLegLeftTexture); // Lower Leg Left
                        this.DrawCharacterPart(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, this.partTexture); // Foot Left
                        this.DrawSkirtBone(skeleton.Joints); // Skirt
*/


/* DS COMMENETD OUT JOINT DRAWING
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

*/
            /* functionality removed
                        break;
                    case SkeletonTrackingState.PositionOnly:
                       //  If we are only tracking position, draw a blue dot
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
             */
            // this is STOOPID - why won't it draw right size?
            //Rectangle r = new Rectangle(0, 0, (int)this.Size.X, (int)this.Size.Y);
            //Rectangle s = new Rectangle(0, 0, this.frontPlayfield.Width, this.frontPlayfield.Height);
          //DS commented to remove front for snapshot  this.SharedSpriteBatch.Draw(this.frontPlayfield, r, s, Color.White);

       // no longer doing this    this.SharedSpriteBatch.End();
            skeletonDrawn = true;

            base.Draw(gameTime);
        }

        internal void DrawSkirtBone(JointCollection jointCollection)
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
            DrawBoneLike(depth, start, end, tp.pelvisTexture);
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {            base.LoadContent();

            tp.causeLoadContent();
            this.frontField = Game.Content.Load<Texture2D>("FrontField");
            this.frontMidField = Game.Content.Load<Texture2D>("FrontMidField");
            this.midField = Game.Content.Load<Texture2D>("MidField");
            this.backField = Game.Content.Load<Texture2D>("BackField");
 }

        /// <summary>
        /// This method draws a bone.
        /// </summary>
        /// <param name="joints">The joint data.</param>
        /// <param name="startJoint">The starting joint.</param>
        /// <param name="endJoint">The ending joint.</param>
        /// <param name="partTexture">The texture to use for the joint</param>
        internal void DrawCharacterPart(JointCollection joints, JointType startJoint, JointType endJoint, Texture2D boneTexture)
        {

            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                    joints[endJoint].TrackingState != JointTrackingState.Tracked)
                return;
          
            float depth = joints[startJoint].Position.Z;
            Vector2 start = this.mapMethod(joints[startJoint].Position);

            Vector2 end = this.mapMethod(joints[endJoint].Position);
            DrawBoneLike(depth, start, end, boneTexture);
            //if (startJoint == JointType.Head)
            //    Console.WriteLine("Cartoon Elements drawBone head position after remapping"+ start.X);
            
        }

        private void DrawBoneLike(float depth, Vector2 start, Vector2 end, Texture2D boneTexture)
        {
                Vector2 diff = end - start;
            Vector2 scale = new Vector2((float)(FOCAL_PLANE_DIST / Math.Max(NEAR_PLANE, depth)), 
                diff.Length() / boneTexture.Height * (1.0f / (1.0f - 2 * PIN_FROM_END)));

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Vector2 center = new Vector2((int)(start.X), (int)(start.Y));
            /* DS commented because not being used
            // Scale the dest, not the source!
            Rectangle sourceRect = new Rectangle(0, 0, partTexture.Width, partTexture.Height);
            Rectangle destRect = new Rectangle(0, 0, 0, 0);

            destRect.Offset((int)(start.X), (int)(start.Y));
            */
            // just for development
            Color color = Color.White;
      
       
            // Vector2 destctr = new Vector2(destRect.X, destRect.Y); 
            
            this.SharedSpriteBatch.Draw(boneTexture, center, null, color, angle,
                new Vector2(boneTexture.Width/2 , boneTexture.Height * PIN_FROM_END),
               scale, SpriteEffects.None, 1.0f);
        }


        // DS added the following two methods to make a parent part shorter by drawing the origin of the child part at a new orgiin point
        internal void DrawShortBone(JointCollection joints, JointType startJoint, JointType endJoint, JointType parentJoint, Texture2D boneTexture, float originAlongParent)
        {
            string nameJoint = startJoint.ToString();
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                    joints[endJoint].TrackingState != JointTrackingState.Tracked)
                return;

            float depth = joints[startJoint].Position.Z;
            Vector2 start = this.mapMethod(joints[startJoint].Position);
            Vector2 shortStart = this.mapMethod(joints[parentJoint].Position);
            Vector2 end = this.mapMethod(joints[endJoint].Position);
            DrawShortBoneLike(depth, start, end, shortStart, boneTexture, originAlongParent, nameJoint);
            //if (startJoint == JointType.Head)
            //    Console.WriteLine("Cartoon Elements drawBone head position after remapping"+ start.X);
        }

        private void DrawShortBoneLike(float depth, Vector2 start, Vector2 end, Vector2 shortStart, Texture2D boneTexture, float origin, string nameJoint)
        {
            float originPoint = origin;
            Vector2 longDiff = end - start;
            Vector2 shortDiff = start - shortStart;
            Vector2 scale = new Vector2((float)(FOCAL_PLANE_DIST / Math.Max(NEAR_PLANE, depth)),
                longDiff.Length() / boneTexture.Height * (1.0f / (1.0f - 2 * PIN_FROM_END)));

            float angle = (float)Math.Atan2(longDiff.Y, longDiff.X) - MathHelper.PiOver2;
                        Console.WriteLine(nameJoint + scale);
            Vector2 center = new Vector2((int)(shortStart.X+shortDiff.X/1.2), (int)(shortStart.Y+shortDiff.Y*originPoint)) ;



            // just for development
            Color color = Color.White;



            this.SharedSpriteBatch.Draw(boneTexture, center, null, color, angle,
                new Vector2(boneTexture.Width / 2, boneTexture.Height * PIN_FROM_END),
               scale, SpriteEffects.None, 1.0f);
        }

        // DS added the following for leaf body parts whose size need to overshoot their end joints such as the head.
        internal void DrawLongBone(JointCollection joints, JointType startJoint, JointType endJoint, Texture2D partTexture, float overshootAmount)
        {

            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                    joints[endJoint].TrackingState != JointTrackingState.Tracked)
                return;

            float depth = joints[startJoint].Position.Z;
            Vector2 start = this.mapMethod(joints[startJoint].Position);
            Vector2 end = this.mapMethod(joints[endJoint].Position);
            DrawLongBoneLike(depth, start, end, partTexture, overshootAmount);
        }

        private void DrawLongBoneLike(float depth, Vector2 start, Vector2 end, Texture2D partTexture, float overshootAmount)
        {
            Vector2 diff = (end - start) * overshootAmount ;
            Vector2 scale = new Vector2((float)(FOCAL_PLANE_DIST / Math.Max(NEAR_PLANE, depth)),
                diff.Length() / partTexture.Height * (1.0f / (1.0f - 2 * PIN_FROM_END)));

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Vector2 center = new Vector2((int)(start.X), (int)(start.Y));
          
            Color color = Color.White;

            this.SharedSpriteBatch.Draw(partTexture, center, null, color, angle,
                new Vector2(partTexture.Width / 2, partTexture.Height * PIN_FROM_END),
               scale, SpriteEffects.None, 1.0f);
        }



        internal void addSubRenderers(PaintersAlgorithmRenderer par)
        {
            if (false == this.initialized)
            {
                this.Initialize();
            }
            // If the joint texture isn't loaded, load all the content
            if (null == this.TP.lowerLegLeftTexture)
            {
                this.LoadContent();
            }
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            par.addSubRenderer(new BillboardSubrenderer(midField, MIDFIELD_Z));
            par.addSubRenderer(new BillboardSubrenderer(frontMidField, FRONT_MIDFIELD_Z));
            par.addSubRenderer(new BillboardSubrenderer(frontField, FRONT_FIELD_Z));
            par.addSubRenderer(new BillboardSubrenderer(backField, BACK_FIELD_Z));

            int playerID = 1;
            foreach (var skeleton in skeletonData)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // DS switched order, refactored FrontCharacterElements from SkeletonElements and added BackSkeletonElements so that video cutout is in front of head shape and behind other drawn elements
                    par.addSubRenderer(new BackCharacterElements(this, skeleton));
                    par.addSubRenderer(new LiveElements(this.playerImageRenderer, skeleton, this.playerImageRenderer.SkeletonToColorMap, playerID));
                    par.addSubRenderer(new FrontCharacterElements(this, skeleton));
                    
                }
                playerID++;
            }

            skeletonDrawn = true;
        }


    }
}
