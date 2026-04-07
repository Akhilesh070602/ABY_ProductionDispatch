namespace PackingDisplay.Models
{
    public class SAPConnectionConfig
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? SystemId { get; set; }
        public string? SystemNumber { get; set; }
        public string? AppServerHost { get; set; }
        public string? SAPRouter { get; set; }
        public string? Client { get; set; }
        public string? Language { get; set; }

        public string? User { get; set; }
        public string? Password { get; set; }

        public int? PoolSize { get; set; }

        public string? createdBy { get; set; }
        public DateTime? createdAt { get; set; }
        public string? updatedBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? PeakConnectionLimit { get; set; }
        public int? ConnectionIdleTimeout { get; set; }
        public bool? IsActive { get; set; }
    }
}