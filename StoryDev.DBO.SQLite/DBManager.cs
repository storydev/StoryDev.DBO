using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Data.SQLite;
using StoryDev.DBO.Scripting;

namespace StoryDev.DBO.SQLite
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
        /// Close a SQLite database connection.
        /// </summary>
        /// <param name="con">The SQLite database connection object.</param>
        public void CloseConnection(object con)
        {
            if (con != null && con.GetType() == typeof(SQLiteConnection))
            {
                var casted = (SQLiteConnection)con;
                casted.Close();
            }
        }

        public void CreateTable(string name, Type dbType = null)
        {
            throw new NotImplementedException();
        }

        public void CreateTable<T>()
        {
            throw new NotImplementedException();
        }

        public int End()
        {
            int result = 0;
            if (IsBuilding)
            {
                string query = BulkModifier.GetAllLines();
                var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
                var command = new SQLiteCommand(query, connection);
                result = command.ExecuteNonQuery();
                IsBuilding = false;
            }
            return result;
        }

        /// <summary>
        /// Opens a SQLite connection with the given connection info string.
        /// </summary>
        /// <param name="connectionInfo">The string value to use to connect to a SQLite database.</param>
        /// <returns>The SQLite connection object.</returns>
        public object OpenConnection(string connectionInfo)
        {
            var connection = new SQLiteConnection(connectionInfo);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Search a SQLite database with the given filters. This function uses
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
            var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
            var command = new SQLiteCommand(query, connection);
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
            var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
            var command = new SQLiteCommand(query, connection);
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
