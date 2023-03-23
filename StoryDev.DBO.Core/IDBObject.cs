using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    public interface IDBObject
    {

        void Insert();
        void Update(params DBFilter[] filters);
        void Delete();

    }
}
