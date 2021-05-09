using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace RedisCacheWebApp.Services
{
    public class DataService : IDataService
    {
        public const string ConnectionString = "Data Source=.;Initial Catalog=QA;Integrated Security=true";

        public async Task<State> GetData(int id)
        {
            State s = null;
            var query = "select * from tblState where StatePK = @id";
            using (var connection = new SqlConnection(ConnectionString))
            {
                s = await connection.QueryFirstOrDefaultAsync<State>(query, new { id = id });
            }

            return s;
        }
    }

    public class State
    {
        public string DisplayName { get; set; }

        public string ShortName { get; set; }

        public string stateCode { get; set; }
    }
}
