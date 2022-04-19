using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace StoryDev.DBO.SQLite
{
    public class DBReader : IDBReader
    {

        private SQLiteDataReader reader;
        
        public DBReader(SQLiteDataReader reader)
        {
            this.reader = reader;
        }

        public string GetString(string column)
        {
            return reader.GetString(GetOrdinal(column));
        }

        public string GetString(int i)
        {
            return reader.GetString(i);
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

        public ushort GetUInt16(string column)
        {
            var result = reader.GetInt16(GetOrdinal(column));
            if (result < 0)
                result = 0;

            return (ushort)result;
        }

        public ushort GetUInt16(int i)
        {
            var result = reader.GetInt16(i);
            if (result < 0)
                result = 0;

            return (ushort)result;
        }

        public uint GetUInt32(string column)
        {
            var result = reader.GetInt32(GetOrdinal(column));
            if (result < 0)
                result = 0;

            return (uint)result;
        }

        public uint GetUInt32(int i)
        {
            var result = reader.GetInt32(i);
            if (result < 0)
                result = 0;

            return (uint)result;
        }

        public ulong GetUInt64(string column)
        {
            var result = reader.GetInt64(GetOrdinal(column));
            if (result < 0)
                result = 0;

            return (uint)result;
        }

        public ulong GetUInt64(int i)
        {
            var result = reader.GetInt64(i);
            if (result < 0)
                result = 0;

            return (uint)result;
        }

        public float GetFloat(string column)
        {
            return reader.GetFloat(GetOrdinal(column));
        }

        public float GetFloat(int i)
        {
            return reader.GetFloat(i);
        }

        public decimal GetDecimal(string column)
        {
            return reader.GetDecimal(GetOrdinal(column));
        }

        public decimal GetDecimal(int i)
        {
            return reader.GetDecimal(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public int GetOrdinal(string name)
        {
            return reader.GetOrdinal(name);
        }

        public bool GetBoolean(string column)
        {
            return reader.GetBoolean(GetOrdinal(column));
        }

        public bool GetBoolean(int i)
        {
            return reader.GetBoolean(i);
        }

        public double GetDouble(string column)
        {
            return reader.GetDouble(GetOrdinal(column));
        }

        public double GetDouble(int i)
        {
            return reader.GetDouble(i);
        }

        public DateTime GetDateTime(string column)
        {
            return reader.GetDateTime(GetOrdinal(column));
        }

        public DateTime GetDateTime(int i)
        {
            return reader.GetDateTime(i);
        }

        public bool Read()
        {
            return reader.Read();
        }

    }
}
