using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{

    // TODO - Lots of refactoring to be done here, combining similar methods into a more generic one
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Extension method that takes in a file name and returns the entire file path.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FullFilePath(this string fileName) // PrizeModels.csv
        {
            // Returns C:\data\TournamentTracker\PrizeModels.csv
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName } ";
        } 

        /// <summary>
        /// Extension method that takes in a full file path and retruns a List<string>
        /// containing each entry in the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> LoadFile(this string file)
        {
      
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            // If the file does exist, return a List<string> containing all the lines
            // of the text file.
            return File.ReadAllLines(file).ToList();
        }



        // CONVERTING STRINGS TO MODELS //


        /// <summary>
        /// Extension method that takes in a List<string> and converts it to a
        /// List<PrizeModel>.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                // Splitting the file at the commas (since a csv file)
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();

                // Attempting to assign the separated lines to the members of the PrizeModel
                p.ID = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);

                // Adding p to the List<PrizeModels>
                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Extension method that takes in a List<string> and converts it to a
        /// List<PersonModel>.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                // Splitting each string at the commas and putting the values into an array.
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();

                // Filling out a PersonModel with the values from the array.
                p.ID = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];

                output.Add(p);
            }
            
            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            // team id, team name, list of people id's separated by a pipe, eg.
            // 3, Red Team, 1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                // Getting the team ID and name into a new TeamModel
                TeamModel t = new TeamModel();
                t.ID = int.Parse(cols[0]);
                t.TeamName = cols[1];

                // Putting each of the team member IDs into a separate array.
                string[] personIDs = cols[2].Split('|');

                // Take the List of people from the textfile and search for where the id of the Person
                // in the List equals each id in the personIDs array and add them to the List<PersonModel>
                // in the TeamModel. 
                // Returns null if it cannot find the matching person.
                foreach (string id in personIDs)
                {
                    t.TeamMembers.Add(people.Where(x => x.ID == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
        {

            List<TournamentModel> output = new List<TournamentModel>();

            // Pulling the teams, prizes and matchups from the textfile.
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel t = new TournamentModel()
                {
                    ID = int.Parse(cols[0]),
                    TournamentName = cols[1],
                    EntryFee = decimal.Parse(cols[2])
                };

                string[] teamIDs = cols[3].Split('|');

                foreach(string id in teamIDs)
                {
                    // Looks through the list of Teams and provides the ones where the id's match
                    // the id's of the Teams in our TournamentModel.

                    t.EnteredTeams.Add(teams.Where(x => x.ID == int.Parse(id)).First());
                }

                if (cols[4].Length > 0)
                {
                    string[] prizeIDs = cols[4].Split('|');

                    foreach (string id in prizeIDs)
                    {
                        // Looks through the list of Prizes and provides the ones where the id's match
                        // the id's of the Prizes in our TournamentModel.

                        t.Prizes.Add(prizes.Where(x => x.ID == int.Parse(id)).First());
                    } 
                }

                // Splitting the rounds into each set of matchups (id^id^id)
                string[] rounds = cols[5].Split('|');

                
                foreach (string round in rounds)
                {
                    // Creating a new List<Matches> for each round
                    List<MatchupModel> matches = new List<MatchupModel>();
                    string[] matchupIDs = round.Split('^');

                    // Adding the matchups to 
                    foreach (string id in matchupIDs)
                    {
                        matches.Add(matchups.Where(x => x.ID == int.Parse(id)).First());
                    }

                    t.Rounds.Add(matches);
                }

                output.Add(t);
            }

            return output;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupModel m = new MatchupModel();

                m.ID = int.Parse(cols[0]);
                m.Entries = ConvertStringToMatchupEntryModel(cols[1]);

                // Error handling for if we haven't set a winner yet.
                if (cols[2].Length == 0)
                {
                    m.Winner = null;

                } else
                {
                    m.Winner = LookupTeamByID(int.Parse(cols[2]));
                }

                m.MatchupRound = int.Parse(cols[3]);
                
                output.Add(m);
            }

            return output;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupEntryModel p = new MatchupEntryModel();

                p.ID = int.Parse(cols[0]);

                // Error handling in case there are no teams competing that round, eg. maybe
                // the last round.
                if (cols[1].Length == 0)
                {
                    p.TeamCompeting = null;

                } else
                {
                    p.TeamCompeting = LookupTeamByID(int.Parse(cols[1]));
                }

                p.Score = double.Parse(cols[2]);
                
                // Checking if there is a parent matchup or not, if not, put null.
                // ie. for the first round.
    
                if (int.TryParse(cols[3], out int parentID)) {

                    p.ParentMatchup = LookupMatchupByID(parentID);

                } else
                {
                    p.ParentMatchup = null;
                }

                output.Add(p);

            }

            return output;
        }



        // SAVE METHODS //

        public static void SaveToPersonFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();

            // Pulling the members of each PrizeModel out and concatenating them into a
            // string and adding that line to a List<string>.
            foreach (PersonModel p in models)
            {
                lines.Add($"{p.ID},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumber}");
            }

            // Providing the full file path of fileName, and providing the List<string> to
            // write to the file.
            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();

            // Pulling the members of each PrizeModel out and concatenating them into a
            // string and adding that line to a List<string>.
            foreach(PrizeModel p in models)
            {
                lines.Add($"{p.ID},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }

            // Providing the full file path of fileName, and providing the List<string> to
            // write to the file.
            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            // Pulling the members of each TeamModel out and concatenating them into a
            // string and adding that line to a List<string>.
            foreach (TeamModel t in models)
            {
                // Adds a row in the format: Team_ID,Team_Name,MemID|MemID|MemID
                lines.Add($"{t.ID},{t.TeamName},{ConvertPeopleListToIDString(t.TeamMembers)}");
            }

            // Providing the full file path of fileName, and providing the List<string> to
            // write to the file.
            File.WriteAllLines(GlobalConfig.TeamFile.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model)
        {
            // Loop through each round
            // Loop through each matchup

            // Get ID for new matchup and save the record
            // Loop through each entry, get the ID, and save it

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel match in round)
                {
                    // Saving each match in the round
                    match.SaveMatchToFile();
                }
            }
        }

        public static void SaveMatchToFile(this MatchupModel match)
        {
            // Reading our existing matches out of the textfile into a list of MatchupModel
            List<MatchupModel> matches = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            int currentID = 1;

            if (matches.Count > 0)
            {
                currentID = matches.OrderByDescending(x => x.ID).First().ID + 1;
            }

            match.ID = currentID;

            // Adding our new match to the existing list
            matches.Add(match);

            // Then we save each entry in the match, before saving the match down below.
            foreach (MatchupEntryModel entry in match.Entries)
            { 
                entry.SaveEntryToFile();
            }

            // ID = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matches)
            {
                string winner = "";

                // Getting the winner ID if there is one, otherwise use an empty stting
                if (m.Winner != null)
                {
                    winner = m.Winner.ID.ToString();
                }

                // 0 - ID, 1 - List<MatchupEntryModel>, 2 - TeamModel, 3 - Matchup Round
                lines.Add($"{m.ID},{ConvertMatchupEntryListToIDString(m.Entries)},{winner},{m.MatchupRound}");

            }

            // Writing to the MatchupFile
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void UpdateMatchupToFile(this MatchupModel match)
        {
            // Reading our existing matches out of the textfile into a list of MatchupModel
            List<MatchupModel> matches = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();
            // Need to remove the current match in order to replace it with the updated one.
            foreach (MatchupModel m in matches)
            {
                if (m.ID == match.ID)
                {
                    oldMatchup = m;
                }
            }

            matches.Remove(oldMatchup);

            // Adding our new match to the existing list
            matches.Add(match);

            // Then we save each entry in the match, before saving the match down below.
            foreach (MatchupEntryModel entry in match.Entries)
            {
                entry.UpdateEntryToFile();
            }

            // ID = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matches)
            {
                string winner = "";

                // Getting the winner ID if there is one, otherwise use an empty stting
                if (m.Winner != null)
                {
                    winner = m.Winner.ID.ToString();
                }

                // 0 - ID, 1 - List<MatchupEntryModel>, 2 - TeamModel, 3 - Matchup Round
                lines.Add($"{m.ID},{ConvertMatchupEntryListToIDString(m.Entries)},{winner},{m.MatchupRound}");

            }

            // Writing to the MatchupFile
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            // Reading our existing entries out of the textfile into a list of MatchupEntryModel
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            // Checking for existing ID and updating if necessary
            int currentID = 1;

            if (entries.Count > 0)
            {
                currentID = entries.OrderByDescending(x => x.ID).First().ID + 1;
            }

            entry.ID = currentID;

            // Adding the new entry to the existing list
            entries.Add(entry);

            
            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parentID = "";

                // Checking for no existing parent matchup
                if (e.ParentMatchup != null)
                {
                    parentID = e.ParentMatchup.ID.ToString();
                }

                string teamCompeting = "";

                // Checking for no competing teams
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.ID.ToString();
                }

                lines.Add($"{e.ID},{teamCompeting},{e.Score},{parentID}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }

        // Replaces the existing entry with the updated entry and saves it.
        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            // Reading our existing entries out of the textfile into a list of MatchupEntryModel
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel();

            foreach (MatchupEntryModel e in entries)
            {
                if (e.ID == entry.ID)
                {
                    oldEntry = e;
                }
            }

            // Removing the old entry
            entries.Remove(oldEntry);

            // Adding the new entry to the existing list
            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parentID = "";

                // Checking for no existing parent matchup
                if (e.ParentMatchup != null)
                {
                    parentID = e.ParentMatchup.ID.ToString();
                }

                string teamCompeting = "";

                // Checking for no competing teams
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.ID.ToString();
                }

                lines.Add($"{e.ID},{teamCompeting},{e.Score},{parentID}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models)
        {
            List<String> lines = new List<string>();

            // Putting the information into a string of the form: 
            // ID,TournamentName,EntryFee,
            // (id|id|id - list of entered teams),
            // (id|id|id - list of prizes),
            // (id^id^id|id^id^id|id^id^id - list of rounds, ie. list of list of matchups)
            foreach (TournamentModel tm in models)
            {
                lines.Add($@"{tm.ID},{tm.TournamentName},{tm.EntryFee},{ConvertTeamListToIDString(tm.EnteredTeams)},{ConvertPrizeListToIDString(tm.Prizes)},{ConvertRoundListToMatchupString(tm.Rounds)}");
            }

            // Providing the full file path of fileName, and providing the List<string> to
            // write to the file.
            File.WriteAllLines(GlobalConfig.TournamentFile.FullFilePath(), lines);


        }






        // HELPER METHODS ///
       


        // Returns the team with the corresponding ID so we can assign a winner
        private static TeamModel LookupTeamByID(int ID)
        {
            // Gets all of the teams saved in the textfile
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();
            

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');

                if (cols[0] == ID.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);

                    // Returning the matching team.
                    return matchingTeams.ConvertToTeamModels().First();
                }
            }

            // Just in case there is no matching team.
            return null;
        }

        // Finds and returns a particular matchup from a given ID
        private static MatchupModel LookupMatchupByID(int ID)
        {
            // Gets all of the teams saved in the textfile
            List<string> matches = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

            // Searching for just the matching entry.
            foreach (string match in matches)
            {
                string[] cols = match.Split(',');

                if (cols[0] == ID.ToString())
                {
                    List<string> matchingMatches = new List<string>();
                    matchingMatches.Add(match);

                    return matchingMatches.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModel(string input)
        {
            string[] IDs = input.Split('|');

            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            // Looking just for the existing entries that match our given ID
            foreach (string id in IDs)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }

            // Converting the matching entries to MatchupEntryModel, and returning them.
            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        // Returns the IDs of all the teams from the given list in the format: 1|2|3 etc. 
        private static string ConvertTeamListToIDString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            // Concatenating each ID
            foreach (TeamModel t in teams)
            {
                output += $"{t.ID}|";
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        // Returns the IDs of all the prizes from the given list in the format: 1|2|3 etc. 
        private static string ConvertPrizeListToIDString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            // Concatenating the IDs
            foreach (PrizeModel p in prizes)
            {
                output += $"{p.ID}|";
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        // Converts the given list of rounds into a string containing the IDs of all the matchups,
        // which we then need to split again.
        private static string ConvertRoundListToMatchupString(List<List<MatchupModel>> rounds)
        {
            // Rounds - id^id^id|id^id^id|id^id^id
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            
            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToIDString(r)}|"; // Each group of matchups (id^id^id) separated by a pipe to yield above ID string
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        // Returns the IDs of all the matches from the given MatchupModel list in the format: 1^2^3 etc. 
        private static object ConvertMatchupListToIDString(List<MatchupModel> matchups)
        {
            // Matchups - id^id^id
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            // Concatenating the IDs of the matches.
            foreach (MatchupModel m in matchups)
            {
                output += $"{m.ID}^";
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        // Returns the IDs of all the matches from the given MatchupModel list in the format: 1^2^3 etc. 
        private static object ConvertMatchupEntryListToIDString(List<MatchupEntryModel> entries)
        {
            // Entries - id|id|id
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            // Concatenating the IDs of the matches.
            foreach (MatchupEntryModel e in entries)
            {
                output += $"{e.ID}|";
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        // Returns the IDs of all the team members the given Person list in the format: 1|2|3 etc. 
        private static string ConvertPeopleListToIDString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            // 2|5|3| <- need to remove trailing pipe character
            foreach (PersonModel p in people)
            {
                output += $"{p.ID}|";
            }

            // Removing the trailing pipe character.
            output = output.Substring(0, output.Length - 1); 

            return output;
        }

    }
}
