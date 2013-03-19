using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class BackCharacterElements : SubRenderer
    {
        private Microsoft.Kinect.Skeleton skeleton;
        private CartoonElements cartooner;
        private const float DEPTH_DELTA = 0.1f;

        public override float Z
        {
            get
            {
                return skeleton.Joints[JointType.HipCenter].Position.Z + DEPTH_DELTA;
            }
            set
            {
                ;
            }
        }

        public BackCharacterElements(CartoonElements cartooner, Microsoft.Kinect.Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.cartooner = cartooner;
        }


        internal override void Draw(Xna.Framework.Graphics.SpriteBatch SharedSpriteBatch)
        {

            // Draw Bones using painters algorithm


            cartooner.DrawLongBone(skeleton.Joints, JointType.Spine, JointType.ShoulderCenter,  cartooner.TP.hairBottomTexture, 1.3f);  // Neck and hairBottom
            cartooner.DrawLongBone(skeleton.Joints,JointType.ShoulderCenter, JointType.Head,  cartooner.TP.hairDoTexture,1.5f); // Head  (pigtails and ribbons)
           
        }
    }
}
