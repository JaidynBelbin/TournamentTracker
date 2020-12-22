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

        // TODO - Make the CreatePrize method actually save to the database.

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
    }
}
