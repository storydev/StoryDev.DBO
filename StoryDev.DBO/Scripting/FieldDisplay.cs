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
    public enum FieldDisplay
    {
        Date,
        Time,
        DateTime,
        Dropdown,
        Numeric,
        SingleText,
        MultilineText,
        MarkdownText,
        Array
    }
}
