using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class SkeletonElements : SubRenderer
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

        public SkeletonElements(CartoonElements cartooner, Microsoft.Kinect.Skeleton skeleton)
        {

            this.skeleton = skeleton;
            this.cartooner = cartooner;
        }


        internal override void Draw(Xna.Framework.Graphics.SpriteBatch SharedSpriteBatch)
        {

            // Draw Bones using painters algorithm


            cartooner.DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter, cartooner.waistTexture);  // Waist
            // DS commented out drawing the small pelvis joints
            //        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft, this.boneTexture);
            //      this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight, this.boneTexture);
            /* left of screen and left arm of player, NOT stage left */
            cartooner.DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft, cartooner.upperArmRightTexture);  // Upper Arm Right 
            cartooner.DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight, cartooner.upperArmLeftTexture);   // Upper Arm Left 
            cartooner.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft, cartooner.shoulderRightTexture); // Shoulder Right
            cartooner.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight, cartooner.shoulderLeftTexture); // Shoulder Left
            cartooner.DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter, cartooner.headTexture); // Head  
            cartooner.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine, cartooner.torsoTexture); // Torso
            cartooner.DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight, cartooner.rightHandTexture); // Right hand 
            cartooner.DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight, cartooner.lowerArmRightTexture); // Lower Arm Right
            cartooner.DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft, cartooner.boneTexture); // Left hand
            cartooner.DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft, cartooner.lowerArmLeftTexture); // Lower Arm Left
            cartooner.DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft, cartooner.upperLegRightTexture); // Upper Leg Right       
            cartooner.DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft, cartooner.lowerLegRightTexture); //Lower Leg Right
            cartooner.DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight, cartooner.footRightTexture);    // Foot Right    
            cartooner.DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight, cartooner.upperLegLeftTexture); // Upper Leg Left
            cartooner.DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight, cartooner.lowerLegLeftTexture); // Lower Leg Left
            cartooner.DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft, cartooner.boneTexture); // Foot Left
            cartooner.DrawSkirtBone(skeleton.Joints); // Skirt
        }
    }
}
