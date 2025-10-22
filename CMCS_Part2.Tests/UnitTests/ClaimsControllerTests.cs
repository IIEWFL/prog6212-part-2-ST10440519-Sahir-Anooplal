using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Controllers;
using CMCS_Part2.Models;
using CMCS_Part2.Services;
using CMCS_Part2.Tests.TestModels;

namespace CMCS_Part2.Tests.UnitTests
{
    [TestClass]
    public class ClaimsControllerTests
    {
        private CMCSDbContext _context = null!;
        private ClaimsController _controller = null!;
        private MockUserService _userService = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: "ControllerTestDatabase")
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();

            // Seed test data
            var lecturer = new Lecturer { Id = 1, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);
            _context.SaveChanges();

            _userService = new MockUserService(_context);
            _controller = new ClaimsController(_context, _userService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithClaims_ForCurrentUser()
        {
            // Arrange
            _userService.SetCurrentLecturer(1);

            // Add test claims
            var claim = new Claim { LecturerId = 1, HoursWorked = 10, HourlyRate = 500.00m };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
        }

        [TestMethod]
        public async Task Create_ValidClaim_RedirectsToIndex()
        {
            // Arrange
            _userService.SetCurrentLecturer(1);
            var claim = new Claim { HoursWorked = 20, HourlyRate = 450.00m, Notes = "Test claim" };

            // Act
            var result = await _controller.Create(claim, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Create_InvalidClaim_ReturnsViewWithModel()
        {
            // Arrange
            _userService.SetCurrentLecturer(1);
            var claim = new Claim { HoursWorked = 0, HourlyRate = 450.00m }; // Invalid hours
            _controller.ModelState.AddModelError("HoursWorked", "Required");

            // Act
            var result = await _controller.Create(claim, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(claim, viewResult.Model);
        }
    }

    // Example User Service for testing
    public class MockUserService : IUserService
    {
        private readonly CMCSDbContext _context;
        private int? _currentLecturerId;

        public MockUserService(CMCSDbContext context)
        {
            _context = context;
        }

        public Lecturer? GetCurrentLecturer()
        {
            if (_currentLecturerId.HasValue)
            {
                return _context.Lecturers.FirstOrDefault(l => l.Id == _currentLecturerId.Value);
            }
            return null;
        }

        public void SetCurrentLecturer(int lecturerId)
        {
            _currentLecturerId = lecturerId;
        }

        public List<Lecturer> GetAllLecturers()
        {
            return _context.Lecturers.ToList();
        }
    }
}