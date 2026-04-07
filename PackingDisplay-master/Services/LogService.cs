using System;
using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;

namespace PackingDisplay.Services
{
    public class LogService
    {
        private readonly string? _connectionString;

        public LogService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string missing");
        }
        // 🔴 ERROR LOG
        public void LogError(Exception ex, string poNo, string apiName, string serviceName, string requestData = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertErrorLog", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Message", ex.Message ?? "");
                    cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                    cmd.Parameters.AddWithValue("@Source", ex.Source ?? "");

                    cmd.Parameters.AddWithValue("@PoNo", poNo ?? "");
                    cmd.Parameters.AddWithValue("@ApiName", apiName ?? "");
                    cmd.Parameters.AddWithValue("@ServiceName", serviceName ?? "");

                    cmd.Parameters.AddWithValue("@Status","FAILED");
                    cmd.Parameters.AddWithValue("@RequestData", requestData ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // avoid crash
            }
        }

        // 🟢 SUCCESS LOG (also using SAME SP 🔥)
        public void LogSuccess(string poNo, string apiName, string serviceName, string requestData = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertErrorLog", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Message", "");
                    cmd.Parameters.AddWithValue("@StackTrace", "");
                    cmd.Parameters.AddWithValue("@Source", "");

                    cmd.Parameters.AddWithValue("@PoNo", poNo ?? "");
                    cmd.Parameters.AddWithValue("@ApiName", apiName ?? "");
                    cmd.Parameters.AddWithValue("@ServiceName", serviceName ?? "");

                    cmd.Parameters.AddWithValue("@Status", "SUCCESS");
                    cmd.Parameters.AddWithValue("@RequestData", requestData ?? "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
        public void LogInfo(string message, string poNo, string apiName, string serviceName, string requestData = "")
        {
            ExecuteLog(
                message,
                "",
                "",
                poNo,
                apiName,
                serviceName,
                "SUCCESS",
                requestData
            );
        }

        // 🔥 COMMON METHOD (BEST PRACTICE)
        private void ExecuteLog(string message, string stackTrace, string source,
                                string poNo, string apiName, string serviceName,
                                string status, string requestData)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_InsertErrorLog", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Message", System.Data.SqlDbType.NVarChar).Value = message ?? "";
                    cmd.Parameters.Add("@StackTrace", System.Data.SqlDbType.NVarChar).Value = stackTrace ?? "";
                    cmd.Parameters.Add("@Source", System.Data.SqlDbType.NVarChar).Value = source ?? "";

                    cmd.Parameters.Add("@PoNo", System.Data.SqlDbType.NVarChar).Value = poNo ?? "";
                    cmd.Parameters.Add("@ApiName", System.Data.SqlDbType.NVarChar).Value = apiName ?? "";
                    cmd.Parameters.Add("@ServiceName", System.Data.SqlDbType.NVarChar).Value = serviceName ?? "";

                    cmd.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar).Value = status ?? "";
                    cmd.Parameters.Add("@RequestData", System.Data.SqlDbType.NVarChar).Value = requestData ?? "";

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch{}
        }

    }
}