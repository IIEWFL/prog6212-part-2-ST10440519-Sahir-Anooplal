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
            _context = context; // [1]
            _userService = userService; // [2]
        }

        // GET: Claims - Shows list of claims for the CURRENT lecturer
        public async Task<IActionResult> Index()
        {
            var currentLecturer = _userService.GetCurrentLecturer(); 
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home"); // [3]
            }

            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.SupportingDocuments)
                .Where(c => c.LecturerId == currentLecturer.Id)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync(); // [4]

            ViewData["CurrentLecturer"] = currentLecturer;
            return View(claims); // [5]
        }

        // GET: Claims/Create - Show claim submission form
        public IActionResult Create()
        {
            var currentLecturer = _userService.GetCurrentLecturer(); 
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home"); // [3]
            }
            return View(); // [5]
        }

        // POST: Claims/Create - Handle claim submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? supportingDocument)
        {
            var currentLecturer = _userService.GetCurrentLecturer(); 
            if (currentLecturer == null)
            {
                return RedirectToAction("SelectUser", "Home"); // [3]
            }

            if (ModelState.IsValid) // [6]
            {
                // Set lecturer ID from current user
                claim.LecturerId = currentLecturer.Id;
                claim.Status = ClaimStatus.Pending;

                _context.Add(claim); // [7]
                await _context.SaveChangesAsync(); // [8]

                // Handle file upload if present
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    await HandleFileUpload(supportingDocument, claim.Id); // [9]
                }

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index)); // [3]
            }
            return View(claim); // [5]
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
                await file.CopyToAsync(stream); // [9]
            }

            // Save document info to database
            var document = new SupportingDocument
            {
                FileName = file.FileName,
                StoredFileName = storedFileName,
                ClaimId = claimId
            };

            _context.SupportingDocuments.Add(document); // [7]
            await _context.SaveChangesAsync(); // [8]
        }
    }
}

/*
[1] Microsoft Docs. "Dependency injection in ASP.NET Core." https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
[2] Microsoft Docs. "Interfaces (C# Programming Guide)." https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/
[3] Microsoft Docs. "Controller.RedirectToAction Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.redirecttoaction
[4] Microsoft Docs. "ToListAsync Method (Entity Framework Core)." https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.tolistasync
[5] Microsoft Docs. "Controller.View Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.view
[6] Microsoft Docs. "ModelState.IsValid Property." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.modelstate
[7] Microsoft Docs. "DbContext.Add Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.add
[8] Microsoft Docs. "DbContext.SaveChangesAsync Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechangesasync
[9] Microsoft Docs. "IFormFile.CopyToAsync Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.iformfile.copytoasync
*/
