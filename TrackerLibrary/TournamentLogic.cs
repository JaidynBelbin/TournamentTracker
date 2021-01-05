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
        private static readonly Random rng = new Random();

        public static void CreateRounds(TournamentModel model)
        {
            // Randomising the teams
            List<TeamModel> randomisedTeams = Randomise(model.EnteredTeams);

            // Finding the number of rounds we need and whether or not we need to
            // include a bye in the first round.
            int rounds = FindNumberOfRounds(randomisedTeams.Count);
            int byes = FindNumberOfByes(rounds, randomisedTeams.Count);

            // int rounds = Math.DivRem(model.EnteredTeams.Count, 2, out int byes);

            // Creating the first round of matchups in our tournament
            model.Rounds.Add(CreateFirstRound(byes, randomisedTeams));

            // Creating all subsequent rounds
            CreateNextRounds(model, rounds);
        }

        /// <summary>
        /// Method that randomises a given List<TeamModel>. 
        /// </summary>
        /// <typeparam name="TeamModel"></typeparam>
        /// <param name="list"></param>
        private static List<TeamModel> Randomise(List<TeamModel> list)
        {

            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1); // return random number that is greater than or equal to 0
                                         // but less than n + 1.

                TeamModel value = list[k]; // 
                list[k] = list[n];         // swapping the values at indexes k and n
                list[n] = value;           // 
            }

            return list;
        }

        //private static List<TeamModel> Randomise(List<TeamModel> teams)
        //{
        //    return teams.OrderBy(x => Guid.NewGuid()).ToList();
        //}

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }

            return output;
        }

        private static int FindNumberOfByes(int rounds, int teamCount)
        {

            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - teamCount;

            return output;
        }

        // Pairing the teams up into matchups, accounting for any byes that need to be used
        // in the first round. There are no winners yet, that is calculated later.
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> round = new List<MatchupModel>();
            MatchupModel currentMatch = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                // Adding the first entry, and subseqent pairs.
                currentMatch.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                // If there is a bye, or the matchup is already full (ie. 2 teams), add
                // the MatchupEntryModel to the MatchupModel and make a new MatchupModel. If not, break out of the
                // if statement and add the second team to the Entries list, then finish.
                if (byes > 0 || currentMatch.Entries.Count > 1)
                {
                    currentMatch.MatchupRound = 1;
                    round.Add(currentMatch);

                    currentMatch = new MatchupModel();

                    // The bye is assigned, so decrement it to 0. Next time we come
                    // to this loop, byes will be 0, so only need to check if there's
                    // 2 teams in the match or not.

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return round;
        }

        private static void CreateNextRounds(TournamentModel model, int rounds) 
        {
            
            // Will always start with the second round, the initial round is created in a
            // separate method.
            int round = 2;

            // Initially, previousRound is the first round
            List<MatchupModel> previousRound = model.Rounds[0];

            // Setting up a new round and a new match
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatch = new MatchupModel();

            while (round <= rounds)
            {
                // Looping through each of the matches in the (initially first) round
                foreach (MatchupModel match in previousRound)
                {
                    // The parent match is the one the current match is coming from
                    currentMatch.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    // If there is more than one team in the current match...
                    if (currentMatch.Entries.Count > 1)
                    {
                        // Setting the round number
                        currentMatch.MatchupRound = round;

                        // Adding the filled match to the round
                        currentRound.Add(currentMatch);

                        // Resetting the currentMatch
                        currentMatch = new MatchupModel();
                    }
                }

                // Adding the round to the Tournament
                model.Rounds.Add(currentRound);
                // Incrementing the round
                previousRound = currentRound;

                // Resetting the current round to 0 matches and incrementing the round number
                currentRound = new List<MatchupModel>();
                round += 1;
            }
        }
    }
}
