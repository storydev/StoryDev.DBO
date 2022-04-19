using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace StoryDev.DBO.MSSQL
{
    public class DBReader : IDBReader
    {

        private SqlDataReader reader;

        public DBReader(SqlDataReader reader)
        {
            this.reader = reader;
        }

        public bool GetBoolean(string column)
        {
            return reader.GetBoolean(GetOrdinal(column));
        }

        public bool GetBoolean(int i)
        {
            return reader.GetBoolean(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public DateTime GetDateTime(string column)
        {
            return reader.GetDateTime(GetOrdinal(column));
        }

        public DateTime GetDateTime(int i)
        {
            return reader.GetDateTime(i);
        }

        public decimal GetDecimal(string column)
        {
            return reader.GetDecimal(GetOrdinal(column));
        }

        public decimal GetDecimal(int i)
        {
            return reader.GetDecimal(i);
        }

        public double GetDouble(string column)
        {
            return reader.GetDouble(GetOrdinal(column));
        }

        public double GetDouble(int i)
        {
            return reader.GetDouble(i);
        }

        public float GetFloat(string column)
        {
            return reader.GetFloat(GetOrdinal(column));
        }

        public float GetFloat(int i)
        {
            return reader.GetFloat(i);
        }

        public short GetInt16(string column)
        {
            return reader.GetInt16(GetOrdinal(column));
        }

        public short GetInt16(int i)
        {
            return reader.GetInt16(i);
        }

        public int GetInt32(string column)
        {
            return reader.GetInt32(GetOrdinal(column));
        }

        public int GetInt32(int i)
        {
            return reader.GetInt32(i);
        }

        public long GetInt64(string column)
        {
            return reader.GetInt64(GetOrdinal(column));
        }

        public long GetInt64(int i)
        {
            return reader.GetInt64(i);
        }

        public int GetOrdinal(string name)
        {
            return reader.GetOrdinal(name);
        }

        public string GetString(string column)
        {
            return reader.GetString(GetOrdinal(column));
        }

        public string GetString(int i)
        {
            return reader.GetString(i);
        }

        public ushort GetUInt16(string column)
        {
            var value = reader.GetInt16(GetOrdinal(column));
            if (value < 0)
                value = 0;
            return (ushort)value;
        }

        public ushort GetUInt16(int i)
        {
            var value = reader.GetInt16(i);
            if (value < 0)
                value = 0;
            return (ushort)value;
        }

        public uint GetUInt32(string column)
        {
            var value = reader.GetInt32(GetOrdinal(column));
            if (value < 0)
                value = 0;
            return (uint)value;
        }

        public uint GetUInt32(int i)
        {
            var value = reader.GetInt32(i);
            if (value < 0)
                value = 0;
            return (uint)value;
        }

        public ulong GetUInt64(string column)
        {
            var value = reader.GetInt64(GetOrdinal(column));
            if (value < 0)
                value = 0;
            return (ulong)value;
        }

        public ulong GetUInt64(int i)
        {
            var value = reader.GetInt64(i);
            if (value < 0)
                value = 0;
            return (ulong)value;
        }

        public bool Read()
        {
            return reader.Read();
        }
    }
}
