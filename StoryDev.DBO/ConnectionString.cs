using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoryDev.DBO.Scripting;

namespace StoryDev.DBO
{
    public class ConnectionString
    {
        /// <summary>
        /// For SQL Server
        /// </summary>
        public AuthMode AuthenticationMode;
        public string FileOrFolder;
        public DatabaseVendor Vendor;
        public string ServerOrHost;
        public ushort Port;
        public string DatabaseName;
        public string Username;
        public string Password;

        public ConnectionString()
        {

        }

        public override string ToString()
        {
            if (Vendor == DatabaseVendor.MySQL)
            {
                return string.Format("Host={0};Port={1};Database={2};Uid={3};Pwd={4}",
                    ServerOrHost, Port, DatabaseName, Username, Password);
            }
            else if (Vendor == DatabaseVendor.Microsoft)
            {
                if (AuthenticationMode == AuthMode.Sql)
                    return string.Format("Server={0},{1};Database={2};User Id={3};Password={4};TrustServerCertificate=Yes;",
                        ServerOrHost, Port, DatabaseName, Username, Password);
                else if (AuthenticationMode == AuthMode.Windows)
                    return string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=true; TrustServerCertificate=Yes;",
                        ServerOrHost, DatabaseName);
            }
            else if (Vendor == DatabaseVendor.SQLite)
            {
                return string.Format("Data Source={0}; Version=3;", FileOrFolder);
            }

            return "";
        }

    }

    public enum AuthMode
    {
        Windows,
        Sql
    }
}
