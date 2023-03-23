using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core.MySQL
{
    public class DBReader : IDBReader
    {

        private MySqlDataReader reader;

        public DBReader(MySqlDataReader reader)
        {
            this.reader = reader;
        }

        public bool GetBoolean(string column)
        {
            return reader.GetBoolean(column);
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
            return reader.GetDateTime(column);
        }

        public DateTime GetDateTime(int i)
        {
            return reader.GetDateTime(i);
        }

        public decimal GetDecimal(string column)
        {
            return reader.GetDecimal(column);
        }

        public decimal GetDecimal(int i)
        {
            return reader.GetDecimal(i);
        }

        public double GetDouble(string column)
        {
            return reader.GetDouble(column);
        }

        public double GetDouble(int i)
        {
            return reader.GetDouble(i);
        }

        public float GetFloat(string column)
        {
            return reader.GetFloat(column);
        }

        public float GetFloat(int i)
        {
            return reader.GetFloat(i);
        }

        public short GetInt16(string column)
        {
            return reader.GetInt16(column);
        }

        public short GetInt16(int i)
        {
            return reader.GetInt16(i);
        }

        public int GetInt32(string column)
        {
            return reader.GetInt32(column);
        }

        public int GetInt32(int i)
        {
            return reader.GetInt32(i);
        }

        public long GetInt64(string column)
        {
            return reader.GetInt64(column);
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
            return reader.GetString(column);
        }

        public string GetString(int i)
        {
            return reader.GetString(i);
        }

        public ushort GetUInt16(string column)
        {
            return reader.GetUInt16(column);
        }

        public ushort GetUInt16(int i)
        {
            return reader.GetUInt16(i);
        }

        public uint GetUInt32(string column)
        {
            return reader.GetUInt32(column);
        }

        public uint GetUInt32(int i)
        {
            return reader.GetUInt32(i);
        }

        public ulong GetUInt64(string column)
        {
            return reader.GetUInt64(column);
        }

        public ulong GetUInt64(int i)
        {
            return reader.GetUInt64(i);
        }

        public bool Read()
        {
            return reader.Read();
        }
    }
}
