using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Oracle.ManagedDataAccess.Client;
using StoryDev.DBO.Scripting;

namespace StoryDev.DBO.Oracle
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
            if (con != null && con.GetType() == typeof(OracleConnection))
            {
                var casted = (OracleConnection)con;
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
                var connection = (OracleConnection)OpenConnection(ConnectionInfo);
                var command = new OracleCommand(query, connection);
                result = command.ExecuteNonQuery();
                IsBuilding = false;
            }
            return result;
        }

        /// <summary>
        /// Opens an Oracle connection with the given connection info string.
        /// </summary>
        /// <param name="connectionInfo">The string value to use to connect to an Oracle database.</param>
        /// <returns>The Oracle connection object.</returns>
        public object OpenConnection(string connectionInfo = "")
        {
            if (connectionInfo == "")
                connectionInfo = ConnectionInfo;

            var connection = new OracleConnection(connectionInfo);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Search an Oracle database with the given filters. This function uses
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
            var connection = (OracleConnection)OpenConnection(ConnectionInfo);
            var command = new OracleCommand(query, connection);
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
            var connection = (OracleConnection)OpenConnection(ConnectionInfo);
            var command = new OracleCommand(query, connection);
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
