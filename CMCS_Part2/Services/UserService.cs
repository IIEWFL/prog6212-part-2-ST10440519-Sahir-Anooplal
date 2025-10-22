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
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Lecturer? GetCurrentLecturer()
        {
            var lecturerId = _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKey);
            if (lecturerId.HasValue)
            {
                return _context.Lecturers.FirstOrDefault(l => l.Id == lecturerId.Value);
            }
            return null;
        }

        public void SetCurrentLecturer(int lecturerId)
        {
            _httpContextAccessor.HttpContext?.Session.SetInt32(SessionKey, lecturerId);
        }

        public List<Lecturer> GetAllLecturers()
        {
            return _context.Lecturers.ToList();
        }
    }
}