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
        public PersonModel CreatePerson(PersonModel person)
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

                return person;
            }
        }

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
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

                return model;
            }
        }

        public TeamModel CreateTeam(TeamModel team)
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

                return team;
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
    }
}
