using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    public interface IDBReader
    {

        string GetString(string column);

        string GetString(int i);

        short GetInt16(string column);

        short GetInt16(int i);

        int GetInt32(string column);

        int GetInt32(int i);

        long GetInt64(string column);

        long GetInt64(int i);

        ushort GetUInt16(string column);

        ushort GetUInt16(int i);

        uint GetUInt32(string column);

        uint GetUInt32(int i);

        ulong GetUInt64(string column);

        ulong GetUInt64(int i);

        float GetFloat(string column);

        float GetFloat(int i);

        decimal GetDecimal(string column);

        decimal GetDecimal(int i);

        long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

        int GetOrdinal(string name);

        bool GetBoolean(string column);

        bool GetBoolean(int i);

        double GetDouble(string column);

        double GetDouble(int i);

        DateTime GetDateTime(string column);

        DateTime GetDateTime(int i);

        bool Read();

    }
}
