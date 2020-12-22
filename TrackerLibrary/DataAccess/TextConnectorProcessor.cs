using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

// * Load the text file
// Convert the text to a List<PrizeModel>
// Find the id
// Add new record with the new id (max + 1)
// Convert the prizes to a List<string>
// Save the List<string> to the text file

namespace TrackerLibrary.DataAccess.TextHelpers
{
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
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();

                p.ID = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];

                output.Add(p);
            }
            
            return output;

        }

        public static void SaveToPersonFile(this List<PersonModel> models, string fileName)
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
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
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
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
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
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            // team id, team name, list of people id's separated by a pipe, eg.
            // 3, Red Team, 1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

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
            }

            return output;
        }

        // Returns the IDs of all the team members the given list in the format: 1|2|3 etc. 
        
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
