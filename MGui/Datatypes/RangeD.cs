using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// Structure representing a range of double values.
    /// </summary>
    public struct RangeD
    {
        public double Max;
        public double Min;

        public RangeD( double min, double max )
        {
            this.Min = min;
            this.Max = max;
        }
    }
}
