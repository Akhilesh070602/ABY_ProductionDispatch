//using PackingDisplay.Models;
//using PackingDisplay.SAP;
//using SAP.Middleware.Connector;
//using Microsoft.Data.SqlClient;

//namespace PackingDisplay.Services
//{
//    public class DispatchService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly LogService _logService;

//        //public DispatchService(IConfiguration configuration)
//        //{
//        //    _configuration = configuration;
//        //}
//        public DispatchService(IConfiguration configuration, LogService logService)
//        {
//            _configuration = configuration;
//            _logService = logService;
//        }


//        // ✅ GET DATA FROM SAP
//        public DispatchDto GetDispatchData(string code)
//        {
//            try
//            {
//                var dest = RfcDestinationManager.GetDestination("DEP");
//                var repo = dest.Repository;

//                IRfcFunction func = repo.CreateFunction("ZRFC_HU_DET");
//                func.SetValue("IV_AUFNR", code);
//                func.Invoke(dest);

//                IRfcTable table = func.GetTable("LT_TBL");

//                // ❗ FIX → return null (not empty object)
//                if (table.RowCount == 0)
//                    return null;

//                DispatchDto dto = new DispatchDto();

//                var firstRow = table[0];
//                Console.WriteLine("🚀 SERVICE START");
//                dto.PoNo = firstRow.GetString("AUFNR");
//                dto.EXIDV = firstRow.GetString("EXIDV");
//                dto.SONUM = firstRow.GetString("SONUM");
//                dto.WERKS = firstRow.GetString("WERKS");
//                dto.Material = firstRow.GetString("MATNR");
//                dto.Cone = firstRow.GetString("ZCONE_NO");
//                dto.HuCode = firstRow.GetString("VENUM");
//                dto.StdWeight = firstRow.GetDecimal("STD_WEIGHT");
//                dto.Vemeh = firstRow.GetString("VEMEH");

//                foreach (IRfcStructure row in table)
//                {
//                    dto.Items.Add(new LineItemDto
//                    {
//                        Name = row.GetString("KDMAT"),
//                        ConeWeight = row.GetDecimal("ZCONE_WT"),
//                        Material = row.GetString("MATNR")
//                    });
//                }
//                return dto;
//            }
//            catch (Exception ex)
//            {
//                _logService.LogError(
//                    ex,
//                    code,
//                    "GetDispatchData",
//                    "DispatchService",
//                    $"PO={code}"
//                );

//                return null;
//            }
//        }

