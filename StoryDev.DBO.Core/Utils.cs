using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    public class Utils
    {

        public static void PushArray<T>(ref T[] arr, T instance)
        {
            T[] temp = new T[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
            {
                temp[i] = arr[i];
            }
            temp[arr.Length] = instance;
            arr = temp;
        }

        public static void PushArray<T>(ref T[] arr, T[] toAdd)
        {
            int total = arr.Length + toAdd.Length;
            T[] temp = new T[total];
            for (int i = 0; i < total; i++)
            {
                if (i > arr.Length - 1)
                {
                    temp[i] = toAdd[i - arr.Length];
                }
                else
                {
                    temp[i] = arr[i];
                }
            }
            arr = temp;
        }

        public static bool GetResultOf(object value, DBOperator op, object match)
        {
            if (op == DBOperator.Begins)
            {
                return ((string)value).StartsWith((string)match);
            }
            else if (op == DBOperator.Ends)
            {
                return ((string)value).EndsWith((string)match);
            }
            else if (op == DBOperator.Contains)
            {
                return ((string)value).Contains((string)match);
            }
            else if (op == DBOperator.Equals)
            {
                if (value.GetType() == typeof(float))
                    return (float)value == (float)Convert.ToSingle(match);
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value == (decimal)Convert.ToDecimal(match);
                else if (value.GetType() == typeof(int))
                    return (int)value == (int)Convert.ToInt32(match);
                else if (value.GetType() == typeof(long))
                    return (long)value == (long)Convert.ToInt64(match);
                else
                    return value.Equals(match);
            }
            else if (op == DBOperator.GreaterThan)
            {
                if (value.GetType() == typeof(float))
                    return (float)value > (float)match;
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value > (decimal)match;
                else if (value.GetType() == typeof(int))
                    return (int)value > (int)match;
            }
            else if (op == DBOperator.GreaterThanEquals)
            {
                if (value.GetType() == typeof(float))
                    return (float)value >= (float)match;
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value >= (decimal)match;
                else if (value.GetType() == typeof(int))
                    return (int)value >= (int)match;
            }
            else if (op == DBOperator.LessThan)
            {
                if (value.GetType() == typeof(float))
                    return (float)value < (float)match;
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value < (decimal)match;
                else if (value.GetType() == typeof(int))
                    return (int)value < (int)match;
            }
            else if (op == DBOperator.LessThanEquals)
            {
                if (value.GetType() == typeof(float))
                    return (float)value <= (float)match;
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value <= (decimal)match;
                else if (value.GetType() == typeof(int))
                    return (int)value <= (int)match;
            }
            else
            {
                if (value.GetType() == typeof(bool))
                    return value == match;
                else
                    return false;
            }

            return false;
        }

        static Dictionary<Type, Signature> sqlSignatures;
        public static IDictionary<Type, Signature> SqlSignatures => sqlSignatures;

        public static void InitSignatures()
        {
            if (sqlSignatures == null)
            {
                sqlSignatures = new Dictionary<Type, Signature>();
                sqlSignatures.Add(typeof(string), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetString(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetString(info.Property.Name));
                        }
                    } catch { }
                });
                sqlSignatures.Add(typeof(short), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetInt16(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        { 
                            info.Property.SetValue(obj, reader.GetInt16(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(int), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetInt32(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetInt32(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(long), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetInt64(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetInt64(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(ushort), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetUInt16(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetUInt16(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(uint), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetUInt32(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetUInt32(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(ulong), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetUInt64(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetUInt64(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(float), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetFloat(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetFloat(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(decimal), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetDecimal(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetDecimal(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(byte[]), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            reader.GetBytes(reader.GetOrdinal(info.Field.Name), 0, (byte[])info.Field.GetValue(obj), 0, ((byte[])info.Field.GetValue(obj)).Length);
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            reader.GetBytes(reader.GetOrdinal(info.Property.Name), 0, (byte[])info.Property.GetValue(obj), 0, ((byte[])info.Property.GetValue(obj)).Length);
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(bool), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetBoolean(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetBoolean(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(double), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetDouble(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetDouble(info.Property.Name));
                        }
                    }
                    catch { }
                });
                sqlSignatures.Add(typeof(DateTime), (FieldRef info, object obj, IDBReader reader) =>
                {
                    try
                    {
                        if (info.Scope == FieldScope.Field)
                        {
                            info.Field.SetValue(obj, reader.GetDateTime(info.Field.Name));
                        }
                        else if (info.Scope == FieldScope.Property)
                        {
                            info.Property.SetValue(obj, reader.GetDateTime(info.Property.Name));
                        }
                    }
                    catch { }
                });
            }
        }

        public static string GetFilterString(params DBFilter[] filters)
        {
            string result = "";
            for (int i = 0; i < filters.Length; i++)
            {
                var f = filters[i];
                result += GetFilterStringEquivalent(f) + " ";
                if (f.NextCondition == DBCondition.And && i != filters.Length - 1)
                {
                    result += "AND ";
                }
                else if (f.NextCondition == DBCondition.Or && i != filters.Length - 1)
                {
                    result += "OR ";
                }
            }
            return result;
        }

        private static string GetFilterStringEquivalent(DBFilter f)
        {
            var result = "";

            result += f.FieldName;
            var type = f.ConditionValue.GetType();

            if (!IsBasicType(type))
            {
                return null;
            }

            var value = Convert.ToString(f.ConditionValue);
            if (type == typeof(string))
            {
                var reg = new Regex(@"[\x00\x0A\x0D\x1A\x22\x25\x27\x5C\x5F]");
                value = reg.Replace(value, "\\\0");
            }

            switch (f.Operator)
            {
                case DBOperator.Equals:
                    {
                        if (type == typeof(string))
                            result += " = \"" + value + "\"";
                        else
                            result += " = " + value;
                    }
                    break;
                case DBOperator.GreaterThan:
                    {
                        result += " > " + value;
                        if (type == typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.GreaterThanEquals:
                    {
                        result += " >= " + value;
                        if (type == typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.LessThanEquals:
                    {
                        result += " <= " + value;
                        if (type == typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.LessThan:
                    {
                        result += " < " + value;
                        if (type == typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.Begins:
                    {
                        result += " LIKE \"" + value + "%\"";
                        if (type != typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.Ends:
                    {
                        result += " LIKE \"%" + value + "\"";
                        if (type != typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
                case DBOperator.Contains:
                    {
                        result += " LIKE \"%" + value + "%\"";
                        if (type != typeof(string))
                        {
                            return null;
                        }
                    }
                    break;
            }

            return result;
        }

        private static bool IsBasicType(Type type)
        {
            var result = (type == typeof(string) || type == typeof(bool) || type == typeof(DateTime)
                || type == typeof(int) || type == typeof(short) || type == typeof(long)
                || type == typeof(uint) || type == typeof(ushort) || type == typeof(ulong)
                || type == typeof(float) || type == typeof(double) || type == typeof(decimal));
            return result;
        }

        public static bool IsBasicArrayType(Type type)
        {
            var result = (type == typeof(string[]) || type == typeof(bool[]) || type == typeof(DateTime[])
                || type == typeof(int[]) || type == typeof(short[]) || type == typeof(long[])
                || type == typeof(uint[]) || type == typeof(ushort[]) || type == typeof(ulong[])
                || type == typeof(float[]) || type == typeof(double[]) || type == typeof(decimal[]));
            return result;
        }

        public static bool IsNumberType(Type type)
        {
            var result = type == typeof(int) || type == typeof(short) || type == typeof(long)
                || type == typeof(uint) || type == typeof(ushort) || type == typeof(ulong)
                || type == typeof(byte) || type == typeof(sbyte);
            return result;
        }

        public static string GenerateDelete(string tableName, object ID)
        {
            string result = "DELETE FROM " + tableName;
            result += " WHERE ID = " + ID.ToString();

            return result;
        }

        public static string GenerateInsert(string tableName, FieldRef[] fields, string primaryKeyName, bool requiresReturning = false)
        {
            var query = "INSERT INTO " + tableName + " (";

            var firstValue = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Scope == FieldScope.Field)
                {
                    if (field.Field.Name == primaryKeyName || field.Field.IsInitOnly)
                        continue;

                    if (!firstValue)
                    {
                        query += ", ";
                    }

                    query += field.Field?.Name;
                    firstValue = false;
                }
                else if (field.Scope == FieldScope.Property)
                {
                    if (field.Property.Name == primaryKeyName || !field.Property.CanRead)
                        continue;

                    if (!firstValue)
                    {
                        query += ", ";
                    }

                    query += field.Property?.Name;
                    firstValue = false;
                }

            }
            query += ") ";

            query += "VALUES (";
            var firstValue1 = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Scope == FieldScope.Field)
                {
                    if (field.Field.Name == primaryKeyName || field.Field.IsInitOnly)
                        continue;
                    if (!firstValue1)
                    {
                        query += ", ";
                    }
                    query += "@" + field.Field?.Name;
                    firstValue1 = false;
                }
                else if (field.Scope == FieldScope.Property)
                {
                    if (field.Property.Name == primaryKeyName || !field.Property.CanRead)
                        continue;
                    if (!firstValue1)
                    {
                        query += ", ";
                    }
                    query += "@" + field.Property?.Name;
                }

                firstValue1 = false;
            }
            query += ")";

            if (requiresReturning)
                query += " RETURNING " + primaryKeyName;

            query += ";";

            return query;
        }

        public static string GenerateUpdate(string tableName, FieldRef[] fields, object ID, string primaryKeyName, DBFilter[] filters = null)
        {
            var query = "UPDATE " + tableName + " SET ";

            var isFirstValue = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Scope == FieldScope.Field)
                {
                    if (field.Field.Name == primaryKeyName || field.Field.IsInitOnly)
                        continue;

                    if (!isFirstValue)
                    {
                        query += ", ";
                    }

                    query += field.Field.Name + " = @" + field.Field.Name;
                }
                else if (field.Scope == FieldScope.Property)
                {
                    if (field.Property.Name == primaryKeyName || !field.Property.CanRead)
                        continue;

                    if (!isFirstValue)
                    {
                        query += ", ";
                    }

                    query += field.Property.Name + " = @" + field.Property.Name;
                }
            }

            if (filters != null && filters.Length > 0)
                query += " WHERE " + GetFilterString(filters);
            else
                query += " WHERE ID = " + ID.ToString();

            return query;
        }

        public static string GetSQLType(Type type, DatabaseVendor vendor)
        {
            var stringSize = (SQLStringSize)type.GetCustomAttribute(typeof(SQLStringSize));

            if (type == typeof(string))
            {
                var length = 255;
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "TEXT";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    if (stringSize != null)
                    {
                        if (stringSize.Size > 0)
                            length = stringSize.Size;

                        if (stringSize.StringType == SQLStringType.Variable)
                            return "VARCHAR(" + length + ")";
                        else if (stringSize.StringType == SQLStringType.Fixed)
                        {
                            if (stringSize.Size == (int)SQLStringSizeFormat.Tiny)
                                return "TINYTEXT";
                            else if (stringSize.Size == (int)SQLStringSizeFormat.Normal)
                                return "TEXT";
                            else if (stringSize.Size == (int)SQLStringSizeFormat.Medium)
                                return "MEDIUMTEXT";
                            else if (stringSize.Size == (int)SQLStringSizeFormat.Long)
                                return "LONGTEXT";
                        }
                    }
                }
                
            }
            else if (type == typeof(sbyte) || type == typeof(byte))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "INTEGER";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 3;
                    var unsigned = "";

                    if (type == typeof(byte))
                        unsigned = "UNSIGNED ";

                    return "TINYINT(" + length + ")" + unsigned;
                }

            }
            else if (type == typeof(short) || type == typeof(ushort))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "INTEGER";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 5;
                    var unsigned = "";

                    if (type == typeof(ushort))
                        unsigned = " UNSIGNED";

                    return "SMALLINT(" + length + ")" + unsigned;
                }

            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "INTEGER";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 10;
                    var unsigned = "";

                    if (type == typeof(uint))
                        unsigned = " UNSIGNED";

                    return "INT(" + length + ")" + unsigned;
                }

            }
            else if (type == typeof(long) || type == typeof(ulong))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "INTEGER";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 20;
                    var unsigned = "";

                    if (type == typeof(ulong))
                        unsigned = " UNSIGNED";

                    return "BIGINT(" + length + ")" + unsigned;
                }

            }
            else if (type == typeof(decimal))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "REAL";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 65;

                    return "DECIMAL(" + length + ") ";
                }
            }
            else if (type == typeof(float))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "REAL";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 24;

                    return "FLOAT(" + length + ") ";
                }

            }
            else if (type == typeof(double))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "REAL";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var length = 53;

                    if (length > 0 && length < 25)
                        return "FLOAT(" + length + ") ";
                    else
                        return "DOUBLE(" + length + ") ";
                }

            }
            else if (type == typeof(DateTime))
            {
                if (vendor == DatabaseVendor.SQLite)
                {
                    return "TEXT";
                }
                else if (vendor == DatabaseVendor.MySQL)
                {
                    var option = type.GetCustomAttribute<SQLDate>();
                    if (option == null)
                        throw new Exception("DateTime type must have SQLDate attribute.");

                    if (option.Format == SQLDateFormat.Date)
                    {
                        return "DATE ";
                    }
                    else if (option.Format == SQLDateFormat.DateTime)
                    {
                        return "DATETIME ";
                    }
                    else if (option.Format == SQLDateFormat.Time)
                    {
                        return "TIME ";
                    }
                }

            }

            return "";
        }
    }
}
