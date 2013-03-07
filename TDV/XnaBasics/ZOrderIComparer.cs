using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class ZOrderIComparer : IComparer<SubRenderer>
    {
        private static ZOrderIComparer def =new ZOrderIComparer();

        static public ZOrderIComparer defaultComparer()
        {
            return def;
        }

        #region IComparer<SubRenderer> Members

        public int Compare(SubRenderer a, SubRenderer b)
        {
            if (a.Z < b.Z) return -1;
            if (a.Z > b.Z) return 1;
            return 0;
        }

        #endregion
    }
}
