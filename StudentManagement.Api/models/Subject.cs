namespace StudentManagement.Api.models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Teacher> Teachers { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<ClassAssignment> ClassAssignments { get; set; } = new();
    }
}