using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services;

public class StudentService : IStudentServices
{
    private readonly StudentDbContext _context;

    public StudentService(StudentDbContext context)
    {
        _context = context;
    }

    public List<Student> GetALL()
    {
        return _context.Students.ToList();
    }

    public Student? GetById(int id)
    {
        return _context.Students.FirstOrDefault(s => s.Id == id);
    }

    public Student Create(Student student)
    {
        student.FullName ??= string.Empty;
        student.Email ??= string.Empty;
        student.GradeLevel ??= string.Empty;
        student.Section ??= string.Empty;
        student.PhoneNumber ??= string.Empty;
        student.EnrolledSubjectIds ??= new List<int>();

        _context.Students.Add(student);
        _context.SaveChanges();

        return student;
    }

    public bool Delete(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return false;

        // Clean up any enrollments tied to this student
        var studentEnrollments = _context.Enrollments.Where(e => e.StudentId == id);
        _context.Enrollments.RemoveRange(studentEnrollments);

        // Remove student
        _context.Students.Remove(student);
        _context.SaveChanges();
        return true;
    }
    public bool Update(int id, Student updatedStudent)
    {
        var existing = _context.Students.Find(id);
        if (existing == null) return false;

        existing.FullName = updatedStudent.FullName;
        existing.Email = updatedStudent.Email;
        existing.PhoneNumber = updatedStudent.PhoneNumber;
        existing.DateOfBirth = updatedStudent.DateOfBirth;
        existing.GradeLevel = updatedStudent.GradeLevel;
        existing.Section = updatedStudent.Section;

        _context.SaveChanges();
        return true;
    }
    public List<Student> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetALL();
        }

        query = query.Trim().ToLower();

        return _context.Students
            .Where(s => s.FullName.ToLower().Contains(query) ||
                        s.Email.ToLower().Contains(query) ||
                        s.GradeLevel.ToLower().Contains(query) ||
                        s.Section.ToLower().Contains(query))
            .ToList();
    }
}