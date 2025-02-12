namespace EmirsCorrespondence.Models
{
    public class DocumentVersion
    {
        public int DocumentVersionId { get; set; }
        public int DocumentId { get; set; }
        public string VersionNumber { get; set; }
        public string FilePath { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Document Document { get; set; }
    }

}
