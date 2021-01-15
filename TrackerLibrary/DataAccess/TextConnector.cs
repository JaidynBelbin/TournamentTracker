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
  
        /// <summary>
        /// Saves a new person to a text file.
        /// </summary>
        /// <param name="person">The persons' information.</param>
        /// <returns>The person information, including their unique identifier.</returns>
        public void CreatePerson(PersonModel person)
        {
            // Getting the full file path, reading the file into a List<string>, and converting the List<string> to a List<PersonModel>
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            int currentID = 1;

            if (people.Count > 0)
            {
                currentID = people.OrderByDescending(x => x.ID).First().ID + 1;
            }

            person.ID = currentID;

            people.Add(person);

            // Convert the people to a List<string>
            // Save the List<string> to the text file
            people.SaveToPeopleFile();
        }

        /// <summary>
        /// Saves a new prize to a text file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel prize)
        {

            // Load the text file
            // Convert the text to a List<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
 
            // Finds the highest ID in the list, and adds 1 to it to make the new ID,
            // since text files do not increment IDs for us.
            int currentID = 1;

            if (prizes.Count > 0)
            {
                currentID = prizes.OrderByDescending(x => x.ID).First().ID + 1;
            }

            prize.ID = currentID;

            // Add new record to the List<PrizeModel> with the new id (max + 1)
            prizes.Add(prize);

            // Convert the prizes to a List<string>
            // Save the List<string> to the text file
            prizes.SaveToPrizeFile();
        }

        public void CreateTeam(TeamModel team)
        {
            // Reading the teams out of the text file.
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();

            int currentID = 1;

            if (teams.Count > 0)
            {
                currentID = teams.OrderByDescending(x => x.ID).First().ID + 1;
            }

            team.ID = currentID;

            teams.Add(team);

            teams.SaveToTeamFile();
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.ID).First().ID + 1;
            }

            model.ID = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentFile();

            TournamentLogic.UpdateTournamentResults(model);
        }

        public List<PersonModel> GetPerson_All()
        {
            // Reading all the people out of the text file.
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournament_All()
        {
           return GlobalConfig.TournamentFile.
                    FullFilePath().
                    LoadFile().
                    ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
