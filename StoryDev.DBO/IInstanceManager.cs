using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoryDev.DBO.Scripting;

namespace StoryDev.DBO
{
    public interface IInstanceManager
    {

        DataStruct StructReference { get; set; }

        Dictionary<string, string> SourcePath { get; }

        string ConnectionInfo { get; set; }

        Dictionary<string, List<object>> Items { get; }

        int LastInsertedID { get; }

        IEnumerable<object> Search(string name, params DBFilter[] filters);

        IEnumerable<T> Search<T>(params DBFilter[] filters);

        object OpenConnection(string connectionInfo);

        void CloseConnection(object connection);

        void Begin();

        int End();

    }
}