//        // ✅ GET IMAGE FROM DB
//        public string GetMaterialImage(string matnr)
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(
//                    _configuration.GetConnectionString("DefaultConnection")))
//                {
//                    con.Open();



//                    SqlCommand cmd = new SqlCommand("sp_GetMaterialImage", con);
//                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@Material", matnr);

//                    var result = cmd.ExecuteScalar();
//                    return result?.ToString();
//                }
//            }
//            catch (Exception ex)
//            {
//                _logService.LogError(
//                    ex,
//                    "",
//                    "GetMaterialImage",
//                    "DispatchService",
//                    $"Material={matnr}"
//                );

//                return null;
//            }
//        }

//        // ✅ SEND DATA TO SAP
//        public void SaveActualWeight(DispatchSaveDto model)
//        {
//            try
//            {
//                decimal noOfCone = Convert.ToDecimal(model.ZCONE_NO ?? "");
//                decimal coneWeight = Convert.ToDecimal(model.ZCONE_WT);
//                decimal ActualWeight = Convert.ToDecimal(model.ACT_WEIGHT);

//                // Standard weight
//                decimal StdWeight = Convert.ToDecimal(coneWeight*noOfCone);

//                // added Tolerance
//                decimal Tolerance = StdWeight * 0.05m;

//                //check tolerance
//                decimal minWeight = StdWeight - Tolerance;
//                decimal maxWeight = StdWeight + Tolerance;

//                // for validation 
//                if (ActualWeight < minWeight)
//                    throw new Exception($"UNDER WEIGHT (Min: {minWeight})");

//                if (ActualWeight > maxWeight)
//                    throw new Exception($"OVER WEIGHT (Max: {maxWeight})");

//                var dest = RfcDestinationManager.GetDestination("DEP");
//                var repo = dest.Repository;

//                IRfcFunction func = repo.CreateFunction("ZRFC_HU_DATA_POST");

//                // ✅ STRING VALUES (never null)
//                func.SetValue("IV_AUFNR", model.AUFNR ?? "");
//                func.SetValue("IV_VENUM", model.VENUM ?? "");
//                func.SetValue("IV_EXIDV", model.EXIDV ?? "");
//                func.SetValue("IV_SONUM", model.SONUM ?? "");

//                func.SetValue("IV_MATNR", model.MATNR ?? "");
//                func.SetValue("IV_WERKS", model.WERKS ?? "");
//                func.SetValue("IV_KDMAT", model.KDMAT ?? "");

//                func.SetValue("IV_VEMEH", model.VEMEH ?? "");

//                // ✅ DECIMAL VALUES (never null)
//                func.SetValue("IV_STD_WEIGHT", Convert.ToDecimal(model.STD_WEIGHT));
//                func.SetValue("IV_ZCONE_WT", Convert.ToDecimal(model.ZCONE_WT));
//                func.SetValue("IV_ACT_WEIGHT", Convert.ToDecimal(model.ACT_WEIGHT));

//                // ✅ STRING BUT NUMERIC FORMAT
//                func.SetValue("IV_ZCONE_NO", model.ZCONE_NO ?? "");

//                // 🔥 DEBUG (VERY IMPORTANT)
//                Console.WriteLine("---- SAP INPUT ----");
//                Console.WriteLine($"AUFNR: {model.AUFNR}");
//                Console.WriteLine($"VENUM: {model.VENUM}");
//                Console.WriteLine($"MATNR: {model.MATNR}");
//                Console.WriteLine($"ACT_WEIGHT: {model.ACT_WEIGHT}");
//                for (int i = 0; i < func.Metadata.ParameterCount; i++)
//                {
//                    var p = func.Metadata[i];
//                    Console.WriteLine("PARAM: " + p.Name);
//                }
//                Console.WriteLine("🚀 CALLING SAP...");
//                func.Invoke(dest);
//                Console.WriteLine("✅ SAP SUCCESS");
//            }
//            catch (Exception ex)
//            {
//                _logService.LogError(
//                    ex,
//                    model?.AUFNR,
//                    "SaveActualWeight",
//                    "DispatchService",
//                    $"PO={model?.AUFNR}"
//                );

//                throw;
//            }
//        }
//    }
//}
using PackingDisplay.Models;
//using PackingDisplay.SAP;
using SAP.Middleware.Connector;
using Microsoft.Data.SqlClient;

namespace PackingDisplay.Services
{
    public class DispatchService
    {
        private readonly IConfiguration _configuration;
        private readonly LogService _logService;

        public DispatchService(IConfiguration configuration, LogService logService)
        {
            _configuration = configuration;
            _logService = logService;
        }

        // ✅ GET DATA FROM SAP
        public DispatchDto GetDispatchData(string code)
        {
            try
            {
                var dest = RfcDestinationManager.GetDestination("DEP");
                var repo = dest.Repository;

                IRfcFunction func = repo.CreateFunction("ZRFC_HU_DET");
                func.SetValue("IV_AUFNR", code);

                // ✅ Only important log
                _logService.LogInfo("Calling SAP ZRFC_HU_DET", code, "GetDispatchData", "DispatchService");

                func.Invoke(dest);

                IRfcTable table = func.GetTable("LT_TBL");

                if (table.RowCount == 0)
                    return new DispatchDto
                    {
                        Items = new List<LineItemDto>()
                    }; ;

                DispatchDto dto = new DispatchDto
                {
                    Items = new List<LineItemDto>() // ✅ FIX
                };

                var firstRow = table[0];

                dto.PoNo = firstRow.GetString("AUFNR");
                dto.EXIDV = firstRow.GetString("EXIDV");
                dto.SONUM = firstRow.GetString("SONUM");
                dto.WERKS = firstRow.GetString("WERKS");
                dto.Material = firstRow.GetString("MATNR");
                dto.Cone = firstRow.GetString("ZCONE_NO");
                dto.HuCode = firstRow.GetString("VENUM");
                dto.StdWeight = firstRow.GetDecimal("STD_WEIGHT");
                dto.Vemeh = firstRow.GetString("VEMEH");

                foreach (IRfcStructure row in table)
                {
                    dto.Items.Add(new LineItemDto
                    {
                        Name = row.GetString("KDMAT"),
                        ConeWeight = row.GetDecimal("ZCONE_WT"),
                        Material = row.GetString("MATNR")
                    });
                }

                _logService.LogSuccess(code, "GetDispatchData", "DispatchService");

                return dto;
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, code, "GetDispatchData", "DispatchService");
                return new DispatchDto
                {
                    Items = new List<LineItemDto>()
                }; ;
            }
        }
        // ✅ GET IMAGE FROM DB
        public string GetMaterialImage(string matnr)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                con.Open();

                SqlCommand cmd = new SqlCommand("sp_GetMaterialImage", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Material", matnr);

                var result = cmd.ExecuteScalar();

                _logService.LogSuccess(matnr, "GetMaterialImage", "DispatchService");

                return result.ToString() ?? "";
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, matnr, "GetMaterialImage", "DispatchService");
                return "";
            }
        }
        public void SaveActualWeight(DispatchSaveDto model)
        {
            try
            {
                decimal noOfCone = Convert.ToDecimal(model.ZCONE_NO ?? "");
                decimal coneWeight = Convert.ToDecimal(model.ZCONE_WT);
                decimal actualWeight = Convert.ToDecimal(model.ACT_WEIGHT);

                decimal stdWeight = coneWeight * noOfCone;
                decimal tolerance = stdWeight * 0.05m;

                decimal minWeight = stdWeight - tolerance;
                decimal maxWeight = stdWeight + tolerance;

                if (actualWeight < minWeight || actualWeight > maxWeight)
                    throw new Exception("Weight out of tolerance");

                var dest = RfcDestinationManager.GetDestination("DEP");
                var repo = dest.Repository;

                IRfcFunction func = repo.CreateFunction("ZRFC_HU_DATA_POST");

                func.SetValue("IV_AUFNR", model.AUFNR ?? "");

                // ✅ Only important log
                _logService.LogInfo("Calling SAP ZRFC_HU_DATA_POST", model.AUFNR ?? "", "SaveActualWeight", "DispatchService");

                func.Invoke(dest);

                _logService.LogSuccess(model.AUFNR ?? "", "SaveActualWeight", "DispatchService");
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, model.AUFNR ?? "", "SaveActualWeight", "DispatchService");
                throw;
            }
        }

    }
}