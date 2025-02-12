namespace EmirsCorrespondence.Models
{
    public class DocumentShareLink
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        //Navigation Property
        public Document Document { get; set; }
    }
}
