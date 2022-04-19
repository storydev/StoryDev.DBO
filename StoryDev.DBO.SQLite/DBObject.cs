using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SQLite;

namespace StoryDev.DBO.SQLite
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

        public int ID;

        public void Delete()
        {
            var query = Utils.GenerateDelete(GetType().Name, ID);

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
            var fields = GetType().GetFields();
            var query = Utils.GenerateInsert(GetType().Name, fields);

            var connection = new SQLiteConnection(Manager.ConnectionInfo);
            var command = new SQLiteCommand(query);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID")
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
            var query = Utils.GenerateUpdate(name, fields, ID, filters);

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
