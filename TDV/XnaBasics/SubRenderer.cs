﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    abstract class SubRenderer : Object
    {
        private float zz = 0.0f;

        public virtual float Z {
            get 
            { 
                return zz; 
            }
            set
            {
                zz = value;
            }
        }

        internal abstract void Draw(SpriteBatch SharedSpriteBatch);
            
    }
}
