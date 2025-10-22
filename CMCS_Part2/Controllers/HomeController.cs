using Microsoft.AspNetCore.Mvc;
using CMCS_Part2.Services;
using CMCS_Part2.Models;

namespace CMCS_Part2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService; // [1]
        }

        public IActionResult Index()
        {
            var currentUser = _userService.GetCurrentLecturer();
            if (currentUser == null)
            {
                // Redirect to user selection if no user is set
                return RedirectToAction("SelectUser"); // [2]
            }

            ViewData["CurrentUserName"] = currentUser.Name;
            return View(); // [3]
        }

        public IActionResult SelectUser()
        {
            var lecturers = _userService.GetAllLecturers();
            return View(lecturers); // [3]
        }

        [HttpPost]
        public IActionResult SelectUser(int lecturerId)
        {
            _userService.SetCurrentLecturer(lecturerId);
            return RedirectToAction("Index"); // [2]
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // [4]
            return RedirectToAction("SelectUser"); // [2]
        }
    }
}

/*
[1] Microsoft Docs. "Dependency injection in ASP.NET Core." https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
[2] Microsoft Docs. "Controller.RedirectToAction Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.redirecttoaction
[3] Microsoft Docs. "Controller.View Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.view
[4] Microsoft Docs. "HttpContext.Session Property." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.session
*/
