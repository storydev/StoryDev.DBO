using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{

    public enum FieldScope
    {
        Property,
        Field
    }

    public class FieldRef
    {

        public FieldInfo? Field;
        public PropertyInfo? Property;
        public FieldScope Scope;

        public FieldRef()
        {

        }

    }
}
