namespace EmirsCorrespondence.Models
{
    public class DocumentTag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
