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
    public class SourceOptions
    {

        public string Name;
        public string Info;
        public SourceType Type;
        public DatabaseVendor Vendor;
        public bool IsDefaultConnection;

        public SourceOptions()
        {
            
        }

    }

    /// <summary>
    /// Used internally by StoryDev Data Studio for scripting.
    /// </summary>
    public enum SourceType
    { 
        File,
        Database,
    }

    /// <summary>
    /// Used internally by StoryDev Data Studio for scripting.
    /// </summary>
    public enum DatabaseVendor
    {
        None,
        SQLite,
        MySQL,
        Postgresql,
        Oracle,
        Microsoft
    }

}
