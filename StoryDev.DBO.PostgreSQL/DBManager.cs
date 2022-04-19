using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Npgsql;
using StoryDev.DBO.Scripting;

namespace StoryDev.DBO.PostgreSQL
{
    public class DBManager : IInstanceManager
    {

        internal bool IsBuilding;

        public DataStruct StructReference { get; set; }

        /// <summary>
        /// Source Path invalid for SQL connections.
        /// </summary>
        public Dictionary<string, string> SourcePath => throw new NotImplementedException();

        public string ConnectionInfo { get; set; }

        public int LastInsertedID { get; internal set; }

        /// <summary>
        /// A Dictionary representing the recently searched results of specific data structures.
        /// Normally used in conjunction with <c>StoryDev.DBO.Scripting</c> and the scripting API
        /// for StoryDev Data Studio.
        /// </summary>
        public Dictionary<string, List<object>> Items => throw new NotImplementedException();

        public void Begin()
        {
            IsBuilding = true;
        }

        /// <summary>
        /// Close a PostgreSQL database connection.
        /// </summary>
        /// <param name="con">The PostgreSQL database connection object.</param>
        public void CloseConnection(object con)
        {
            if (con != null && con.GetType() == typeof(NpgsqlConnection))
            {
                var casted = (NpgsqlConnection)con;
                casted.Close();
            }
        }

        public int End()
        {
            int result = 0;
            if (IsBuilding)
            {
                string query = BulkModifier.GetAllLines();
                var connection = (NpgsqlConnection)OpenConnection(ConnectionInfo);
                var command = new NpgsqlCommand(query, connection);
                result = command.ExecuteNonQuery();
                IsBuilding = false;
            }
            return result;
        }

        /// <summary>
        /// Opens a PostgreSQL connection with the given connection info string.
        /// </summary>
        /// <param name="connectionInfo">The string value to use to connect to a PostgreSQL database.</param>
        /// <returns>The PostgreSQL connection object.</returns>
        public object OpenConnection(string connectionInfo)
        {
            var connection = new NpgsqlConnection(connectionInfo);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Search a PostgreSQL database with the given filters. This function uses
        /// the Scripting implementation, normally provided in conjunction with StoryDev
        /// Data Studio.
        /// </summary>
        /// <param name="name">The name of the structure to search against, and its respective table in the dataset.</param>
        /// <param name="filters">The filters to use for searching.</param>
        /// <returns></returns>
        public IEnumerable<object> Search(string name, params DBFilter[] filters)
        {
            Type currentType = null;
            if (ItemConstructor.ResolvedTypes.ContainsKey(name))
            {
                currentType = ItemConstructor.ResolvedTypes[name];
            }

            FieldInfo[] fields = currentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            Utils.InitSignatures();

            if (currentType.BaseType != typeof(DBObject))
                return null;

            var clsName = currentType.Name;
            string query = "SELECT ";
            if (SearchOptions.UsingOptions && SearchOptions.UseSearchCount)
            {
                query += "COUNT(*)";
            }
            else
            {
                query += "*";
            }
            query += " FROM " + clsName;
            query += " WHERE " + Utils.GetFilterString(filters);

            if (SearchOptions.UsingOptions && !SearchOptions.UseSearchCount
                && SearchOptions.Limit > -1)
            {
                if (SearchOptions.CurrentPage > -1)
                {
                    query += " LIMIT " + SearchOptions.Limit + ", " + (SearchOptions.CurrentPage * SearchOptions.Limit);
                }
                else
                {
                    query += " LIMIT " + SearchOptions.Limit;
                }
            }

            query += ";";
            
            List<object> results = new List<object>();
            var connection = (NpgsqlConnection)OpenConnection(ConnectionInfo);
            var command = new NpgsqlCommand(query, connection);
            IDBReader reader = new DBReader(command.ExecuteReader());
            while (reader.Read())
            {
                if (!SearchOptions.UseSearchCount)
                {
                    var instance = Activator.CreateInstance(currentType);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];

                        if (field.IsInitOnly)
                            continue;
                        
                        if (Utils.SqlSignatures.ContainsKey(field.FieldType))
                            Utils.SqlSignatures[field.FieldType].Invoke(field, instance, reader);
                    }

                    results.Add(instance);
                }
            }

            CloseConnection(connection);

            return results;
        }

        public IEnumerable<T> Search<T>(params DBFilter[] filters)
        {
            Type currentType = typeof(T);

            FieldInfo[] fields = currentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            Utils.InitSignatures();

            if (currentType.BaseType != typeof(DBObject))
                return null;

            var clsName = currentType.Name;
            string query = "SELECT ";
            if (SearchOptions.UsingOptions && SearchOptions.UseSearchCount)
            {
                query += "COUNT(*)";
            }
            else
            {
                query += "*";
            }
            query += " FROM " + clsName;
            query += " WHERE " + Utils.GetFilterString(filters);

            if (SearchOptions.UsingOptions && !SearchOptions.UseSearchCount
                && SearchOptions.Limit > -1)
            {
                if (SearchOptions.CurrentPage > -1)
                {
                    query += " LIMIT " + SearchOptions.Limit + ", " + (SearchOptions.CurrentPage * SearchOptions.Limit);
                }
                else
                {
                    query += " LIMIT " + SearchOptions.Limit;
                }
            }

            query += ";";

            List<T> results = new List<T>();
            var connection = (NpgsqlConnection)OpenConnection(ConnectionInfo);
            var command = new NpgsqlCommand(query, connection);
            IDBReader reader = new DBReader(command.ExecuteReader());
            while (reader.Read())
            {
                if (!SearchOptions.UseSearchCount)
                {
                    var instance = (T)Activator.CreateInstance(currentType);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];

                        if (field.IsInitOnly)
                            continue;

                        if (Utils.SqlSignatures.ContainsKey(field.FieldType))
                            Utils.SqlSignatures[field.FieldType].Invoke(field, instance, reader);
                    }

                    results.Add(instance);
                }
            }

            CloseConnection(connection);

            return results;
        }
    }
}
