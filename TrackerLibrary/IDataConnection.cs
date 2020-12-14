using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    public interface IDataConnection
    {
        // Method called when the Create Prize button is clicked
        PrizeModel CreatePrize(PrizeModel model);

    }
}
