using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    public interface IInstanceManager
    {

        Dictionary<string, string> SourcePath { get; }

        string ConnectionInfo { get; set; }

        Dictionary<string, List<object>> Items { get; }

        int LastInsertedID { get; }

        void CreateTable(string name, Type dbType = null);

        void CreateTable<T>();

        IEnumerable<object> Search(string name, params DBFilter[] filters);

        IEnumerable<T> Search<T>(params DBFilter[] filters);

        object OpenConnection(string connectionInfo = "");

        void CloseConnection(object connection);

        void Begin();

        int End();

    }
}
