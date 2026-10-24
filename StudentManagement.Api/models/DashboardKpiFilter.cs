namespace StudentManagement.Api.models;

public class DashboardKpiFilter
{
    public string? GradeLevel { get; set; }
    public string? Section { get; set; }
    public int? SubjectId { get; set; }
}

public class MonthlyKpiPoint
{
    public string Month { get; set; } = string.Empty; // e.g. "Sep", "Oct"
    public double AverageMarks { get; set; }          // Percentage or score
    public double AttendanceRate { get; set; }        // Percentage
}

public class SchoolKpiViewModel
{
    public string SelectedGrade { get; set; } = "All";
    public string SelectedSection { get; set; } = "All";
    public int? SelectedSubjectId { get; set; }

    public List<MonthlyKpiPoint> KpiData { get; set; } = new();
}