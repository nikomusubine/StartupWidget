using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoubleBoundsAttribute: Attribute
    {
        public double Max { get; set; }
        public double Min { get; set; }

        public DoubleBoundsAttribute(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
