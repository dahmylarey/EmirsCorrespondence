using EmirsCorrespondence.Models;
using System.ComponentModel.DataAnnotations;
using Attachment = EmirsCorrespondence.Models.Attachment;

namespace DreyCorrespondence.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeletedBySender { get; set; } = false;
        public bool IsDeletedByReceiver { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Sent";

        // Navigation Properties
        public Users Sender { get; set; }
        public Users Receiver { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }
}
