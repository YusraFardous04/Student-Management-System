using System.Collections.Generic;

namespace StudentManagement.Web.models;

public class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;

    public int? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public List<ClassAssignment> ClassAssignments { get; set; } = new();
}
