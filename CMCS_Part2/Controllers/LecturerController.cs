using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Models;

namespace CMCS_Part2.Controllers
{
    public class LecturerController : Controller
    {
        private readonly CMCSDbContext _context;

        public LecturerController(CMCSDbContext context)
        {
            _context = context; // [1]
        }

        // GET: Lecturer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lecturers.OrderBy(l => l.Name).ToListAsync()); // [2]
        }

        // GET: Lecturer/Create
        public IActionResult Create()
        {
            return View(); // [3]
        }

        // POST: Lecturer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lecturer lecturer)
        {
            if (ModelState.IsValid) // [4]
            {
                _context.Add(lecturer); // [1]
                await _context.SaveChangesAsync(); // [1]
                return RedirectToAction(nameof(Index)); // [5]
            }
            return View(lecturer); // [3]
        }

        // GET: Lecturer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // [6]
            }

            var lecturer = await _context.Lecturers.FindAsync(id); // [1]
            if (lecturer == null)
            {
                return NotFound(); // [6]
            }
            return View(lecturer); // [3]
        }

        // POST: Lecturer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lecturer lecturer)
        {
            if (id != lecturer.Id)
            {
                return NotFound(); // [6]
            }

            if (ModelState.IsValid) // [4]
            {
                try
                {
                    _context.Update(lecturer); // [1]
                    await _context.SaveChangesAsync(); // [1]
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LecturerExists(lecturer.Id))
                    {
                        return NotFound(); // [6]
                    }
                    else
                    {
                        throw; // [7]
                    }
                }
                return RedirectToAction(nameof(Index)); // [5]
            }
            return View(lecturer); // [3]
        }

        private bool LecturerExists(int id)
        {
            return _context.Lecturers.Any(e => e.Id == id); // [8]
        }
    }
}

/*
[1] Microsoft Docs. "DbContext Methods: Add, Update, SaveChangesAsync, FindAsync." https://learn.microsoft.com/en-us/ef/core/
[2] Microsoft Docs. "ToListAsync Method (Entity Framework Core)." https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.tolistasync
[3] Microsoft Docs. "Controller.View Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.view
[4] Microsoft Docs. "ModelState.IsValid Property." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.modelstate
[5] Microsoft Docs. "Controller.RedirectToAction Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.redirecttoaction
[6] Microsoft Docs. "Controller.NotFound Method." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.notfound
[7] Microsoft Docs. "Exception Handling in C#." https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/
[8] Microsoft Docs. "LINQ Any Method." https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any
*/
