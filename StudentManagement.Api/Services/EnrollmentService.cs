using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services
{

    public interface IEnrollmentService
    {
        List<Enrollment> GetByStudent(int studentId);
        (bool Success, string? Error) Enroll(int studentId, int subjectId);
        bool Unenroll(int enrollmentId);
    }


    public class EnrollmentService:IEnrollmentService
    {
        private const int MaxSubjectsPerStudent = 10;
        private readonly StudentDbContext _context;
        public EnrollmentService(StudentDbContext context) => _context = context;

        public List<Enrollment> GetByStudent(int studentId)
             => _context.Enrollments
        .Include(e => e.Subject)
        .Where(e => e.StudentId == studentId)
        .ToList();

        public (bool Success,String? Error) Enroll(int studentId, int subjectId)
        {
            var student = _context.Students.Find(studentId);
            var subject = _context.Subjects.Find(subjectId);
            if (student is null) return (false, "Student not found");
            if (subject is null) return (false, "Subject not found");

            if (_context.Enrollments.Any(e =>e.StudentId == studentId && e.SubjectId == subjectId))
                return (false, "Student is already enrolled in this subject");

            if (_context.Enrollments.Count(e => e.StudentId == studentId) >= MaxSubjectsPerStudent)
                return (false, $"Student is already enrolled in maximum of {MaxSubjectsPerStudent}");

            _context.Enrollments.Add(new Enrollment {StudentId=studentId, SubjectId=subjectId });
            _context.SaveChanges();
            return (true, null);

        }

        public bool Unenroll(int enrollmentId)
        {
            var enrollment = _context.Enrollments.Find(enrollmentId);
            if (enrollment is null) return false;
            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return true;
        }

    }
}
