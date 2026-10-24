namespace StudentManagement.Api.models
{
    public class Marks
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public decimal Mark { get; set; } = 100;
    }
}
