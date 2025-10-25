using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core.SQLite
{
    public class DBObject : IDBObject
    {

        private static DBManager manager;
        public static DBManager Manager
        {
            get
            {
                if (manager == null)
                    manager = new DBManager();
                return manager;
            }
        }

        public void Delete()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            object primaryKeyValue = null;

            foreach (var field in fields)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKeyValue = field.GetValue(this);
                }
            }

            foreach (var field in properties)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKeyValue = field.GetValue(this);
                }
            }

            if (primaryKeyValue == null)
            {
                throw new Exception("No primary key has been set for this database object type.");
            }

            var query = Utils.GenerateDelete(GetType().Name, primaryKeyValue);

            if (!Manager.IsBuilding)
            {
                var connection = (SQLiteConnection)Manager.OpenConnection(Manager.ConnectionInfo);
                var command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(query);
            }
        }

        public void Insert()
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string primaryKey = null;
            var refs = new List<FieldRef>();
            foreach (var field in fields)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKey = field.Name;
                }

                refs.Add(new FieldRef
                {
                    Field = field,
                    Property = null,
                    Scope = FieldScope.Field
                });
            }

            foreach (var field in properties)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKey = field.Name;
                }

                refs.Add(new FieldRef
                {
                    Field = null,
                    Property = field,
                    Scope = FieldScope.Property
                });
            }

            if (primaryKey == null)
            {
                throw new Exception("No primary key has been set for this database object type.");
            }

            var query = Utils.GenerateInsert(GetType().Name, refs.ToArray(), primaryKey);

            var connection = new SQLiteConnection(Manager.ConnectionInfo);
            var command = new SQLiteCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == primaryKey)
                    continue;
                command.Parameters.AddWithValue("@" + field.Name, field.GetValue(this));
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.Name == primaryKey)
                    continue;
                command.Parameters.AddWithValue("@" + property.Name, property.GetValue(this));
            }

            if (!Manager.IsBuilding)
            {
                connection.Open();
                command.Connection = connection;
                command.ExecuteNonQuery();
                Manager.LastInsertedID = (int)connection.LastInsertRowId;
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(command.CommandText);
            }
        }

        public void Update(DBFilter[] filters = null)
        {
            var name = GetType().Name;
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string primaryKey = null;
            object primaryKeyValue = null;
            var refs = new List<FieldRef>();
            foreach (var field in fields)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKey = field.Name;
                    primaryKeyValue = field.GetValue(this);
                }

                refs.Add(new FieldRef
                {
                    Field = field,
                    Property = null,
                    Scope = FieldScope.Field
                });
            }

            foreach (var field in properties)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKeyValue = field.GetValue(this);
                    primaryKey = field.Name;
                }

                refs.Add(new FieldRef
                {
                    Field = null,
                    Property = field,
                    Scope = FieldScope.Property
                });
            }

            if (primaryKey == null || primaryKeyValue == null)
            {
                throw new Exception("No primary key has been set for this database object type.");
            }

            var query = Utils.GenerateUpdate(name, refs.ToArray(), primaryKeyValue, primaryKey, filters);

            var connection = new SQLiteConnection(Manager.ConnectionInfo);
            var command = new SQLiteCommand(query);
            for (int i = 0; i < refs.Count; i++)
            {
                var field = refs[i];
                if (field.Scope == FieldScope.Property)
                {
                    if (field.Property == null)
                        continue;
                    command.Parameters.AddWithValue("@" + field.Property.Name, field.Property.GetValue(this));
                }
                else if (field.Scope == FieldScope.Field)
                {
                    if (field.Field == null)
                        continue;
                    command.Parameters.AddWithValue("@" + field.Field.Name, field.Field.GetValue(this));
                }                
            }

            if (!Manager.IsBuilding)
            {
                connection.Open();
                command.Connection = connection;
                command.ExecuteNonQuery();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(query);
            }
        }
    }
}
