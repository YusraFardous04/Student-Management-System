using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services;


public interface IClassAssignmentService
{
    List<ClassAssignment> GetByTeacher(int teacherId);
    void Assign(int teacherId, int subjectId, string gradeLevel, string section);
    void RemoveByTeacher(int teacherId);
    void RemoveAssignment(int assignmentId);
    bool Delete(int id);
}
public class ClassAssignmentService : IClassAssignmentService
{
    private readonly StudentDbContext _context;

    public ClassAssignmentService(StudentDbContext context)
    {
        _context = context;
    }

    public List<ClassAssignment> GetByTeacher(int teacherId)
    {
        return _context.ClassAssignments.Where(ca => ca.TeacherId == teacherId).ToList();
    }

    public void Assign(int teacherId, int subjectId, string gradeLevel, string section)
    {
        _context.ClassAssignments.Add(new ClassAssignment
        {
            TeacherId = teacherId,
            SubjectId = subjectId,
            GradeLevel = gradeLevel,
            Section = section
        });
        _context.SaveChanges();
    }

    public void RemoveByTeacher(int teacherId)
    {
        var assignments = _context.ClassAssignments.Where(ca => ca.TeacherId == teacherId);
        _context.ClassAssignments.RemoveRange(assignments);
        _context.SaveChanges();
    }

    public void RemoveAssignment(int assignmentId)
    {
        var assignment = _context.ClassAssignments.Find(assignmentId);
        if (assignment != null)
        {
            _context.ClassAssignments.Remove(assignment);
            _context.SaveChanges();
        }
    }

    public bool Delete(int id)
    {
        var assignment = _context.ClassAssignments.Find(id);
        if (assignment == null) return false;

        _context.ClassAssignments.Remove(assignment);
        _context.SaveChanges();
        return true;
    }
}