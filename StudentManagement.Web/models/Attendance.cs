namespace StudentManagement.Web.models
{
    public enum AttendanceStatus { Present, Absent, Late }

    public class Attendance
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
