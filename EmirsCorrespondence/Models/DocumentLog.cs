namespace EmirsCorrespondence.Models
{
    public class DocumentLog
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Action { get; set; } // Uploaded, Downloaded, Deleted, Edited
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Document Document { get; set; }
        public Users User { get; set; }
    }

}
