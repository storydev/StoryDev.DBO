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
            if (string.IsNullOrEmpty(name) && dbType == null)
            {
                throw new ArgumentException("name");
            }

            Type resolved = null;
            if (dbType != null)
            {
                resolved = dbType;
            }
            else if (ItemConstructor.ResolvedTypes.ContainsKey(name))
            {
                resolved = ItemConstructor.ResolvedTypes[name];
            }

            if (string.IsNullOrEmpty(name))
            {
                name = dbType.Name;
            }

            string query = "";
            query += "CREATE TABLE IF NOT EXISTS " + name + " (\r\n";

            string primaryKey = "";
            FieldInfo[] fields = resolved.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.IsInitOnly)
                    continue;

                query += "\t";

                var primaryKeyAttr = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                if (primaryKeyAttr != null)
                {
                    if (string.IsNullOrEmpty(primaryKey))
                    {
                        primaryKey = field.Name;
                    }
                    else
                    {
                        throw new Exception("You cannot have more than one primary key on the database object.");
                    }
                }

                query += field.Name;
                query += " ";
                query += Utils.GetSQLType(field.FieldType, DatabaseVendor.SQLite) + " ";

                if (primaryKeyAttr != null)
                {
                    query += "PRIMARY KEY ";
                }

                var autoIncrementAttr = (SQLAutoIncrement)field.GetCustomAttribute(typeof(SQLAutoIncrement));
                if (autoIncrementAttr != null && primaryKeyAttr != null)
                {
                    query += "AUTOINCREMENT ";
                }

                if (i < fields.Length - 1)
                {
                    query += ",\r\n";
                }
            }

            query += ");";

            var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
            CloseConnection(connection);
        }



        public void CreateTable<T>()
        {
            CreateTable("", typeof(T));
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
            if (Searching.UsingOptions && Searching.UseSearchCount)
            {
                query += "COUNT(*)";
            }
            else
            {
                query += "*";
            }
            query += " FROM " + clsName;
            query += " WHERE " + Utils.GetFilterString(filters);

            if (Searching.UsingOptions && !Searching.UseSearchCount
                && Searching.Limit > -1)
            {
                if (Searching.CurrentPage > -1)
                {
                    query += " LIMIT " + Searching.Limit + ", " + (Searching.CurrentPage * Searching.Limit);
                }
                else
                {
                    query += " LIMIT " + Searching.Limit;
                }
            }

            query += ";";
            
            List<object> results = new List<object>();
            var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
            var command = new SQLiteCommand(query, connection);
            IDBReader reader = new DBReader(command.ExecuteReader());
            while (reader.Read())
            {
                if (!Searching.UseSearchCount)
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
            if (Searching.UsingOptions && Searching.UseSearchCount)
            {
                query += "COUNT(*)";
            }
            else
            {
                query += "*";
            }
            query += " FROM " + clsName;
            query += " WHERE " + Utils.GetFilterString(filters);

            if (Searching.UsingOptions && !Searching.UseSearchCount
                && Searching.Limit > -1)
            {
                if (Searching.CurrentPage > -1)
                {
                    query += " LIMIT " + Searching.Limit + ", " + (Searching.CurrentPage * Searching.Limit);
                }
                else
                {
                    query += " LIMIT " + Searching.Limit;
                }
            }

            query += ";";

            List<T> results = new List<T>();
            var connection = (SQLiteConnection)OpenConnection(ConnectionInfo);
            var command = new SQLiteCommand(query, connection);
            IDBReader reader = new DBReader(command.ExecuteReader());
            while (reader.Read())
            {
                if (!Searching.UseSearchCount)
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
