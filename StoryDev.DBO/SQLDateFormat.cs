using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SQLDate : Attribute
    {

        public SQLDateFormat Format;

        public SQLDate(SQLDateFormat format)
        {
            Format = format;
        }

    }

    public enum SQLDateFormat
    {
        Date,
        Time,
        DateTime
    }
}
