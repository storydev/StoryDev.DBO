using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SQLStringSize : Attribute
    {

        public SQLStringType StringType;
        public int Size;

        public SQLStringSize(SQLStringType type, int size)
        {
            StringType = type;
            Size = size;
        }


    }

    public enum SQLStringType
    {
        Fixed,
        Variable
    }

    public enum SQLStringSizeFormat
    {
        Tiny = 0,
        Small,
        Normal,
        Medium,
        Long
    }
}
