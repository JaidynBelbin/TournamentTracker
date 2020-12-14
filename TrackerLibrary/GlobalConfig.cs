using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        /// <summary>
        /// A List<IDataConnection> lets us handle multiple types of data connections, ie.
        /// to .txt files or to SQL server or even something else later on.
        /// </summary>
        public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>();

        public static void InitialiseConnections(bool database, bool textFiles)
        {
            if (database)
            {
                // TODO - Set up the SQL connection properly.
                SQLConnector sql = new SQLConnector();
                Connections.Add(sql);
            }

            if (textFiles)
            {
                // TODO - Create the text connection properly.

                TextConnector text = new TextConnector();
                Connections.Add(text);
            }
        }
    }
}
