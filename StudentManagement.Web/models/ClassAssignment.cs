using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Web.models; // Adjust to .Web.models in Web project

public class ClassAssignment
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }

    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;

    [NotMapped]
    public string SubjectName { get; set; } = string.Empty;

    public Teacher? Teacher { get; set; }
    public Subject? Subject { get; set; }
}