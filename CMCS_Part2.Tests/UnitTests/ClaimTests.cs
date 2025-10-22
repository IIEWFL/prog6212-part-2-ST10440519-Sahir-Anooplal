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
            var databaseName = $"TestDatabase_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<CMCSDbContext>() // [1]
                .UseInMemoryDatabase(databaseName: databaseName) // [2]
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated(); // [3]

            var lecturer = new Lecturer { Id = 100, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);
            _context.SaveChanges(); // [4]
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
                HourlyRate = 500.00m,
                LecturerId = 100
            };

            // Act
            var totalAmount = claim.TotalAmount; // [5]

            // Assert
            Assert.AreEqual(20000.00m, totalAmount); // [6]
        }

        [TestMethod]
        public void Claim_DefaultStatus_IsPending()
        {
            var claim = new Claim();
            Assert.AreEqual(ClaimStatus.Pending, claim.Status); // [7]
        }

        [TestMethod]
        public void Claim_ValidHoursWorked_ValidationPasses()
        {
            var claim = new Claim
            {
                HoursWorked = 1,
                HourlyRate = 100.00m
            };

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(claim); // [8]
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(claim, validationContext, validationResults, true); // [9]

            Assert.IsTrue(isValid); // [10]
        }

        [TestMethod]
        public void Claim_InvalidHoursWorked_ValidationFails()
        {
            var claim = new Claim
            {
                HoursWorked = 0,
                HourlyRate = 100.00m
            };

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(claim);
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(claim, validationContext, validationResults, true); // [11]

            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Any(vr => vr.MemberNames.Contains("HoursWorked"))); // [12]
        }
    }
}

/*
[1] Microsoft, "DbContextOptionsBuilder Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder
[2] Microsoft, "UseInMemoryDatabase Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.inmemorydatabasebuilderextensions.useinmemorydatabase
[3] Microsoft, "EnsureCreated Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade.ensurecreated
[4] Microsoft, "DbContext.SaveChanges Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechanges
[5] CMCS_Part2.Models, "Claim.TotalAmount Property," Internal Project Documentation, 2025.
[6] Microsoft, "Assert.AreEqual Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.areequal
[7] CMCS_Part2.Models, "Claim.Status Property," Internal Project Documentation, 2025.
[8] Microsoft, "ValidationContext Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationcontext
[9] Microsoft, "Validator.TryValidateObject Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator.tryvalidateobject
[10] Microsoft, "Assert.IsTrue Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.istrue
[11] Microsoft, "Validator.TryValidateObject Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator.tryvalidateobject
[12] Microsoft, "Enumerable.Contains Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.contains
*/
