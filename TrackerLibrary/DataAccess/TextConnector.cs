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
        // Location where we are storing our files.
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamFile = "TeamModels.csv";
        private const string TournamentFile = "TournamentModels.csv";
        private const string MatchupFile = "MatchupModels.csv";
        private const string MatchupEntryFile = "MatchupEntryModels.csv";


        /// <summary>
        /// Saves a new person to a text file.
        /// </summary>
        /// <param name="person">The persons' information.</param>
        /// <returns>The person information, including their unique identifier.</returns>
        public PersonModel CreatePerson(PersonModel person)
        {
            // Getting the full file path, reading the file into a List<string>, and converting the List<string> to a List<PersonModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            int currentID = 1;

            if (people.Count > 0)
            {
                currentID = people.OrderByDescending(x => x.ID).First().ID + 1;
            }

            person.ID = currentID;

            people.Add(person);

            // Convert the people to a List<string>
            // Save the List<string> to the text file
            people.SaveToPersonFile(PeopleFile);

            return person;
        }

        /// <summary>
        /// Saves a new prize to a text file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel prize)
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

            prize.ID = currentID;

            // Add new record to the List<PrizeModel> with the new id (max + 1)
            prizes.Add(prize);

            // Convert the prizes to a List<string>
            // Save the List<string> to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return prize; 
        }

        public TeamModel CreateTeam(TeamModel team)
        {
            // Reading the teams out of the text file.
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            int currentID = 1;

            if (teams.Count > 0)
            {
                currentID = teams.OrderByDescending(x => x.ID).First().ID + 1;
            }

            team.ID = currentID;

            teams.Add(team);

            teams.SaveToTeamFile(TeamFile);

            return team;
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = TournamentFile.
                FullFilePath().
                LoadFile().
                ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);

            int currentID = 1;

            if (tournaments.Count > 0)
            {
                currentID = tournaments.OrderByDescending(x => x.ID).First().ID + 1;
            }

            model.ID = currentID;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentFile();
        }

        public List<PersonModel> GetPerson_All()
        {
            // Reading all the people out of the text file.
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();


        }

        public List<TeamModel> GetTeam_All()
        {
            return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

        }

        public List<TournamentModel> GetTournament_All()
        {
           return TournamentFile.
                    FullFilePath().
                    LoadFile().
                    ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
