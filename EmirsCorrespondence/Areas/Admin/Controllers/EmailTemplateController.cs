using EmirsCorrespondence.Data;
using EmirsCorrespondence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmirsCorrespondence.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class EmailTemplateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailTemplateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailTemplate/Index
        public IActionResult Index()
        {
            var templates = _context.EmailTemplates.ToList();
            return View(templates);
        }

        // GET: EmailTemplate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailTemplate template)
        {
            if (ModelState.IsValid)
            {
                template.CreatedAt = DateTime.Now;
                _context.Add(template);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }

        // GET: EmailTemplate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template = await _context.EmailTemplates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return View(template);
        }

        // POST: EmailTemplate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TemplateName,TemplateSubject,TemplateBody,TemplateType,CreatedAt,UpdatedAt")] EmailTemplate template)
        {
            if (id != template.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    template.UpdatedAt = DateTime.Now;
                    _context.Update(template);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailTemplateExists(template.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }

        // DELETE: EmailTemplate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        // POST: EmailTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id);
            _context.EmailTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Sending Emails Using Templates
        //public async Task SendEmailWithTemplate(int templateId, string recipientEmail, Dictionary<string, string> replacements)
        //{
        //    var template = await _context.EmailTemplates.FindAsync(templateId);

        //    if (template != null)
        //    {
        //        string subject = Salesemailtemplate(template.TemplateSubject, replacements);
        //        string body = ReplacePlaceholders(template.TemplateBody, replacements);

        //        var message = new MimeMessage();
        //        message.From.Add(new MailboxAddress("Your Company", "no-reply@yourcompany.com"));
        //        message.To.Add(new MailboxAddress("", recipientEmail));
        //        message.Subject = subject;
        //        message.Body = new TextPart("html") { Text = body };

        //        using (var client = new SmtpClient())
        //        {
        //            await client.ConnectAsync("smtp.yourserver.com", 587, false);
        //            await client.AuthenticateAsync("your-email@example.com", "password");
        //            await client.SendAsync(message);
        //            await client.DisconnectAsync(true);
        //        }
        //    }
        //}

        //private string ReplacePlaceholders(string text, Dictionary<string, string> replacements)
        //{
        //    foreach (var placeholder in replacements)
        //    {
        //        text = text.Replace("{" + placeholder.Key + "}", placeholder.Value);
        //    }
        //    return text;
        //}
        //public async Task SendEmailWithTemplate(int templateId, string recipientEmail, Dictionary<string, string> replacements)
        //{
        //    var template = await _context.EmailTemplates.FindAsync(templateId);

        //    if (template != null)
        //    {
        //        string subject = ReplacePlaceholders(template.TemplateSubject, replacements);
        //        string body = ReplacePlaceholders(template.TemplateBody, replacements);

        //        var message = new MimeMessage();
        //        message.From.Add(new MailboxAddress("Your Company", "no-reply@yourcompany.com"));
        //        message.To.Add(new MailboxAddress("", recipientEmail));
        //        message.Subject = subject;
        //        message.Body = new TextPart("html") { Text = body };

        //        using (var client = new SmtpClient())
        //        {
        //            await client.ConnectAsync("smtp.yourserver.com", 587, false);
        //            await client.AuthenticateAsync("your-email@example.com", "password");
        //            await client.SendAsync(message);
        //            await client.DisconnectAsync(true);
        //        }
        //    }
        //}

        //private string ReplacePlaceholders(string text, Dictionary<string, string> replacements)
        //{
        //    foreach (var placeholder in replacements)
        //    {
        //        text = text.Replace("{" + placeholder.Key + "}", placeholder.Value);
        //    }
        //    return text;
        //}


        private bool EmailTemplateExists(int id)
        {
            return _context.EmailTemplates.Any(e => e.Id == id);
        }
    }
}

