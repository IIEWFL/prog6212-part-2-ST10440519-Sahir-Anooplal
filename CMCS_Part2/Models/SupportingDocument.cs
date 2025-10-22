namespace CMCS_Part2.Models
{
    public class SupportingDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty; // Unique name on server
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int ClaimId { get; set; }

        // Navigation property
        public Claim? Claim { get; set; }
    }
}