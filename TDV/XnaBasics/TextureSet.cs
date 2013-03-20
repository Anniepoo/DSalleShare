using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class TextureSet : DrawableGameComponent
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

        public TextureSet(Game game, string prefix) :
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
            if (footRightTexture != null) return;

            string prefixstring = ".\\characters\\" + prefix + "\\";

            this.footRightTexture = Game.Content.Load<Texture2D>(prefixstring+"FootRight");
            this.rightHandTexture = Game.Content.Load<Texture2D>(prefixstring + "RightHand");
            this.footLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "FootLeft");
            this.leftHandTexture = Game.Content.Load<Texture2D>(prefixstring + "LeftHand");
            this.headTexture = Game.Content.Load<Texture2D>(prefixstring + "Head");
            this.boneTexture = Game.Content.Load<Texture2D>(prefixstring + "Bone");
            this.torsoTexture = Game.Content.Load<Texture2D>(prefixstring + "Torso");
            this.shoulderRightTexture = Game.Content.Load<Texture2D>(prefixstring + "ShoulderRight");
            this.upperArmRightTexture = Game.Content.Load<Texture2D>(prefixstring + "UpperArmRight");
            this.lowerArmRightTexture = Game.Content.Load<Texture2D>(prefixstring + "LowerArmRight");
            this.shoulderLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "ShoulderLeft");
            this.upperArmLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "UpperArmLeft");
            this.lowerArmLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "LowerArmLeft");
            this.upperLegRightTexture = Game.Content.Load<Texture2D>(prefixstring + "UpperLegRight");
            this.upperLegLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "UpperLegLeft");
            this.lowerLegRightTexture = Game.Content.Load<Texture2D>(prefixstring + "LowerLegRight");
            this.lowerLegLeftTexture = Game.Content.Load<Texture2D>(prefixstring + "LowerLegLeft");
            this.waistTexture = Game.Content.Load<Texture2D>(prefixstring + "Waist");
            this.pelvisTexture = Game.Content.Load<Texture2D>(prefixstring + "Pelvis");
            this.hairDoTexture = Game.Content.Load<Texture2D>(prefixstring + "HairDo");
            this.hairBottomTexture = Game.Content.Load<Texture2D>(prefixstring + "HairBottom");
        }
    }
}
