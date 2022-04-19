using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Scripting
{
    /// <summary>
    /// Used internally by StoryDev Data Studio for scripting.
    /// </summary>
    public enum DataFieldType
    {
        NONE,
        INTEGER,
        FLOAT,
        STRING,
        BOOLEAN,
        DATETIME,
        OFARRAY,
    }
}
