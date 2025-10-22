using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core.Json
{
    public class DBObject : IDBObject
    {

        public int ID;

        public static DBManager Manager { get; private set; }

        public static void SetManagerOptions(string name, string sourceFile)
        {
            if (Manager == null)
                Manager = new DBManager();

            Manager.SourcePath.Add(name, sourceFile);
            Manager.Items.Add(name, new List<object>());

            var dir = Path.GetDirectoryName(sourceFile);
            Directory.CreateDirectory(dir);

            if (!File.Exists(sourceFile))
                File.WriteAllText(sourceFile, "[]");
            else
            {
                var results = File.ReadAllText(sourceFile);
                Manager.Items[name] = JsonConvert.DeserializeObject<List<object>>(results);
            }
        }

        public void Delete()
        {
            var name = GetType().Name;

            var itemIndex = Manager.Items[name].FindIndex((obj) =>
            {
                var jObj = JObject.FromObject(obj);
                return jObj.Property("ID").Value.ToObject<int>() == ID;
            });
            if (itemIndex == -1)
                return;

            Manager.Items[name].RemoveAt(itemIndex);
            var content = JsonConvert.SerializeObject(Manager.Items);
            File.WriteAllText(Manager.SourcePath[name], content);
        }

        public void Insert()
        {
            var name = GetType().Name;

            ID = Manager.Items[name].Count;
            Manager.Items[name].Add(this);

            var content = JsonConvert.SerializeObject(Manager.Items[name]);
            File.WriteAllText(Manager.SourcePath[name], content);
        }

        public void Update(params DBFilter[] filters)
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            var name = GetType().Name;
            var itemIndex = Manager.Items[name].FindIndex((obj) => ((DBObject)obj).ID == ID);

            if (filters.Length > 0)
            {
                var itemResults = Manager.Items[name].Where((obj) =>
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

                foreach (var item in itemResults)
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].Name != "ID")
                            fields[i].SetValue(item, fields[i].GetValue(this));
                    }
                }
            }

            Manager.Items[name].RemoveAt(itemIndex);
            Manager.Items[name].Add(this);

            var content = JsonConvert.SerializeObject(Manager.Items[name]);
            File.WriteAllText(Manager.SourcePath[name], content);
        }
    }
}
