using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Data.SqlClient;
using StoryDev.DBO.Scripting;

namespace StoryDev.DBO.MSSQL
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
        /// Close a Microsoft SQL database connection.
        /// </summary>
        /// <param name="con">The Microsoft SQL database connection object.</param>
        public void CloseConnection(object con)
        {
            if (con != null && con.GetType() == typeof(SqlConnection))
            {
                var casted = (SqlConnection)con;
                casted.Close();
            }
        }

        public void CreateTable(string name, Type dbType = null)
        {
            Type type = null;
            if (dbType != null)
            {
                type = dbType;
            }
            else if (ItemConstructor.ResolvedTypes.ContainsKey(name))
            {
                type = ItemConstructor.ResolvedTypes[name];
            }

            string query = "CREATE TABLE IF NOT EXISTS " + name;
            query += "(\r\n";
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                query += "\t";
                if (!Utils.IsBasicArrayType(field.FieldType))
                {
                    query += field.Name + " ";
                    query += GetSQLTypeName(field) + " ";
                    var autoIncrement = (SQLAutoIncrement)field.GetCustomAttribute(typeof(SQLAutoIncrement));
                    if (autoIncrement != null && Utils.IsNumberType(field.FieldType))
                    {
                        query += "identity(1, 1) ";
                    }

                    var primary = (SQLPrimaryKey)field.GetCustomAttribute(typeof(SQLPrimaryKey));
                    if (primary != null && Utils.IsNumberType(field.FieldType))
                    {
                        query += "primary key ";
                    }

                    query += "\r\n";
                }
                else
                {
                    var attr = (SQLArrayFormat)field.GetCustomAttribute(typeof(SQLArrayFormat));
                    if (attr.Type == ArrayType.Cascade)
                    {
                        query += field.Name + " ";
                        query += "varchar(max) ";
                        query += "\r\n";
                    }
                    else
                    {
                        if (attr.CustomSuffixes != null)
                        {
                            for (int j = 0; j < attr.ArrayMaxItems; j++)
                            {
                                query += field.Name + attr.CustomSuffixes[j] + " ";
                                query += GetSQLTypeName(field) + " ";
                                query += "\r\n";
                            }
                        }
                        else if (attr.ArrayMaxItems > 0)
                        {
                            for (int j = 0; j < attr.ArrayMaxItems; j++)
                            {
                                query += field.Name + (attr.ColumnStartIndex + j) + " ";
                                query += GetSQLTypeName(field) + " ";
                                query += "\r\n";
                            }
                        }
                    }
                }
            }

            query += ");";

            var connection = new SqlConnection(ConnectionInfo);
            var command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private string GetSQLTypeName(FieldInfo field)
        {
            var type = field.FieldType;

            if (type == typeof(bool) || type == typeof(bool[]))
                return "bit";
            else if (type == typeof(float) || type == typeof(float[]))
                return "float(24)";
            else if (type == typeof(double) || type == typeof(double[]))
                return "float(53)";
            else if (type == typeof(decimal) || type == typeof(decimal[]))
                return "decimal(38)";
            else if (type == typeof(long) || type == typeof(ulong) || type == typeof(long[]) || type == typeof(ulong[]))
                return "bigint";
            else if (type == typeof(int) || type == typeof(uint) || type == typeof(int[]) || type == typeof(uint[]))
                return "int";
            else if (type == typeof(short) || type == typeof(ushort) || type == typeof(short[]) || type == typeof(ushort[]))
                return "smallint";
            else if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(byte[]) || type == typeof(sbyte[]))
                return "tinyint";
            else if (type == typeof(DateTime) || type == typeof(DateTime[]))
            {
                var attr = (SQLDate)field.GetCustomAttribute(typeof(SQLDate));
                if (attr != null)
                {
                    if (attr.Format == SQLDateFormat.Date)
                        return "date";
                    else if (attr.Format == SQLDateFormat.DateTime)
                        return "datetime";
                    else if (attr.Format == SQLDateFormat.Time)
                        return "time";
                }
                else
                    return "datetime";
            }
            else if (type == typeof(string) || type == typeof(string[]))
            {
                var attr = (SQLStringSize)field.GetCustomAttribute(typeof(SQLStringSize));
                if (attr != null)
                {
                    if (attr.StringType == SQLStringType.Fixed)
                    {
                        int size = attr.Size;
                        if (attr.Size > 8000)
                            size = 8000;
                        else if (attr.Size < 0)
                            size = 0;

                        return "char(" + size + ")";
                    }
                    else if (attr.StringType == SQLStringType.Variable)
                    {
                        if (attr.Size == -1)
                            return "varchar(max)";
                        else
                        {
                            int size = attr.Size;
                            if (attr.Size > 8000)
                                size = 8000;
                            else if (attr.Size < 0)
                                size = 0;

                            return "varchar(" + size + ")";
                        }
                    }
                }
                else
                {
                    return "varchar(max)";
                }
            }

            return "";
        }

        public void CreateTable<T>()
        {
            var type = typeof(T);
            CreateTable("", type);
        }

        public int End()
        {
            int result = 0;
            if (IsBuilding)
            {
                string query = BulkModifier.GetAllLines();
                var connection = (SqlConnection)OpenConnection(ConnectionInfo);
                var command = new SqlCommand(query, connection);
                result = command.ExecuteNonQuery();
                IsBuilding = false;
            }
            return result;
        }

        /// <summary>
        /// Opens a Microsoft SQL connection with the given connection info string.
        /// </summary>
        /// <param name="connectionInfo">The string value to use to connect to a Microsoft SQL database.</param>
        /// <returns>The Microsoft SQL connection object.</returns>
        public object OpenConnection(string connectionInfo = "")
        {
            if (connectionInfo == "")
                connectionInfo = ConnectionInfo;

            var connection = new SqlConnection(connectionInfo);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Search a Microsoft SQL database with the given filters. This function uses
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
            var connection = (SqlConnection)OpenConnection(ConnectionInfo);
            var command = new SqlCommand(query, connection);
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
            var connection = (SqlConnection)OpenConnection(ConnectionInfo);
            var command = new SqlCommand(query, connection);
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
