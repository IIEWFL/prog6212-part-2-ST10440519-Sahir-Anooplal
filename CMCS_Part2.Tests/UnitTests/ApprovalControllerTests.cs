using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Controllers;
using CMCS_Part2.Models;
using CMCS_Part2.Tests.TestModels;

namespace CMCS_Part2.Tests.UnitTests
{
    [TestClass]
    public class ApprovalControllerTests
    {
        private CMCSDbContext _context = null!;
        private ApprovalController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: "ApprovalTestDatabase")
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();

            // Seed test data
            var lecturer = new Lecturer { Id = 1, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);

            var pendingClaim = new Claim { Id = 1, LecturerId = 1, HoursWorked = 10, HourlyRate = 500.00m, Status = ClaimStatus.Pending };
            var approvedClaim = new Claim { Id = 2, LecturerId = 1, HoursWorked = 15, HourlyRate = 450.00m, Status = ClaimStatus.Approved };

            _context.Claims.AddRange(pendingClaim, approvedClaim);
            _context.SaveChanges();

            _controller = new ApprovalController(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [TestMethod]
        public async Task Index_ReturnsOnlyPendingClaims()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as IEnumerable<Claim>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count()); // Only one pending claim
            Assert.IsTrue(model.All(c => c.Status == ClaimStatus.Pending));
        }

        [TestMethod]
        public async Task Approve_ValidClaim_UpdatesStatus()
        {
            // Act
            var result = await _controller.Approve(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Verify status was updated
            var updatedClaim = await _context.Claims.FindAsync(1);
            Assert.AreEqual(ClaimStatus.Approved, updatedClaim?.Status);
        }

        [TestMethod]
        public async Task Reject_ValidClaim_UpdatesStatus()
        {
            // Act
            var result = await _controller.Reject(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Verify status was updated
            var updatedClaim = await _context.Claims.FindAsync(1);
            Assert.AreEqual(ClaimStatus.Rejected, updatedClaim?.Status);
        }
    }
}