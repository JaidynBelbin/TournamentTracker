using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;


namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {

        private const string db = "Tournaments";

        /// <summary>
        /// Saves a new person to the database
        /// </summary>
        /// <param name="person">The person information.</param>
        /// <returns>The person information, including their unique identifier.</returns>
        public void CreatePerson(PersonModel person)
        {
            // The 'using' syntax properly closes the connection upon reaching the curly brace, preventing memory leaks.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                // Creating a dynamic parameter to hold all the data members
                var p = new DynamicParameters();

                p.Add("@FirstName", person.FirstName);
                p.Add("@LastName", person.LastName);
                p.Add("@EmailAddress", person.EmailAddress);
                p.Add("@CellphoneNumber", person.CellphoneNumber);

                // This is the variable "coming out" of the database, so need to specify
                // ParameterDirection.Output
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Executing the stored procedure "dbo.spPeople_Insert" using the dynamic parameter p 
                // set above using Dapper.
                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                // Looks at the dynamic parameter list p, gets the value called @id of type int and sets it
                // as the model.ID value.
                person.ID = p.Get<int>("@id");
            }
        }

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            // The 'using' syntax properly closes the connection upon reaching the curly brace, preventing memory leaks.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                // Creating a dynamic parameter
                var p = new DynamicParameters();

                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);

                // This is the variable "coming out" of the database, so need to specify
                // ParameterDirection.Output
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Executing the stored procedure "dbo.spPrizes_Insert" using the dynamic parameter p 
                // set above using Dapper.
                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                // Looks at the dynamic parameter list p, gets the value called @id of type int and sets it
                // as the model.ID value.
                model.ID = p.Get<int>("@id");
            }
        }

        public void CreateTeam(TeamModel team)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                // Creating a dynamic parameter to hold all the data members
                var p = new DynamicParameters();

                // Only the team name added in this stored procedure.
                p.Add("@TeamName", team.TeamName);
               
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Executing the stored procedure "dbo.spPeople_Insert" using the dynamic parameter p 
                // set above using Dapper.
                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                // Looks at the dynamic parameter list p, gets the value called @id of type int and sets it
                // as the model.ID value.
                team.ID = p.Get<int>("@id");

                // Inserting each of the team members into the TeamMembers table.
                foreach (PersonModel person in team.TeamMembers)
                {
                    p = new DynamicParameters();

                    p.Add("@TeamId", team.ID);
                    p.Add("@PersonId", person.ID);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                SaveTournament(model, connection);

                SaveTournamentPrizes(model, connection);

                SaveTournamentEntries(model, connection);

                SaveTournamentRounds(model, connection);
            }
        }

        
        // Methods to save the tournament data, tournament prizes, tournament team entries and
        // the tournament rounds into their respective places.
        private void SaveTournament(TournamentModel model, IDbConnection connection)
        {
            var p = new DynamicParameters();

            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);

            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

            model.ID = p.Get<int>("@id");
        }

        private void SaveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            // Inserting each of the tournament prizes into the TournamentPrizes database
            foreach (PrizeModel prize in model.Prizes)
            {
                var p = new DynamicParameters();

                p.Add("@TournamentId", model.ID);
                p.Add("@PrizeId", prize.ID);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            // Inserting each of the tournament entries into the TournamentEntries database
            foreach (TeamModel team in model.EnteredTeams)
            {
                var p = new DynamicParameters();

                p.Add("@TournamentId", model.ID);
                p.Add("@TeamId", team.ID);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {
            // List<List<MatchupModel>> Rounds
            // List<MatchupEntryModel> Competing Teams

            // Save the matches

            // Save the entries of the matches

            // Looping through each round
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // Saving each match in the round
                foreach (MatchupModel match in round)
                {
                    var p = new DynamicParameters();

                    p.Add("@TournamentId", model.ID);
                    p.Add("@MatchupRound", match.MatchupRound);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                    match.ID = p.Get<int>("@id");

                    // Saving each entry in the match
                    foreach (MatchupEntryModel entry in match.Entries)
                    {
                        p = new DynamicParameters();

                        p.Add("@MatchupId", match.ID);

                        if (entry.ParentMatchup == null)
                        {
                            p.Add("@ParentMatchupId", null);

                        } else
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatchup.ID);
                        }
                        
                        // This happens if the teams haven't advanced to the next round, there are
                        // no teams competing in round 2 until the winners are decided.
                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);

                        } else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.ID);
                        }
                        
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }

        }

        // Executing the spPeople_GetAll query and returning the List<PersonModel> that it fills out.
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        // Executing the spTeam_GetAll query and returning the List<TeamModel> that it fills out. Also need
        // the team members associated with each team.
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                // Getting the the team members (People) associated with each team.
                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.ID);

                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                var p = new DynamicParameters();
                

                foreach (TournamentModel t in output)
                {

                    // Putting the Tournament ID into a DynamicParameter to look up the matchup information from the database
                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.ID);

                    // Populating the prizes
                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.ID);

                    // Populating the teams
                    t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeam_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // Getting the the team members (People) associated with each team.
                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        p = new DynamicParameters(); // Don't know why it has to be called this
                        p.Add("@TeamId", team.ID);

                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }


                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.ID);

                    // Getting the matchups from the DB.
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // Looping through and getting each ID
                    foreach (MatchupModel matchup in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.ID);

                        // Getting the matchup entries from the DB for each match ID
                        matchup.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        // Populate each entry (2 models)
                        // Populate each match (1 model)

                        // Loading all the teams so we can find the teams with the same ID as our matchup entries
                        List<TeamModel> allTeams = GetTeam_All();

                        foreach (var me in matchup.Entries)
                        {
                            if (me.TeamCompetingID > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.ID == me.TeamCompetingID).First();
                            }

                            // Since matchups are stored in order, we should always have the parent matchup available to us.
                            if (me.ParentMatchupID > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.ID == me.ParentMatchupID).First();
                            }
                        }
                    }

                    // Putting the matchups into a List and populating our rounds (List<List<MatchupModel>>)
                    List<MatchupModel> currentRow = new List<MatchupModel>();

                    int currentRound = 1;

                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currentRound)
                        {
                            t.Rounds.Add(currentRow);

                            currentRow = new List<MatchupModel>();
                            currentRound += 1;
                        }

                        currentRow.Add(m);
                    }

                    t.Rounds.Add(currentRow);
                }
            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString(db)))
            {
                // Storing the matchup information
                var p = new DynamicParameters();

                if (model.Winner != null)
                {
                    p.Add("@id", model.ID);
                    p.Add("@WinnerId", model.Winner.ID);

                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure); 
                }


                // Storing the matchup entry information
                foreach (MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();

                        p.Add("@id", me.ID);
                        p.Add("@TeamCompetingId", me.TeamCompeting.ID);
                        p.Add("@Score", me.Score);

                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }  
                }
            }
        }
    }
}
