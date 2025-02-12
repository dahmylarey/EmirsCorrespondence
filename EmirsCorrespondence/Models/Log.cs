namespace EmirsCorrespondence.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation Properties
        public Users User { get; set; }
    }
}
