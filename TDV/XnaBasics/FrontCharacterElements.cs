using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class FrontCharacterElements : SubRenderer
    {
        private Microsoft.Kinect.Skeleton skeleton;
        private CartoonElements cartooner;
        private const float DEPTH_DELTA = -0.1f;
        private TextureSet textures;

        public override float Z
        {
            get
            {
                return skeleton.Joints[JointType.HipCenter].Position.Z + DEPTH_DELTA;
            }
            set {
                ;
            }
        }

        public FrontCharacterElements(CartoonElements cartooner, Microsoft.Kinect.Skeleton skeleton, TextureSet ts)
        {
            this.skeleton = skeleton;
            this.cartooner = cartooner;
            this.textures = ts;
        }


        internal override void Draw(Xna.Framework.Graphics.SpriteBatch SharedSpriteBatch)
        {
      
            // Draw Bones using painters algorithm

            // DS uncommented the waist to fill in abdominal gap
           cartooner.DrawCharacterPart(skeleton.Joints, JointType.Spine, JointType.HipCenter, textures.waistTexture);  // Waist
            // DS commented out drawing the small pelvis joints
            //        this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, this.partTexture);
            //      this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipRight, this.partTexture);
            /* left of screen and left arm of player, NOT stage left */
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, textures.upperArmRightTexture);  // Upper Arm Right 
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, textures.upperArmLeftTexture);   // Upper Arm Left 
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, textures.shoulderRightTexture); // Shoulder Right
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, textures.shoulderLeftTexture); // Shoulder Left
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, textures.lowerLegRightTexture); // Lower Leg Right
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.HipRight, JointType.KneeRight, textures.upperLegRightTexture); // Upper Leg Right 
            //cartooner.DrawCharacterPart(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, cartooner.footRightTexture);    // Foot Right    
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, textures.lowerLegLeftTexture); //Lower Leg Left
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, textures.upperLegLeftTexture); // Upper Leg Left
            //cartooner.DrawCharacterPart(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, cartooner.partTexture); // Foot Left
            cartooner.DrawSkirtBone(skeleton.Joints, textures); // Skirt
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, textures.torsoTexture); // Torso
            // cartooner.DrawCharacterPart(skeleton.Joints, JointType.WristRight, JointType.HandRight, cartooner.rightHandTexture); // Right hand 
            cartooner.DrawShortBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, JointType.ShoulderRight, textures.lowerArmRightTexture, 0.93f); // Lower Arm Right
           // cartooner.DrawCharacterPart(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, cartooner.partTexture); // Left hand
            cartooner.DrawShortBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, JointType.ShoulderLeft, textures.lowerArmLeftTexture, 0.93f); // Lower Arm Left
            cartooner.DrawLongBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Head, textures.headTexture,1.7f); // head accessories (pony tails, hat, bangs, etc
        }
    }
}
