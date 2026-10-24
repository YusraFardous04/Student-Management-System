namespace StudentManagement.Api.models
{



    public enum AttendanceStatus
    {  Present,
        Absent,
        Late}

    public class Attendance
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        public AttendanceStatus Status { get; set; }
    }
}
