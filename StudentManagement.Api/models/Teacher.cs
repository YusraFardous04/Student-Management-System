using System.Collections.Generic;

namespace StudentManagement.Api.models;

public class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;

    public int? SubjectId { get; set; }
    public virtual Subject? Subject { get; set; }

    public virtual ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
}