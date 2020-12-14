using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        
        public static IDataConnection Connection { get; private set; }

        // Called in Program.cs when the program starts up.
        public static void InitialiseConnections(DatabaseType db)
        {
            if (db == DatabaseType.SQL)
            {
                // TODO - Set up the SQL connection properly.
                SQLConnector sql = new SQLConnector();
                Connection = (sql);
            }

            else if (db == DatabaseType.Textfile)
            {
                // TODO - Create the text connection properly.

                TextConnector text = new TextConnector();
                Connection = (text);
            }
        }

        /// <summary>
        /// Goes into the app.config file and retrives the connection string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
