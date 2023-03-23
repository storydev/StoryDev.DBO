using System.Reflection;

namespace StoryDev.DBO.Core
{
    public delegate void Signature(FieldInfo info, object obj, IDBReader reader);
    public delegate void OnBulkQueryExecute(string queries);
}