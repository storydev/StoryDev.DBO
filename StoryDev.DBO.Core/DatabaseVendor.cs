using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
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
