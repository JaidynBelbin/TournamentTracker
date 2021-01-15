using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;

namespace TrackerUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialise the 2 database connections, assuming the user want to write to
            // both a database and a text file.

            // Now initialising a TextFile connection instead of SQL
            TrackerLibrary.GlobalConfig.InitialiseConnections(DatabaseType.Textfile);

            Application.Run(new TournamentDashboardForm()); // for testing
        }
    }
}
