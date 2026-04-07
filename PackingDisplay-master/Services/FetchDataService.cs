using Microsoft.Data.SqlClient;
//using PackingDisplay.SAP;
using SAP.Middleware.Connector;
using System.Data;

namespace PackingDisplay.Services
{
    public class FetchDataService
    {
        private readonly string? _connectionString;
        private readonly ILogger<FetchDataService> _logger;

        public FetchDataService(IConfiguration config, ILogger<FetchDataService> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task FetchAndStoreData()
        {
            try
            {
                var dest = RfcDestinationManager.GetDestination("DEP");
                var repo = dest.Repository;

                IRfcFunction func = repo.CreateFunction("ZPP_PKG_PRD");

                // ✅ Correct RFC Parameters: fetch all current month
                var today = DateTime.Today;
                var firstDay = new DateTime(today.Year, today.Month, 1);
                var lastDay = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                func.SetValue("PLANT", "1100");
                func.SetValue("FDATE", firstDay.ToString("yyyyMMdd"));  // 1st day of current month
                func.SetValue("TDATE", lastDay.ToString("yyyyMMdd"));   // last day of current month

                // Call SAP
                func.Invoke(dest);

                IRfcTable table = func.GetTable("PKG_PRD");

                if (table == null || table.Count == 0)
                {
                    _logger.LogWarning("No data received from SAP");
                    return;
                }

                // Convert SAP → DataTable
                DataTable dt = ConvertToDataTable(table);

                // Insert into SQL Server
                await BulkInsert(dt);

                _logger.LogInformation("SAP Data inserted successfully. Rows: {count}", dt.Rows.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FetchAndStoreData");
            }
        }

        // Convert SAP Table → DataTable
        private DataTable ConvertToDataTable(IRfcTable sapTable)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MBLNR", typeof(string));
            dt.Columns.Add("MJAHR", typeof(string));
            dt.Columns.Add("ZEILE", typeof(decimal));
            dt.Columns.Add("MENGE", typeof(decimal));
            dt.Columns.Add("BUDAT", typeof(DateTime)); // SQL Date column

            foreach (IRfcStructure row in sapTable)
            {
                dt.Rows.Add(
                    row.GetString("MBLNR"),
                    row.GetString("MJAHR"),
                    Convert.ToDecimal(row.GetValue("ZEILE") ?? 0),
                    Convert.ToDecimal(row.GetValue("MENGE") ?? 0),
                    ParseSapDate(row.GetString("BUDAT")) // ✅ Parse SAP BUDAT
                );
            }

            return dt;
        }

        // Parse SAP Date String → DateTime
        private object ParseSapDate(string sapDate)
        {
            if (string.IsNullOrWhiteSpace(sapDate))
                return DBNull.Value;

            sapDate = sapDate.Trim();

            // Try YYYY-MM-DD
            if (DateTime.TryParseExact(sapDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Try YYYYMMDD
            if (DateTime.TryParseExact(sapDate, "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }

            _logger.LogWarning("Invalid BUDAT format from SAP: {sapDate}", sapDate);
            return DBNull.Value;
        }

        // Bulk Insert into SQL (with duplicate handling)
        private async Task BulkInsert(DataTable dt)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            // ✅ Clear table before inserting new data to avoid duplicates
            using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE PKG_PRDNEW_TEMP", con))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            using SqlBulkCopy bulk = new SqlBulkCopy(con)
            {
                DestinationTableName = "PKG_PRDNEW_TEMP",
                BatchSize = 5000,
                BulkCopyTimeout = 60
            };

            // Column Mapping
            bulk.ColumnMappings.Add("MBLNR", "MBLNR");
            bulk.ColumnMappings.Add("MJAHR", "MJAHR");
            bulk.ColumnMappings.Add("ZEILE", "ZEILE");
            bulk.ColumnMappings.Add("MENGE", "MENGE");
            bulk.ColumnMappings.Add("BUDAT", "BUDAT");

            await bulk.WriteToServerAsync(dt);
        }
    }
}