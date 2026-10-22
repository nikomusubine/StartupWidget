using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IntBoundsAttribute:Attribute
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public IntBoundsAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}
