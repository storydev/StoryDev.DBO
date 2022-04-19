using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace StoryDev.DBO.MSSQL
{
    public class DBObject : IDBObject
    {

        private static DBManager manager;
        /// <summary>
        /// The main Database Manager for this Database Object, allowing
        /// for the use of commonly used actions against the current database.
        /// </summary>
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
        
        /// <summary>
        /// Delete this instance from the database.
        /// </summary>
        public void Delete()
        {
            var name = GetType().Name;
            var query = Utils.GenerateDelete(name, ID);

            if (!Manager.IsBuilding)
            {
                var connection = (SqlConnection)Manager.OpenConnection(Manager.ConnectionInfo);
                var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(query);
            }
        }

        /// <summary>
        /// Insert the current instance into the database. If this instance already exists, <c>Update</c> should be used.
        /// </summary>
        public void Insert()
        {
            var name = GetType().Name;
            var fields = GetType().GetFields();
            var query = Utils.GenerateInsert(name, fields, true);

            var connection = new SqlConnection(Manager.ConnectionInfo);
            var command = new SqlCommand(query);
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
                Manager.LastInsertedID = (int)command.ExecuteScalar();
                Manager.CloseConnection(connection);
            }
            else
            {
                BulkModifier.AppendQuery(command.CommandText);
            }
        }

        /// <summary>
        /// Update this instance into the database. If the optional <c>filters</c> are provided, this instance will be used to update all
        /// records that match those filters.
        /// </summary>
        /// <param name="filters"></param>
        public void Update(DBFilter[] filters = null)
        {
            var name = GetType().Name;
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var query = Utils.GenerateUpdate(name, fields, ID, filters);

            var connection = new SqlConnection(Manager.ConnectionInfo);
            var command = new SqlCommand(query);
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
                BulkModifier.AppendQuery(command.CommandText);
            }
        }

    }
}
