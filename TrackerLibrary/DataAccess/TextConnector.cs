using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // Location where we are storing our file.
        private const string PrizesFile = "PrizeModels.csv";

        /// <summary>
        /// Saves a new prize to a text file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Chaining all our extension methods defined in TextConnectorProcessor to do all
            // the work in one go.

            // Load the text file
            // Convert the text to a List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Finds the highest ID in the list, and adds 1 to it to make the new ID,
            // since text files do not increment IDs for us.
            int currentID = 1;

            // If there are no prizes, we begin with an ID of 1, else, we increment it and assign it
            // to the next prize
            if (prizes.Count > 0)
            {
                currentID = prizes.OrderByDescending(x => x.ID).First().ID + 1;
            }

            model.ID = currentID;

            // Add new record to the List<PrizeModel> with the new id (max + 1)
            prizes.Add(model);

            // Convert the prizes to a List<string>
            // Save the List<string> to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model; 
        }
    }
}
