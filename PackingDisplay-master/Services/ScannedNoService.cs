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

        // ✅ Increment Scan
        public bool IncrementScan()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_IncrementScanCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                cmd.ExecuteNonQuery();

                _logService.LogSuccess("", "IncrementScan", "ScannedNoService");

                return true;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "IncrementScan", "ScannedNoService");
                return false;
            }
        }

        // ✅ Get Scan Count
        public int GetScanCount()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_GetScanCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                var result = cmd.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;

                _logService.LogSuccess("", "GetScanCount", "ScannedNoService");

                return count;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetScanCount", "ScannedNoService");
                return 0;
            }
        }

        // ✅ Increment Confirm
        public bool IncrementConfirm()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_IncrementConfirmCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                cmd.ExecuteNonQuery();

                _logService.LogSuccess("", "IncrementConfirm", "ScannedNoService");

                return true;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "IncrementConfirm", "ScannedNoService");
                return false;
            }
        }

        // ✅ Get Confirm Count
        public int GetConfirmCount()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("sp_GetConfirmCount", con);

                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                var result = cmd.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;

                _logService.LogSuccess("", "GetConfirmCount", "ScannedNoService");

                return count;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetConfirmCount", "ScannedNoService");
                return 0;
            }
        }
    }
}