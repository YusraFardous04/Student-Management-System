using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Web.models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ClassAssignment> ClassAssignments { get; set; } = new();

    }
}
