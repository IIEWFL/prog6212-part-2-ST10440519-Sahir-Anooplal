using CMCS_Part2.Models;

namespace CMCS_Part2.Services
{
    public interface IUserService
    {
        Lecturer? GetCurrentLecturer();
        void SetCurrentLecturer(int lecturerId);
        List<Lecturer> GetAllLecturers();
    }
}