using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        public int ID { get; set; }


        public int TeamCompetingID { get; set; }

        /// <summary>
        /// Represents one team in a matchup of two teams.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// Represents the score for this particular team
        /// </summary>
        public double Score { get; set; }

        public int ParentMatchupID { get; set; }

        /// <summary>
        /// Represents the matchup that this team came
        /// from as the winner, so we can trace a winning team
        /// back to their original matchup.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }

    }
}
