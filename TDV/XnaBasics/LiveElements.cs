using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class LiveElements : SubRenderer
    {
        private PlayerImageRenderer playerImageRenderer;
        private Skeleton skeleton;
        private SkeletonPointMap mapMethod;
        private int playerID;

        public override float Z
        {
            get
            {
                return skeleton.Joints[JointType.HipCenter].Position.Z;
            }
            set
            {
                ;
            }
        }

        public LiveElements(PlayerImageRenderer playerImageRenderer, Microsoft.Kinect.Skeleton skeleton, SkeletonPointMap mapMethod, int playerID)
        {
            this.playerImageRenderer = playerImageRenderer;
            this.skeleton = skeleton;
            this.mapMethod = mapMethod;
            this.playerID = playerID;
        }

        private const float HEAD_HEIGHT = 96.0f;
        private const float HEAD_WIDTH = 96.0f;
        private const float HAND_HEIGHT = 48.0f;
        private const float HAND_WIDTH = 48.0f;

        internal override void Draw(SpriteBatch ssb)
        {
            Rectangle[] rects = new Rectangle[3];
            
    
            Vector2 head = this.mapMethod(skeleton.Joints[JointType.Head].Position);

            rects[0] = new Rectangle(
                     (int)(head.X - HEAD_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.Head].Position.Z) / 2.0f),
                     (int)(head.Y - HEAD_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.Head].Position.Z) / 2.0f),
                     (int)(HEAD_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.Head].Position.Z)),
                     (int)(HEAD_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.Head].Position.Z))
                     );

            Vector2 left_hand = this.mapMethod(skeleton.Joints[JointType.HandLeft].Position);
            rects[1] = new Rectangle(
                     (int)(left_hand.X - HAND_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandLeft].Position.Z) / 2.0f),
                     (int)(left_hand.Y - HAND_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandLeft].Position.Z) / 2.0f),
                     (int)(HAND_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandLeft].Position.Z)),
                     (int)(HAND_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandLeft].Position.Z))
                     );

            Vector2 right_hand = this.mapMethod(skeleton.Joints[JointType.HandRight].Position);
            rects[2] = new Rectangle(
                     (int)(right_hand.X - HAND_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandRight].Position.Z) / 2.0f),
                     (int)(right_hand.Y - HAND_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandRight].Position.Z) / 2.0f),
                     (int)(HAND_WIDTH * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandRight].Position.Z)),
                     (int)(HAND_HEIGHT * 1.5f / Math.Max(1.0f, skeleton.Joints[JointType.HandRight].Position.Z))
                     );

            playerImageRenderer.Draw(ssb, playerID, rects);
        }
    }
}
