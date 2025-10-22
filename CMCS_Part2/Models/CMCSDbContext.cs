using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CMCS_Part2.Models
{
    public class CMCSDbContext : DbContext
    {
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options) { }

        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Example lecturer
            modelBuilder.Entity<Lecturer>().HasData(
                new Lecturer { Id = 1, Name = "Dr. Tom Hanson", Email = "tom.hanson@university.com" }
            );
        }
    }
}