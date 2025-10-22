using CMCS_Part2.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS_Part2.Services
{
    public class UserService : IUserService
    {
        private readonly CMCSDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "CurrentLecturerId";

        public UserService(CMCSDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context; // [1]
            _httpContextAccessor = httpContextAccessor; // [2]
        }

        public Lecturer? GetCurrentLecturer()
        {
            var lecturerId = _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKey); // [3]
            if (lecturerId.HasValue)
            {
                return _context.Lecturers.FirstOrDefault(l => l.Id == lecturerId.Value); // [1]
            }
            return null;
        }

        public void SetCurrentLecturer(int lecturerId)
        {
            _httpContextAccessor.HttpContext?.Session.SetInt32(SessionKey, lecturerId); // [3]
        }

        public List<Lecturer> GetAllLecturers()
        {
            return _context.Lecturers.ToList(); // [1]
        }
    }
}

/*
[1] Microsoft Docs. "DbContext Methods: FirstOrDefault, ToList." https://learn.microsoft.com/en-us/ef/core/querying/
[2] Microsoft Docs. "Dependency Injection in ASP.NET Core." https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
[3] Microsoft Docs. "HttpContext.Session Methods." https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession
*/
