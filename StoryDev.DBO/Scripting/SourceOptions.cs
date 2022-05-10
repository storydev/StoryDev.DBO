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
        public ConnectionString Info;
        public ConnectionString TestInfo;
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
        None = -1,
        SQLite = 0,
        MySQL = 1,
        Postgresql = 2,
        Oracle = 3,
        Microsoft = 4,
    }


}
