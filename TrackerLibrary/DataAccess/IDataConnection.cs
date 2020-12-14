using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        // Method called when the Create Prize button is clicked
        // TODO - Probably will add methods to create TeamModel, TournamentModel, etc.
        PrizeModel CreatePrize(PrizeModel model);

    }
}
