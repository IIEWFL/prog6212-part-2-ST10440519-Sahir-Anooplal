using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Models;
using CMCS_Part2.Tests.TestModels;

namespace CMCS_Part2.Tests.UnitTests
{
    [TestClass]
    public class ClaimTests
    {
        private CMCSDbContext _context = null!; 

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();

            // Seed test data
            var lecturer = new Lecturer { Id = 1, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [TestMethod]
        public void Claim_TotalAmount_CalculatesCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 500.00m, // R500 per hour
                LecturerId = 1
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.AreEqual(20000.00m, totalAmount); // 40 * 500 = 20,000
        }

        [TestMethod]
        public void Claim_DefaultStatus_IsPending()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.AreEqual(ClaimStatus.Pending, claim.Status);
        }

        [TestMethod]
        public void Claim_ValidHoursWorked_ValidationPasses()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 1, // Minimum valid value
                HourlyRate = 100.00m
            };

            // Act
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(claim);
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Claim_InvalidHoursWorked_ValidationFails()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 0, // Invalid - must be greater than 0
                HourlyRate = 100.00m
            };

            // Act
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(claim);
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Any(vr => vr.MemberNames.Contains("HoursWorked")));
        }
    }
}