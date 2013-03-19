using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    abstract public class TextureProvider : DrawableGameComponent
    {
        private Game game;
        string prefix;

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
        internal Texture2D hairDoTexture;
        internal Texture2D hairBottomTexture;

        public TextureProvider(Game game, string prefix) :
            base(game)
        {
            this.game = game;
            this.prefix = prefix;
        }

        public void causeLoadContent()
        {
            LoadContent();
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.footRightTexture = Game.Content.Load<Texture2D>(prefix+"FootRight");
            this.rightHandTexture = Game.Content.Load<Texture2D>(prefix + "RightHand");
            this.footLeftTexture = Game.Content.Load<Texture2D>(prefix + "FootLeft");
            this.leftHandTexture = Game.Content.Load<Texture2D>(prefix + "LeftHand");
            this.headTexture = Game.Content.Load<Texture2D>(prefix + "Head");
            this.boneTexture = Game.Content.Load<Texture2D>(prefix + "Bone");
            this.torsoTexture = Game.Content.Load<Texture2D>(prefix + "Torso");
            this.shoulderRightTexture = Game.Content.Load<Texture2D>(prefix + "ShoulderRight");
            this.upperArmRightTexture = Game.Content.Load<Texture2D>(prefix + "UpperArmRight");
            this.lowerArmRightTexture = Game.Content.Load<Texture2D>(prefix + "LowerArmRight");
            this.shoulderLeftTexture = Game.Content.Load<Texture2D>(prefix + "ShoulderLeft");
            this.upperArmLeftTexture = Game.Content.Load<Texture2D>(prefix + "UpperArmLeft");
            this.lowerArmLeftTexture = Game.Content.Load<Texture2D>(prefix + "LowerArmLeft");
            this.upperLegRightTexture = Game.Content.Load<Texture2D>(prefix + "UpperLegRight");
            this.upperLegLeftTexture = Game.Content.Load<Texture2D>(prefix + "UpperLegLeft");
            this.lowerLegRightTexture = Game.Content.Load<Texture2D>(prefix + "LowerLegRight");
            this.lowerLegLeftTexture = Game.Content.Load<Texture2D>(prefix + "LowerLegLeft");
            this.waistTexture = Game.Content.Load<Texture2D>(prefix + "Waist");
            this.pelvisTexture = Game.Content.Load<Texture2D>(prefix + "Pelvis");
            this.hairDoTexture = Game.Content.Load<Texture2D>(prefix + "HairDo");
            this.hairBottomTexture = Game.Content.Load<Texture2D>(prefix + "HairBottom");
        }
    }
}
