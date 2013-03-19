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


        public override float Z
        {
            get
            {
                return skeleton.Joints[JointType.HipCenter].Position.Z;
            }
            set {
                ;
            }
        }

        public FrontCharacterElements(CartoonElements cartooner, Microsoft.Kinect.Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.cartooner = cartooner;
        }


        internal override void Draw(Xna.Framework.Graphics.SpriteBatch SharedSpriteBatch)
        {
      
            // Draw Bones using painters algorithm

            // DS uncommented the waist to fill in abdominal gap
           cartooner.DrawCharacterPart(skeleton.Joints, JointType.Spine, JointType.HipCenter, cartooner.waistTexture);  // Waist
            // DS commented out drawing the small pelvis joints
            //        this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, this.partTexture);
            //      this.DrawCharacterPart(skeleton.Joints, JointType.HipCenter, JointType.HipRight, this.partTexture);
            /* left of screen and left arm of player, NOT stage left */
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, cartooner.TP.upperArmRightTexture);  // Upper Arm Right 
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, cartooner.TP.upperArmLeftTexture);   // Upper Arm Left 
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, cartooner.TP.shoulderRightTexture); // Shoulder Right
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, cartooner.TP.shoulderLeftTexture); // Shoulder Left
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, cartooner.TP.lowerLegRightTexture); // Lower Leg Right
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.HipRight, JointType.KneeRight, cartooner.TP.upperLegRightTexture); // Upper Leg Right 
            //cartooner.DrawCharacterPart(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, cartooner.footRightTexture);    // Foot Right    
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, cartooner.TP.lowerLegLeftTexture); //Lower Leg Left
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, cartooner.TP.upperLegLeftTexture); // Upper Leg Left
            //cartooner.DrawCharacterPart(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, cartooner.partTexture); // Foot Left
            cartooner.DrawSkirtBone(skeleton.Joints); // Skirt
            cartooner.DrawCharacterPart(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, cartooner.TP.torsoTexture); // Torso
            // cartooner.DrawCharacterPart(skeleton.Joints, JointType.WristRight, JointType.HandRight, cartooner.rightHandTexture); // Right hand 
            cartooner.DrawShortBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, JointType.ShoulderRight, cartooner.TP.lowerArmRightTexture, 0.83f); // Lower Arm Right
           // cartooner.DrawCharacterPart(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, cartooner.partTexture); // Left hand
            cartooner.DrawShortBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, JointType.ShoulderLeft, cartooner.TP.lowerArmLeftTexture, 0.83f); // Lower Arm Left
            cartooner.DrawLongBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Head, cartooner.TP.headTexture,1.5f);
        }
    }
}
