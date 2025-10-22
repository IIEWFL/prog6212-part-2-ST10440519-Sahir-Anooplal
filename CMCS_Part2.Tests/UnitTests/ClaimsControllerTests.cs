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
            var databaseName = $"ControllerTestDatabase_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<CMCSDbContext>() // [1]
                .UseInMemoryDatabase(databaseName: databaseName) // [2]
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated(); // [3]

            _userService = new MockUserService(_context); // [4]
            _controller = new ClaimsController(_context, _userService); // [5]
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
            var lecturer = new Lecturer { Id = 5, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync(); // [6]

            _userService.SetCurrentLecturer(5); // [7]

            var claim = new Claim { Id = 5, LecturerId = 5, HoursWorked = 10, HourlyRate = 500.00m };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            var result = await _controller.Index(); // [8]

            Assert.IsInstanceOfType(result, typeof(ViewResult)); // [9]
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
        }

        [TestMethod]
        public async Task Create_InvalidClaim_ReturnsViewWithModel()
        {
            var lecturer = new Lecturer { Id = 7, Name = "Test Lecturer", Email = "test@university.com" };
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            _userService.SetCurrentLecturer(7);
            var claim = new Claim { HoursWorked = 0, HourlyRate = 450.00m };

            _controller.ModelState.AddModelError("HoursWorked", "Hours must be greater than 0"); // [10]

            var result = await _controller.Create(claim, null); // [11]

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(claim, viewResult.Model); // [12]
        }

        [TestMethod]
        public async Task Index_NoCurrentUser_RedirectsToSelectUser()
        {
            var result = await _controller.Index();

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult)); // [13]
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("SelectUser", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);
        }
    }

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
                return _context.Lecturers.FirstOrDefault(l => l.Id == _currentLecturerId.Value); // [14]
            }
            return null;
        }

        public void SetCurrentLecturer(int lecturerId)
        {
            _currentLecturerId = lecturerId;
        }

        public List<Lecturer> GetAllLecturers()
        {
            return _context.Lecturers.ToList(); // [15]
        }
    }
}

/*
[1] Microsoft, "DbContextOptionsBuilder Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder
[2] Microsoft, "UseInMemoryDatabase Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.inmemorydatabasebuilderextensions.useinmemorydatabase
[3] Microsoft, "EnsureCreated Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade.ensurecreated
[4] CMCS_Part2.Tests.UnitTests, "MockUserService Constructor," Internal Project Documentation, 2025.
[5] CMCS_Part2.Controllers, "ClaimsController Constructor," Internal Project Documentation, 2025.
[6] Microsoft, "DbContext.SaveChangesAsync Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechangesasync
[7] CMCS_Part2.Tests.UnitTests, "MockUserService.SetCurrentLecturer Method," Internal Project Documentation, 2025.
[8] CMCS_Part2.Controllers, "ClaimsController.Index Method," Internal Project Documentation, 2025.
[9] Microsoft, "Assert.IsInstanceOfType Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.isinstanceoftype
[10] Microsoft, "ModelStateDictionary.AddModelError Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelstatedictionary.addmodelerror
[11] CMCS_Part2.Controllers, "ClaimsController.Create Method," Internal Project Documentation, 2025.
[12] Microsoft, "Assert.AreEqual Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.areequal
[13] Microsoft, "RedirectToActionResult Class," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.redirecttoactionresult
[14] Microsoft, "Queryable.FirstOrDefault Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.linq.queryable.firstordefault
[15] Microsoft, "Queryable.ToList Method," Microsoft Docs, 2024. [Online]. Available: https://learn.microsoft.com/en-us/dotnet/api/system.linq.queryable.tolist
*/
