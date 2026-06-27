using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringOptionAttribute: Attribute
    {
        public StringOptionAttribute(params string[] options)
        {
            Options = options;
        }
        public ICollection<string> Options;
    }
}
