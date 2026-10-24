using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Api.models
{
    public class ClassAssignment
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public string GradeLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;

        [NotMapped]
        public string SubjectName { get; set; } = string.Empty;
    }
}