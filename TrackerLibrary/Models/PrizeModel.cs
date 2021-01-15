using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the prize
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Number representing the position the person came in.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// A string representing the actual name of the position, ie.
        /// "Champion" or "First runner-up", etc.
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// A decimal amount representing the amount of prize money the
        /// winner/s get/s.
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// An optional percentage of the total tournament revenue
        /// in the event that the actual number of players is less
        /// than expected.
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }


        /// <summary>
        /// Overloaded constructor to parse all the number values to strings.
        /// </summary>
        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}
