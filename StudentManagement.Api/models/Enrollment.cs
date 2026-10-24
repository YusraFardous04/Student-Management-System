namespace StudentManagement.Api.models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }// primary key 
        public Student? Student { get; set; }// navigation property 
        
        public int SubjectId  { get; set; }
        public Subject? Subject { get; set; }

   
        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
        
        public int? Mark { get; set; }
        // 0–100, null if not graded yet
        public int Absences { get; set; } = 0;
        public List<Attendance> AttendanceRecords { get; set; } = new();
    }
}
