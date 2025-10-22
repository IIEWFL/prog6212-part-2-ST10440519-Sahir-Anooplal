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
            _userService = userService;
        }

        public IActionResult Index()
        {
            var currentUser = _userService.GetCurrentLecturer();
            if (currentUser == null)
            {
                // Redirect to user selection if no user is set
                return RedirectToAction("SelectUser");
            }

            ViewData["CurrentUserName"] = currentUser.Name;
            return View();
        }

        public IActionResult SelectUser()
        {
            var lecturers = _userService.GetAllLecturers();
            return View(lecturers);
        }

        [HttpPost]
        public IActionResult SelectUser(int lecturerId)
        {
            _userService.SetCurrentLecturer(lecturerId);
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SelectUser");
        }
    }
}