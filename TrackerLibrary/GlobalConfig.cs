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
        
        // Connection is either the SQLConnector class or the TextConnector class depending on what the user choooses.
        public static IDataConnection Connection { get; private set; }

        // Called in Program.cs when the program starts up, and initialises either the SQL
        // connection or a text connection, depending on what the user chooses.
        public static void InitialiseConnections(DatabaseType db)
        {
            if (db == DatabaseType.SQL)
            {
                // Create an SQL connection and run SQLConnector.CreatePrize()
                // SQLConnector sql = new SQLConnector();
                Connection = new SQLConnector();

            }

            else if (db == DatabaseType.Textfile)
            {
                // Create a text connection and run TextConnector.CreatePrize()
                // TextConnector text = new TextConnector();
                Connection = new TextConnector();
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
