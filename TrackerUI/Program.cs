using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            // Initialise the database connections
            TrackerLibrary.GlobalConfig.InitialiseConnections(true, true);

            Application.Run(new CreatePrizeForm()); // for testing
            //Application.Run(new TournamentDashboardForm());
        }
    }
}
