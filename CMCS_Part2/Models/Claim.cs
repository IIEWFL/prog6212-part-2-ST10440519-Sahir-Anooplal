using System.ComponentModel.DataAnnotations;

namespace CMCS_Part2.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [Range(1, double.MaxValue, ErrorMessage = "Hours worked must be greater than 0.")]
        public double HoursWorked { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0.")]
        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        // Foreign Key
        public int LecturerId { get; set; }

        // Navigation properties
        public Lecturer? Lecturer { get; set; }
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();

        // Calculated property
        public decimal TotalAmount => (decimal)HoursWorked * HourlyRate;
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }
}