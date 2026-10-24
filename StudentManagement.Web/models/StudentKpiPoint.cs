namespace StudentManagement.Web.models;

public class StudentKpiPoint
{
    public string StudentName { get; set; } = string.Empty; // X-Axis
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public double Marks { get; set; }          // Blue Line
    public double AttendanceRate { get; set; } // Green Line
}