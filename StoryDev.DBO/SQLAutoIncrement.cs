using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SQLAutoIncrement : Attribute
    {

        public SQLAutoIncrement()
        {

        }

    }
}
