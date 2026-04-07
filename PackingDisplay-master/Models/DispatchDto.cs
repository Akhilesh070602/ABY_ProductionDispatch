
namespace PackingDisplay.Models
{
    public class DispatchDto
    {
        public string? PoNo { get; set; }
        public string? HuCode { get; set; }
        public string? EXIDV { get; set; }
        public string? SONUM { get; set; }
        public string? WERKS { get; set; }
        public string? Material { get; set; }
        public string? Cone { get; set; }
        public decimal? StdWeight { get; set; }
        public decimal? ActualWeight { get; set; }
        public string? Vemeh { get; set; }

        public List<LineItemDto>? Items { get; set; } = new();
    }

    // 🔹 LINE ITEMS
    public class LineItemDto
    {
        public string? Name { get; set; }
        //public decimal StdWeight { get; set; }     // per cone
        public decimal? ConeWeight { get; set; }    // total
        public string? Material { get; set; }
    }



    public class DispatchSaveDto
    {
        public string? AUFNR { get; set; }
        public string? VENUM { get; set; }
        public string? EXIDV { get; set; }
        public string? SONUM { get; set; }

        public string? MATNR { get; set; }
        public string? WERKS { get; set; }
        public string? KDMAT { get; set; }
        public decimal? STD_WEIGHT { get; set; }
        public string? VEMEH { get; set; }
        public string? ZCONE_NO { get; set; }
        public decimal? ZCONE_WT { get; set; }
        public decimal? ACT_WEIGHT { get; set; }
    }
}
