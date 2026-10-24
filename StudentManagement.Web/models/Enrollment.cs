
namespace StudentManagement.Web.models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public DateTime EnrolledOn { get; set; }
        public int? Mark { get; set; } // 0–100, null if not graded yet

        public int Absences { get; set; } = 0; // 👈 Add this property

        public List<Attendance> AttendanceRecords { get; set; } = new();

    }
}
