using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<ClassAssignment> ClassAssignments => Set<ClassAssignment>();
        public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Teacher → Subject (optional FK)
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.Teachers)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // ClassAssignment → Teacher
            modelBuilder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Teacher)
                .WithMany(t => t.ClassAssignments)
                .HasForeignKey(ca => ca.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // ClassAssignment → Subject
            modelBuilder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Subject)
                .WithMany(s => s.ClassAssignments)
                .HasForeignKey(ca => ca.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment → Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment → Subject
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance → Enrollment
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Enrollment)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed subjects with fixed IDs
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Math" },
                new Subject { Id = 2, Name = "Science" },
                new Subject { Id = 3, Name = "English" },
                new Subject { Id = 4, Name = "Art" },
                new Subject { Id = 5, Name = "Biology" },
                new Subject { Id = 6, Name = "Chemistry" },
                new Subject { Id = 7, Name = "Physics" },
                new Subject { Id = 8, Name = "PE" });
        }
    }
}