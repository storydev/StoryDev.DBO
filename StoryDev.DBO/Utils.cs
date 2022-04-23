﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace StoryDev.DBO
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
                    return (float)value == (float)match;
                else if (value.GetType() == typeof(decimal))
                    return (decimal)value == (decimal)match;
                else if (value.GetType() == typeof(int))
                    return (int)value == (int)match;
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
                sqlSignatures.Add(typeof(string), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetString(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(short), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetInt16(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(int), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetInt32(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(long), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetInt64(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(ushort), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetUInt16(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(uint), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetUInt32(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(ulong), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetUInt64(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(float), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetFloat(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(decimal), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetDecimal(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(byte[]), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { reader.GetBytes(reader.GetOrdinal(info.Name), 0, (byte[])info.GetValue(obj), 0, ((byte[])info.GetValue(obj)).Length); } catch { }
                });
                sqlSignatures.Add(typeof(bool), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetBoolean(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(double), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetDouble(info.Name)); } catch { }
                });
                sqlSignatures.Add(typeof(DateTime), (FieldInfo info, object obj, IDBReader reader) =>
                {
                    try { info.SetValue(obj, reader.GetDateTime(info.Name)); } catch { }
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

        public static string GenerateDelete(string tableName, int ID)
        {
            string result = "DELETE FROM " + tableName;
            result += " WHERE ID = " + ID;

            return result;
        }

        public static string GenerateInsert(string tableName, FieldInfo[] fields, bool requiresReturning = false)
        {
            var query = "INSERT INTO " + tableName + " (";

            var firstValue = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;

                if (!firstValue)
                {
                    query += ", ";
                }

                query += field.Name;
                firstValue = false;
            }
            query += ") ";

            query += "VALUES (";
            var firstValue1 = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;

                if (!firstValue1)
                {
                    query += ", ";
                }

                query += "@" + field.Name;
                firstValue1 = false;
            }
            query += ")";

            if (requiresReturning)
                query += " RETURNING ID";

            query += ";";

            return query;
        }

        public static string GenerateUpdate(string tableName, FieldInfo[] fields, int ID, DBFilter[] filters = null)
        {
            var query = "UPDATE " + tableName + " SET ";
            
            var isFirstValue = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Name == "ID" || field.IsInitOnly)
                    continue;

                if (!isFirstValue)
                {
                    query += ", ";
                }

                query += field.Name + " = @" + field.Name;

                isFirstValue = false;
            }

            if (filters != null)
                query += " WHERE " + GetFilterString(filters);
            else
                query += " WHERE ID = " + ID;

            return query;
        }

    }
}
