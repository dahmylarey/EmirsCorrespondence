using DreyCorrespondence.Models;

namespace EmirsCorrespondence.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public int MessageId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Message Message { get; set; }
    }
}
