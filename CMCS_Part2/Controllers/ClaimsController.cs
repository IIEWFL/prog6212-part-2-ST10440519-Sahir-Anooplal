using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Models;
using CMCS_Part2.Services;

namespace CMCS_Part2.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly CMCSDbContext _context;
        private readonly IUserService _userService;

        public ClaimsController(CMCSDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Claims - Shows list of claims for the CURRENT lecturer
        public async Task<IActionResult> Index()
        {
            var currentLecturer = _userService.GetCurrentLecturer();
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home");
            }

            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.SupportingDocuments)
                .Where(c => c.LecturerId == currentLecturer.Id)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            ViewData["CurrentLecturer"] = currentLecturer;
            return View(claims);
        }

        // GET: Claims/Create - Show claim submission form
        public IActionResult Create()
        {
            var currentLecturer = _userService.GetCurrentLecturer();
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home");
            }
            return View();
        }

        // POST: Claims/Create - Handle claim submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? supportingDocument)
        {
            var currentLecturer = _userService.GetCurrentLecturer();
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home");
            }

            if (ModelState.IsValid)
            {
                // Set lecturer ID from current user
                claim.LecturerId = currentLecturer.Id;
                claim.Status = ClaimStatus.Pending;

                _context.Add(claim);
                await _context.SaveChangesAsync();

                // Handle file upload if present
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    await HandleFileUpload(supportingDocument, claim.Id);
                }

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        private async Task HandleFileUpload(IFormFile file, int claimId)
        {
            // Validate file type and size
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type. Only PDF, DOCX, and XLSX files are allowed.";
                return;
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
            {
                TempData["ErrorMessage"] = "File size too large. Maximum size is 5MB.";
                return;
            }

            // Generate unique filename
            var storedFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, storedFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save document info to database
            var document = new SupportingDocument
            {
                FileName = file.FileName,
                StoredFileName = storedFileName,
                ClaimId = claimId
            };

            _context.SupportingDocuments.Add(document);
            await _context.SaveChangesAsync();
        }
    }
}