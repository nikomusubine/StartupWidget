using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FloatBoundsAttribute: Attribute
    {
        public float Max { get; set; }
        public float Min { get; set; }

        public FloatBoundsAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
