using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    public interface IBulkQuery
    {

        void Execute(string query);

    }
}
