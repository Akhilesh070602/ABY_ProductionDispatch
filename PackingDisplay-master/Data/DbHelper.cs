using Microsoft.Data.SqlClient;

namespace PackingDisplay.Data
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(string query)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            SqlCommand cmd = new SqlCommand(query, con);
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
