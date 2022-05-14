using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace StoryDev.DBO.Oracle
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
            var name = GetType().Name;
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

            var query = Utils.GenerateDelete(name, primaryKeyValue);

            if (!Manager.IsBuilding)
            {
                var connection = (OracleConnection)Manager.OpenConnection(Manager.ConnectionInfo);
                var command = new OracleCommand(query, connection);
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
            var name = GetType().Name;
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

            var query = Utils.GenerateInsert(name, fields, primaryKey, true);

            var connection = new OracleConnection(Manager.ConnectionInfo);
            var command = new OracleCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID")
                    continue;
                command.Parameters.Add("@" + field.Name, field.GetValue(this));
            }

            if (!Manager.IsBuilding)
            {
                connection.Open();
                command.Connection = connection;
                Manager.LastInsertedID = (int)command.ExecuteScalar();
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

            var connection = new OracleConnection(Manager.ConnectionInfo);
            var command = new OracleCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;
                command.Parameters.Add("@" + field.Name, field.GetValue(this));
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
                BulkModifier.AppendQuery(command.CommandText);
            }
        }
    }
}
