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

        // Locations where the files are stored.
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchupFile = "MatchupModels.csv";
        public const string MatchupEntryFile = "MatchupEntryModels.csv";

        // Connection is handled by either the SQLConnector or TextConnector class, depending on what the user chooses.
        public static IDataConnection Connection { get; private set; }

        // Called in Program.cs when the program starts up, and initialises either the SQL
        // connection or a text connection, depending on what the user chooses.
        public static void InitialiseConnections(DatabaseType db)
        {
            if (db == DatabaseType.SQL)
            {
                // Creating the SQL connection
                Connection = new SQLConnector();

            }

            else if (db == DatabaseType.Textfile)
            {
                // Creating the text connection
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

        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
