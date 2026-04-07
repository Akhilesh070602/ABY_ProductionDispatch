namespace PackingDisplay.Models
{
    public class ErrorLog
    {
        public int? Id { get; set; }

        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public string? PoNo { get; set; }
        public string? ApiName { get; set; }
        public string? ServiceName { get; set; }

        public string? Status { get; set; }
        public string? RequestData { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
