using Microsoft.Data.SqlClient;
using System.Data;

namespace PackingDisplay.Services
{
    public class ScannedNoService
    {
        private readonly IConfiguration _configuration;
        private readonly LogService _logService;

        public ScannedNoService(IConfiguration configuration, LogService logService)
        {
            _configuration = configuration;
            _logService = logService;
        }

        // ✅ Increment Scan Count
        public bool IncrementScan()
        {
            string apiName = "IncrementScanCount";
            string serviceName = "ScannedNoService";

            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_IncrementScanCount", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                int rows = cmd.ExecuteNonQuery();  // ✅ ONLY this

                _logService.LogSuccess("", apiName, serviceName, $"RowsAffected={rows}");

                return true;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", apiName, serviceName);
                return false;
            }
        }

        // ✅ Get Scan Count
        public int GetScanCount()
        {
            string apiName = "GetScanCount";
            string serviceName = "ScannedNoService";

            try
            {
                int count = 0;

                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_GetScanCount", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                var result = cmd.ExecuteScalar();  // ✅ ONLY this

                if (result != null)
                    count = Convert.ToInt32(result);

                _logService.LogSuccess("", apiName, serviceName, $"Count={count}");

                return count;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", apiName, serviceName);
                return 0;
            }
        }
        public bool IncrementConfirm()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_IncrementConfirmCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "IncrementConfirm", "ConfirmService");
                return false;
            }
        }

        public int GetConfirmCount()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_GetConfirmCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetConfirmCount", "ConfirmService");
                return 0;
            }
        }
    }
}