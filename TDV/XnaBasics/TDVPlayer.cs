using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class TDVPlayer
    {
        private const int NUM_TEXTURE_SETS = 4;
        private static TextureSet[] textures = new TextureSet[NUM_TEXTURE_SETS];


        private int appearance = 0;

        public TextureSet Textures
        {
            get
            {
                ensureTexturesLoaded();

                return textures[appearance];
            }
        }

        public int Appearance
        {
            get
            {
                return appearance;
            }
            set
            {
                appearance = value;
                while (appearance < 0) appearance += NUM_TEXTURE_SETS;
                while (appearance >= NUM_TEXTURE_SETS) appearance -= NUM_TEXTURE_SETS;
            }
        }

        public TDVPlayer()
        {

        }

        // TODO CODE DEBT refactor this into a TextureProvider and a TextureSet
        private static void ensureTexturesLoaded()
        {
            if (textures[0] != null) return;

            textures[0] = new TextureSet(TDVBasicGame.Default, "f1");
            textures[0].causeLoadContent();
            textures[1] = new TextureSet(TDVBasicGame.Default, "f2");
            textures[1].causeLoadContent();
            textures[2] = new TextureSet(TDVBasicGame.Default, "m1");
            textures[2].causeLoadContent();
            textures[3] = new TextureSet(TDVBasicGame.Default, "m2");
            textures[3].causeLoadContent();
        }
    }
}
