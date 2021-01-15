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
        /// The unique identifier for the match.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The set of teams involved in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// The ID from the database used to identify the winner.
        /// </summary>
        public int WinnerID { get; set; }

        /// <summary>
        /// Representing the Team that won this particular matchup.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The round # of this particular matchup
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// Property that holds the display name of the match. 
        /// </summary>
        public string DisplayName
        {
            get
            {
                string output = "";

                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }
                }

                return output;
            }
        }
    }
}
