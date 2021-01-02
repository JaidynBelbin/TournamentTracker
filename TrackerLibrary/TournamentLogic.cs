using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        private static Random rng = new Random();

        /// <summary>
        /// Extension method that randomises a given List<T>. It doesn't actually need to be generic
        /// but I'll leave it as is just to demonstrate how one would do it, plus it might be useful
        /// in the future.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Randomise<T>(this IList<T> list)
        {

            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1); // return random number that is greater than or equal to 0
                                         // but less than n + 1.

                T value = list[k]; // 
                list[k] = list[n]; // swapping the values at indexes k and n
                list[n] = value;   // 
            }
        }

        public static void CreateRounds(TournamentModel model)
        {
            model.EnteredTeams.Randomise();

            int byes;
            int rounds = Math.DivRem(model.EnteredTeams.Count, 2, out byes);

            
        }

        // Order the list of teams randomly
        // Check to see if there are enough teams, if not, add byes
        // Create our first round of matchups
        // Create every round after that -- 8 matchups - 4 matchups - 2 matchups - 1 matchup -- winner

    }
}
