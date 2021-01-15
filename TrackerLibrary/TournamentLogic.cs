using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        // Order our list randomly of teams
        // Check if it is big enough - if not, add in byes - 2*2*2*2 - 2^4
        // Create our first round of matchups
        // Create every round after that - 8 matchups - 4 matchups - 2 matchups - 1 matchup

        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(model, rounds);
        }

        public static void UpdateTournamentResults(TournamentModel model)
        {
            //int startingRound = model.CheckCurrentRound();
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    // If the winner of the round is not set AND EITHER
                    // any of the entries has a non-zero score OR
                    // the number of entries is one (ie. a bye week)
                    if (rm.Winner == null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1))
                    {
                        toScore.Add(rm); // need to score it.
                    }
                }
            }

            MarkWinnerInMatchups(toScore);

            AdvanceWinners(toScore, model);

            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x)); // Saving the scored matches and entries to the DB

            //int endingRound = model.CheckCurrentRound();

            //if (endingRound > startingRound)
            //{
            //    model.AlertUsersToNewRound();
            //}
        }

        //public static void AlertUsersToNewRound(this TournamentModel model)
        //{
        //    int currentRoundNumber = model.CheckCurrentRound();
        //    List<MatchupModel> currentRound = model.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();

        //    foreach (MatchupModel matchup in currentRound)
        //    {
        //        foreach (MatchupEntryModel me in matchup.Entries)
        //        {
        //            foreach (PersonModel p in me.TeamCompeting.TeamMembers)
        //            {
        //                AlertPersonToNewRound(p, me.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
        //            }
        //        }
        //    }
        //}

        //private static void AlertPersonToNewRound(PersonModel p, string teamName, MatchupEntryModel competitor)
        //{
        //    if (p.EmailAddress.Length > 0)
        //    {
        //        string to = "";
        //        string subject = "";
        //        StringBuilder body = new StringBuilder();

        //        if (competitor != null)
        //        {
        //            subject = $"You have a new matchup with { competitor.TeamCompeting.TeamName }";

        //            body.AppendLine("<h1>You have a new matchup</h1>");
        //            body.Append("<strong>Competitor: </strong>");
        //            body.Append(competitor.TeamCompeting.TeamName);
        //            body.AppendLine();
        //            body.AppendLine();
        //            body.AppendLine("Have a great time!");
        //            body.AppendLine("~Tournament Tracker");
        //        }
        //        else
        //        {
        //            subject = "You have a bye week this round";

        //            body.AppendLine("Enjoy your round off!");
        //            body.AppendLine("~Tournament Tracker");
        //        }

        //        to = p.EmailAddress;

        //        EmailLogic.SendEmail(to, subject, body.ToString());
        //    }

        //    if (p.CellphoneNumber.Length > 0)
        //    {
        //        SMSLogic.SendSMSMessage(p.CellphoneNumber, $"You have a new matchup with { competitor.TeamCompeting.TeamName }");
        //    }
        //}

        private static int CheckCurrentRound(this TournamentModel model)
        {
            int output = 1;

            foreach (List<MatchupModel> round in model.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
            }

            return output;
        }

        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.ID == m.ID)
                                {
                                    me.TeamCompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void MarkWinnerInMatchups(List<MatchupModel> models)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in models)
            {
                // Checks for bye week entry
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                // 0 means low score wins, like in golf
                if (greaterWins == "0")
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in this application.");
                    }
                }
                else
                {
                    // 1 means true, or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in this application.");
                    }
                }
            }
        }

        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currRound);
                previousRound = currRound;

                currRound = new List<MatchupModel>();
                round += 1;
            }
        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;
        }

        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;

            return output;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                // output = output + 1;
                output += 1;

                // val = val * 2;
                val *= 2;
            }

            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            // cards.OrderBy(a => Guid.NewGuid()).ToList();
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
