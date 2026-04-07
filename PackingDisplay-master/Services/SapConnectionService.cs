using Microsoft.Data.SqlClient;
using PackingDisplay.Models;

namespace PackingDisplay.Services
{
    public class SapConnectionService
    {
        private readonly string? _connectionString;
        
        public SapConnectionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SAPConnectionConfig GetActiveConfig()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("sp_GetActiveSAPConfig", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;


            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new SAPConnectionConfig
                {
                    Name = reader["Name"].ToString(),
                    SystemNumber = reader["SystemNumber"].ToString(),
                    AppServerHost = reader["AppServerHost"].ToString(),
                    SAPRouter = reader["SAPRouter"]?.ToString(),
                    Client = reader["Client"].ToString(),
                    Language = reader["Language"].ToString(),
                    User = reader["User"].ToString(),
                    Password = reader["Password"].ToString(),
                    PoolSize = Convert.ToInt32(reader["PoolSize"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])


                };
            }

            return null;
        }
    }
}