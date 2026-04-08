using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using PackingDisplay.Data;
using PackingDisplay.Models;

namespace PackingDisplay.Services
{
    public class DashboardService
    {
        private readonly string? _connectionString;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardService> _logger;
        private readonly DbHelper _db;

        public DashboardService(DbHelper db)
        {
            _db = db;
        }
        public DashboardService(IConfiguration config, IMemoryCache cache, ILogger<DashboardService> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboard()
        {
            try
            {
                return await _cache.GetOrCreateAsync("dashboard_cache", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);

                    using SqlConnection con = new SqlConnection(_connectionString);
                    await con.OpenAsync();

                    // ✅ FIXED QUERY
                    string query = @"
                    SELECT 
                        ISNULL(SUM(CASE 
                            WHEN BUDAT >= CAST(GETDATE() AS DATE) 
                             AND BUDAT < DATEADD(DAY,1,CAST(GETDATE() AS DATE))
                            THEN MENGE ELSE 0 END),0) AS TodayProduction,

                        ISNULL(SUM(CASE 
                            WHEN MONTH(BUDAT)=MONTH(GETDATE()) 
                             AND YEAR(BUDAT)=YEAR(GETDATE())
                            THEN MENGE ELSE 0 END),0) AS MonthProduction,

                        ISNULL((SELECT TOP 1 WKDAYS FROM DYG_WDAYS),0) AS WorkingDays,
                        ISNULL((SELECT TOP 1 TARGET FROM PKG_TARGET),0) AS Target

                    FROM PKG_PRDNEW_TEMP";

                    using SqlCommand cmd = new SqlCommand(query, con);
                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    DashboardDto data = new DashboardDto();

                    if (await reader.ReadAsync())
                    {
                        data.TodayProduction = reader["TodayProduction"] != DBNull.Value ? Convert.ToDecimal(reader["TodayProduction"]) : 0;
                        data.MonthProduction = reader["MonthProduction"] != DBNull.Value ? Convert.ToDecimal(reader["MonthProduction"]) : 0;
                        data.WorkingDays = reader["WorkingDays"] != DBNull.Value ? Convert.ToInt32(reader["WorkingDays"]) : 0;
                        data.Target = reader["Target"] != DBNull.Value ? Convert.ToDecimal(reader["Target"]) : 0;
                    }

                    return data;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard data");
                return new DashboardDto();
            }
        }
        
    }
}