using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class HTMLMaskAttribute : Attribute
    {
        public HTMLMaskAttribute() { }
        public HTMLMaskAttribute(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}
