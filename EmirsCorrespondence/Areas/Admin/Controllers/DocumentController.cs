using EmirsCorrespondence.Data;
using EmirsCorrespondence.Hubs;
using EmirsCorrespondence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace EmirsCorrespondence.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        private string category;

        public DocumentController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }


        //sorting and query
        [HttpGet]
        public async Task<IActionResult> Index(string search, string filter, string sort)
        {
            var query = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.Title.Contains(search));

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(d => d.AccessLevel == filter);

            query = sort switch
            {
                "date_asc" => query.OrderBy(d => d.UploadedAt),
                "date_desc" => query.OrderByDescending(d => d.UploadedAt),
                "size_asc" => query.OrderBy(d => d.FileSize),
                "size_desc" => query.OrderByDescending(d => d.FileSize),
                _ => query
            };

            var documents = await query.Include(d => d.UploadedBy).ToListAsync();
            return View(documents);
        }



        //for File Delete
        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null) return NotFound();
            await LogAction(id, "Deleted");

            // Ensure only Admins or Owners can delete
            if (!User.IsInRole("Admin") && document.UploadedById != GetCurrentUserId())
                return Unauthorized();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Bulk Uploads
        [HttpPost]
        public async Task<IActionResult> BulkUpload(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files selected." });

            var uploadedDocuments = new List<Document>();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new Document
                {
                    Title = Path.GetFileNameWithoutExtension(file.FileName),
                    FileName = file.FileName,
                    FilePath = "/documents/" + uniqueFileName,
                    FileType = file.ContentType,
                    FileSize = file.Length,
                    UploadedById = GetCurrentUserId(),
                    UploadedAt = DateTime.UtcNow
                };

                uploadedDocuments.Add(document);
            }

            _context.Documents.AddRange(uploadedDocuments);
            await _context.SaveChangesAsync();

            //notify users after file upload completion.
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"{uploadedDocuments.Count} files uploaded successfully.");

            return Ok(new { message = $"{uploadedDocuments.Count} files uploaded successfully." });
        }



        //for File Uploads and tagging
        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file, string title, string accessLevel, string category, string tags)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                Title = title,
                FileName = file.FileName,
                FilePath = "/documents/" + uniqueFileName,
                FileType = file.ContentType,
                FileSize = file.Length,
                UploadedById = GetCurrentUserId(),
                AccessLevel = accessLevel,
                Category = category
            };

            if (!string.IsNullOrEmpty(tags))
            {
                document.Tags = tags.Split(',').Select(tag => new DocumentTag { TagName = tag.Trim() }).ToList();
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        //to support versioning when replacing documents.
        [HttpPost]
        public async Task<IActionResult> UpdateDocument(int documentId, IFormFile newFile)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return NotFound();

            // Save old version
            var version = new DocumentVersion
            {
                DocumentId = document.DocumentId,
                VersionNumber = "v" + (await _context.DocumentVersions.CountAsync(dv => dv.DocumentId == documentId) + 1),
                FilePath = document.FilePath
            };
            _context.DocumentVersions.Add(version);

            // Upload new file
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
            var uniqueFileName = $"{Guid.NewGuid()}_{newFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newFile.CopyToAsync(stream);
            }

            document.FileName = newFile.FileName;
            document.FilePath = "/documents/" + uniqueFileName;
            document.FileType = newFile.ContentType;
            document.FileSize = newFile.Length;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Track Activities
        private async Task LogAction(int documentId, string action)
        {
            var log = new DocumentLog
            {
                DocumentId = documentId,
                Action = action,
                UserId = GetCurrentUserId()
            };
            _context.DocumentLogs.Add(log);
            await _context.SaveChangesAsync();
        }


        //Extractor text From PDF
        //private string ExtractTextFromPdf(string filePath)
        //{
        //    using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        //    using var document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly);
        //    string extractedText = "";

        //    foreach (var page in document.Pages)
        //    {
        //        using var image = new MemoryStream();
        //        var renderer = new PdfPageRenderer(page);
        //        renderer.Render(page, image);
        //        using var pix = Pix.LoadFromMemory(image.ToArray());
        //        using var pageText = engine.Process(pix);
        //        extractedText += pageText.GetText();
        //    }

        //    return extractedText;
        //}

        [HttpPost]
        public async Task<IActionResult> GenerateShareLink(int documentId, int expiryHours)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return NotFound();

            var shareLink = new DocumentShareLink
            {
                DocumentId = documentId,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddHours(expiryHours)
            };

            _context.DocumentShareLinks.Add(shareLink);
            await _context.SaveChangesAsync();

            return Ok(new { Link = Url.Action("DownloadShared", "Document", new { token = shareLink.Token }, Request.Scheme) });
        }


        //Document Sharing
        [HttpGet]
        public async Task<IActionResult> DownloadShared(string token)
        {
            var shareLink = await _context.DocumentShareLinks
                .Include(d => d.Document)
                .FirstOrDefaultAsync(s => s.Token == token && s.ExpiryDate > DateTime.UtcNow);

            if (shareLink == null) return NotFound("Link expired or invalid");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", shareLink.Document.FilePath.TrimStart('/'));
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, shareLink.Document.FileType, shareLink.Document.FileName);
        }

        //

        public IActionResult Index()
        {

            return View();
        }
    }
}
