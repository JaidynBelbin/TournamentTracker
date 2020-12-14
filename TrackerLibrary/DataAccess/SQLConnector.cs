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
        // TODO - Make the CreatePrize method actually save to the database.

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // The 'using' syntax properly closes the connection upon reaching the curly brace, preventing memory leaks.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.ConnectionString("Tournaments")))
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

                // Executing the stored procedure "dbo.spPrizes_Insert" using the parameter p 
                // set above using Dapper.
                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                // Looks at the dynamic parameter list p, gets the value called @id of type int and sets it
                // as the model.ID value.
                model.ID = p.Get<int>("@id");

                return model;
            }
        }
    }
}
