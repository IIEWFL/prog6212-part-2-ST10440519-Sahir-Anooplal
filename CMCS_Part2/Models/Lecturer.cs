using System.Security.Claims;

namespace CMCS_Part2.Models
{
    public class Lecturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}