using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoryDev.DBO.Scripting;

namespace StoryDev.DBO.Json
{
    public class DBManager : IInstanceManager
    {

        public DataStruct StructReference { get; set; }

        public int LastInsertedID { get; internal set; }

        public string ConnectionInfo { get; set; }

        private Dictionary<string, string> sourcePath;
        public Dictionary<string, string> SourcePath
        {
            get
            {
                if (sourcePath == null)
                    sourcePath = new Dictionary<string, string>();
                return sourcePath;
            }
        }

        private Dictionary<string, List<object>> items;
        public Dictionary<string, List<object>> Items
        {
            get
            {
                if (items == null)
                    items = new Dictionary<string, List<object>>();
                return items;
            }
        }

        public IEnumerable<object> Search(string name, params DBFilter[] filters)
        {
            IEnumerable<object> results = null;
            if (filters.Length > 0)
            {
                results = items[name].Where((object obj) =>
                {
                    var found = false;

                    foreach (var filter in filters)
                    {
                        var value = obj.GetType().GetField(filter.FieldName).GetValue(obj);
                        if (!Utils.GetResultOf(value, filter.Operator, filter.ConditionValue))
                            return false;

                        found = true;
                    }

                    return found;
                });
            }
            else
            {
                results = items[name];
            }

            return results;
        }

        /// <summary>
        /// Not implemented for JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filters"></param>
        /// <returns></returns>
        public IEnumerable<T> Search<T>(params DBFilter[] filters)
        {
            return null;
        }

        public object OpenConnection(string connectionInfo)
        {
            // Not required for JSON.
            return null;
        }

        public void CloseConnection(object con)
        {
            // Not required for JSON.
            return;
        }

        public void Begin()
        {
            
        }

        public int End()
        {
            return 0;
        }

        public void CreateTable(string name, Type dbType = null)
        {
            throw new NotImplementedException();
        }

        public void CreateTable<T>()
        {
            throw new NotImplementedException();
        }
    }
}
