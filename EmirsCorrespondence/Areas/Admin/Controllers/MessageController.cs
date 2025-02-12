using DreyCorrespondence.Models;
using EmirsCorrespondence.Data;
using EmirsCorrespondence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmirsCorrespondence.Areas.Admin.Controllers
{

    [Area("Admin")]

    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        // Inbox
        public IActionResult Inbox()
        {
            int userId = GetCurrentUserId();
            var messages = _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsDeletedByReceiver)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(messages);
        }

        // Sent Messages
        public IActionResult Sent()
        {
            int userId = GetCurrentUserId();
            var messages = _context.Messages
                .Where(m => m.SenderId == userId && !m.IsDeletedBySender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(messages);
        }

        // GET: Compose Message
        public IActionResult Compose()
        {

            ViewBag.Users = new SelectList(_context.Users.Where(u => u.UserId != GetCurrentUserId()), "UserId", "Username");
            return View();
        }




        // POST: Send Message
        [HttpPost]
        public async Task<IActionResult> Compose(Message message, List<IFormFile> attachments)
        {
            //var allowedExtensions = new List<string> { ".jpg", ".png", ".pdf", ".docx" };
            //var fileExtension = Path.GetExtension(file.FileName).ToLower();

            //if (!allowedExtensions.Contains(fileExtension))
            //{
            //    ModelState.AddModelError("", "Invalid file type. Allowed: .jpg, .png, .pdf, .docx");
            //    return View(message);
            //}

            if (ModelState.IsValid)
            {
                message.SenderId = GetCurrentUserId();
                _context.Messages.Add(message);
                await _context.SaveChangesAsync(); // Save message first to get MessageId





                // Handle File Uploads
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var file in attachments)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                            Directory.CreateDirectory(uploadsFolder);

                            var filePath = Path.Combine(uploadsFolder, file.FileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Save attachment details in DB
                            var attachment = new Attachment
                            {
                                MessageId = message.MessageId,
                                FileName = file.FileName,
                                FilePath = "/uploads/" + file.FileName
                            };
                            _context.Attachments.Add(attachment);
                        }
                    }
                    await _context.SaveChangesAsync();
                }




                return RedirectToAction("Sent");
            }



            return View(message);
        }


        // GET: View Message
        public IActionResult Details(int id)
        {
            var message = _context.Messages.Include(m => m.Sender).Include(m => m.Receiver).FirstOrDefault(m => m.MessageId == id);
            if (message == null) return NotFound();

            // Mark as read
            if (message.ReceiverId == GetCurrentUserId())
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return View(message);
        }

        // POST: Delete Message
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var message = _context.Messages.Find(id);
            if (message == null) return NotFound();

            int userId = GetCurrentUserId();
            if (message.SenderId == userId)
                message.IsDeletedBySender = true;
            if (message.ReceiverId == userId)
                message.IsDeletedByReceiver = true;

            if (message.IsDeletedBySender && message.IsDeletedByReceiver)
                _context.Messages.Remove(message);

            _context.SaveChanges();
            return RedirectToAction("Inbox");
        }



        // POST: DeleteAttachment

        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            if (attachment == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = attachment.MessageId });
        }




    }
}
