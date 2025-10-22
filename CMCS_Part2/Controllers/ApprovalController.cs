using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Models;

namespace CMCS_Part2.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly CMCSDbContext _context;

        public ApprovalController(CMCSDbContext context)
        {
            _context = context;
        }

        // GET: Approval - Shows all pending claims for review
        public async Task<IActionResult> Index()
        {
            var pendingClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.SupportingDocuments)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            return View(pendingClaims);
        }

        // GET: Approval/All - Shows all claims (for overview)
        public async Task<IActionResult> All()
        {
            var allClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.SupportingDocuments)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(allClaims);
        }

        // POST: Approval/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Approved;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Claim #{claim.Id} from {claim.Lecturer?.Name} has been approved.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Approval/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Rejected;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Claim #{claim.Id} from {claim.Lecturer?.Name} has been rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}