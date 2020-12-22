using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        // A unique ID for each team.
        public int ID { get; set; }
        /// <summary>
        /// The name of the Team.
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// A List of Person's making up the Team.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

    }
}
