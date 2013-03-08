using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class BillboardSubrenderer : SubRenderer
    {
        private Texture2D texture;

        public BillboardSubrenderer(Texture2D t, float zval)
        {
            Z = zval;
            texture = t;
        }

        internal override void Draw(SpriteBatch SharedSpriteBatch)
        {
            SharedSpriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
        }
    }
}
