using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class ZOrderIComparer : Comparer<SubRenderer>
    {
        private static ZOrderIComparer def =new ZOrderIComparer();

        static public ZOrderIComparer defaultComparer()
        {
            return def;
        }

        public override int Compare(SubRenderer a, SubRenderer b)
        {
            if (a.Z < b.Z) return 1;
            if (a.Z > b.Z) return -1;
            return 0;
        }
    }
}
