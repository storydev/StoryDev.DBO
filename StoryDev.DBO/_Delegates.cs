using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace StoryDev.DBO
{
    public delegate void Signature(FieldInfo info, object obj, IDBReader reader);
    public delegate void OnBulkQueryExecute(string queries);
}
