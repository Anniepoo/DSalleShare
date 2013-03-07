using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    abstract class SubRenderer : Object
    {
        private float zz = 0.0f;

        public float Z {
            get 
            { 
                return zz; 
            }
            set
            {
                zz = Z;
            }
        }

        internal void Draw(Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
