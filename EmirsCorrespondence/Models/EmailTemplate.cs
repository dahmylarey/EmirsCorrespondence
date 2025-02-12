namespace EmirsCorrespondence.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }  // Name of the template
        public string TemplateSubject { get; set; }  // Subject of the email
        public string TemplateBody { get; set; }  // Body of the email in HTML format
        public string TemplateType { get; set; }  // E.g., "Reminder", "Announcement"
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
