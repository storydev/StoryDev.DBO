using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Scripting
{
    /// <summary>
    /// Used internally by StoryDev Data Studio for scripting.
    /// </summary>
    public class DataStruct
    {

        public string Name;
        public List<DataField> Fields;
        public string SourceFile;
        public string DefinedFormName;

        public DataStruct()
        {
            Fields = new List<DataField>();
        }

    }
}
