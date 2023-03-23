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
            object primaryKeyValue = null;
            foreach (var field in fields)
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
            string primaryKey = null;
            foreach (var field in fields)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKey = field.Name;
                }
            }

            if (primaryKey == null)
            {
                throw new Exception("No primary key has been set for this database object type.");
            }

            var query = Utils.GenerateInsert(GetType().Name, fields, primaryKey);

            var connection = new SQLiteConnection(Manager.ConnectionInfo);
            var command = new SQLiteCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == primaryKey)
                    continue;
                command.Parameters.AddWithValue("@" + field.Name, field.GetValue(this));
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
            string primaryKey = null;
            object primaryKeyValue = null;
            foreach (var field in fields)
            {
                var primaryKeys = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeys != null)
                {
                    primaryKey = field.Name;
                    primaryKeyValue = field.GetValue(this);
                }
            }

            if (primaryKey == null || primaryKeyValue == null)
            {
                throw new Exception("No primary key has been set for this database object type.");
            }

            var query = Utils.GenerateUpdate(name, fields, primaryKeyValue, primaryKey, filters);

            var connection = new SQLiteConnection(Manager.ConnectionInfo);
            var command = new SQLiteCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;
                command.Parameters.AddWithValue("@" + field.Name, field.GetValue(this));
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
