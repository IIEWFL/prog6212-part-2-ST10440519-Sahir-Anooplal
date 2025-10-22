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
            var databaseName = $"ApprovalTestDatabase_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<CMCSDbContext>() // [1]
                .UseInMemoryDatabase(databaseName: databaseName) // [2]
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated(); // [3]
            _controller = new ApprovalController(_context); // [4]
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
            // Arrange
            var lecturer = new Lecturer { Id = 100, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);

            var pendingClaim = new Claim { Id = 100, LecturerId = 100, HoursWorked = 10, HourlyRate = 500.00m, Status = ClaimStatus.Pending };
            var approvedClaim = new Claim { Id = 101, LecturerId = 100, HoursWorked = 15, HourlyRate = 450.00m, Status = ClaimStatus.Approved };

            _context.Claims.AddRange(pendingClaim, approvedClaim);
            await _context.SaveChangesAsync(); // [5]

            // Act
            var result = await _controller.Index(); // [6]

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult)); // [7]
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as IEnumerable<Claim>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count());
            Assert.IsTrue(model.All(c => c.Status == ClaimStatus.Pending));
        }

        [TestMethod]
        public async Task Approve_NonExistentClaim_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Approve(999); // [8]

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult)); // [9]
        }

        [TestMethod]
        public async Task Reject_NonExistentClaim_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Reject(999); // [10]

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult)); // [11]
        }
    }
}

/*
[1] Microsoft, "DbContextOptionsBuilder Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder
[2] Microsoft, "UseInMemoryDatabase Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.inmemorydatabasebuilderextensions.useinmemorydatabase
[3] Microsoft, "EnsureCreated Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade.ensurecreated
[4] CMCS_Part2.Controllers, "ApprovalController Constructor," Internal Project Documentation, 2025.
[5] Microsoft, "SaveChangesAsync Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechangesasync
[6] CMCS_Part2.Controllers, "ApprovalController.Index Method," Internal Project Documentation, 2025.
[7] Microsoft, "Assert.IsInstanceOfType Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.isinstanceoftype
[8] CMCS_Part2.Controllers, "ApprovalController.Approve Method," Internal Project Documentation, 2025.
[9] Microsoft, "NotFoundResult Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.notfoundresult
[10] CMCS_Part2.Controllers, "ApprovalController.Reject Method," Internal Project Documentation, 2025.
[11] Microsoft, "Assert Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert
*/
