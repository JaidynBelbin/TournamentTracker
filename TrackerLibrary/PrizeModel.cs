using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    public class PrizeModel
    {
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

    }
}
