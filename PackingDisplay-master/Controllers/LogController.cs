using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
using PackingDisplay.Models;
//using System;
//using System.Collections.Generic;
//using System.Data

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly string _connectionString;

        public LogController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string missing");
        }

        // ✅ GET ALL LOGS
        [HttpGet("all")]
        public IActionResult GetAllLogs()
        {
            var logs = new List<ErrorLog>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
               
                SqlCommand cmd = new SqlCommand("sp_GetAllLogs", con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(MapLog(reader));
                }
            }

            return Ok(logs);
        }

        // ✅ GET FAILED LOGS
        [HttpGet("failed")]
        public IActionResult GetFailedLogs()
        {
            var logs = new List<ErrorLog>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetFailedLogs", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(MapLog(reader));
                }
            }

            return Ok(logs);
        }

        // ✅ FILTER BY PO NO
        [HttpGet("by-po/{poNo}")]
        public IActionResult GetByPo(string poNo)
        {
            var logs = new List<ErrorLog>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
               
                SqlCommand cmd = new SqlCommand("sp_GetLogsByPo", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PoNo", poNo);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(MapLog(reader));
                }
            }

            return Ok(logs);
        }

        // ✅ FILTER BY STATUS
        [HttpGet("by-status/{status}")]
        public IActionResult GetByStatus(string status)
        {
            var logs = new List<ErrorLog>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
               
                SqlCommand cmd = new SqlCommand("sp_GetLogsByStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(MapLog(reader));
                }
            }

            return Ok(logs);
        }

        // ✅ FILTER BY DATE RANGE
        [HttpGet("by-date")]
        public IActionResult GetByDate(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<ErrorLog>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
            
                SqlCommand cmd = new SqlCommand("sp_GetLogsByDate", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@From", fromDate);
                cmd.Parameters.AddWithValue("@To", toDate);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(MapLog(reader));
                }
            }

            return Ok(logs);
        }

        // 🔁 COMMON MAPPING FUNCTION
        private ErrorLog MapLog(SqlDataReader reader)
        {
            return new ErrorLog
            {
                Id = Convert.ToInt32(reader["Id"]),
                Message = reader["Message"]?.ToString(),
                StackTrace = reader["StackTrace"]?.ToString(),
                Source = reader["Source"]?.ToString(),

                PoNo = reader["PoNo"]?.ToString(),
                ApiName = reader["ApiName"]?.ToString(),
                ServiceName = reader["ServiceName"]?.ToString(),

                Status = reader["Status"]?.ToString(),
                RequestData = reader["RequestData"]?.ToString(),

                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}