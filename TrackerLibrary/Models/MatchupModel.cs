using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one match in the tournament.
    /// </summary>
    
    public class MatchupModel
    {
        /// <summary>
        /// The set of teams involved in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// Representing the Team that won this particular matchup.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The round # of this particular matchup
        /// </summary>
        public int MatchupRound { get; set; }

    }
}
