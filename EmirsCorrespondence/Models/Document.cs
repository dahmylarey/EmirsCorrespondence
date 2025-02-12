using System.ComponentModel.DataAnnotations;

namespace EmirsCorrespondence.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public int UploadedById { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Permissions (Public, Private, Role-based)
        public string AccessLevel { get; set; } // "Public", "Private", "RoleBased"

        // Navigation Properties
        public List<DocumentVersion> Versions { get; set; }
        public string Category { get; set; }  // Example: "Finance", "HR", "Legal"
        public List<DocumentTag> Tags { get; set; } = new List<DocumentTag>();
        public Users UploadedBy { get; set; }
    }



}
