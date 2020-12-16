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
    }
}
