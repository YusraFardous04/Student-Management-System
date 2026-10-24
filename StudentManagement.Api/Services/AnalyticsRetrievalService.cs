using System.Text.RegularExpressions;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services;

public interface IAnalyticsRetrievalService
{
    bool IsAnalyticalQuestion(string question);
    string BuildAnalyticsSummary(string question);
}

public class AnalyticsRetrievalService : IAnalyticsRetrievalService
{
    private readonly StudentDbContext _context;
    public AnalyticsRetrievalService(StudentDbContext context) => _context = context;

    private static readonly string[] Keywords =
    {
        "average", "overall", "compare", "comparison", "performance", "analy", "best", "worst",
        "highest", "lowest", "top", "bottom", "rank", "trend", "attendance", "all students"
    };

    public bool IsAnalyticalQuestion(string question) =>
        Keywords.Any(k => question.Contains(k, StringComparison.OrdinalIgnoreCase));

    public string BuildAnalyticsSummary(string question)
    {
        var grade = ExtractGrade(question);       // e.g. "9th Grade", or null
        var section = ExtractSection(question);   // e.g. "A", or null

        bool Matches(string rowGrade, string rowSection) =>
            (grade == null || string.Equals(rowGrade, grade, StringComparison.OrdinalIgnoreCase)) &&
            (section == null || string.Equals(rowSection, section, StringComparison.OrdinalIgnoreCase));

        var scope = $"{grade ?? "all grades"}, {(section != null ? "Section " + section : "all sections")}";

        // marks
        var markRows = _context.Enrollments
            .Where(e => e.Mark.HasValue)
            .Select(e => new
            {
                Subject = e.Subject!.Name,
                Student = e.Student!.FullName,
                Grade = e.Student!.GradeLevel,
                Section = e.Student!.Section,
                Mark = e.Mark!.Value
            })
            .ToList()
            .Where(r => Matches(r.Grade, r.Section))
            .ToList();

        if (markRows.Count == 0)
            return $"Analytics for {scope}: no marks were found for this group.";

        var bySubject = markRows
            .GroupBy(r => r.Subject)
            .Select(g => $"{g.Key} {g.Average(x => x.Mark):F1}");

        var byStudent = markRows
            .GroupBy(r => r.Student)
            .Select(g => new { Name = g.Key, Average = g.Average(x => x.Mark) })
            .OrderByDescending(s => s.Average)
            .ToList();

        var top = string.Join(", ", byStudent.Take(3).Select(s => $"{s.Name} ({s.Average:F1})"));
        var bottom = string.Join(", ", byStudent.TakeLast(3).Reverse().Select(s => $"{s.Name} ({s.Average:F1})"));

        // attendance
        var attendanceRows = _context.Attendances
            .Select(a => new
            {
                a.Status,
                Grade = a.Enrollment!.Student!.GradeLevel,
                Section = a.Enrollment!.Student!.Section
            })
            .ToList()
            .Where(r => Matches(r.Grade, r.Section))
            .ToList();

        var attendanceText = attendanceRows.Count == 0
            ? "no attendance records"
            : $"{attendanceRows.Count(r => r.Status == AttendanceStatus.Present) * 100.0 / attendanceRows.Count:F0}% present " +
              $"across {attendanceRows.Count} records";

        return $"Analytics for {scope} ({byStudent.Count} students with marks). " +
               $"Average mark by subject: {string.Join(", ", bySubject)}. " +
               $"Highest average students: {top}. Lowest average students: {bottom}. " +
               $"Attendance: {attendanceText}.";
    }

    // "9th grade", "grade 9", "9 grade"  ->  "9th Grade"
    private static string? ExtractGrade(string question)
    {
        var m = Regex.Match(question, @"\b(\d{1,2})\s*(?:st|nd|rd|th)?\s*grade\b", RegexOptions.IgnoreCase);
        if (!m.Success) m = Regex.Match(question, @"\bgrade\s*(\d{1,2})\b", RegexOptions.IgnoreCase);
        if (!m.Success) return null;

        int n = int.Parse(m.Groups[1].Value);
        string suffix = (n % 100 is 11 or 12 or 13) ? "th" : (n % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" };
        return $"{n}{suffix} Grade";
    }

    // "section b", "class d"  ->  "B", "D"
    private static string? ExtractSection(string question)
    {
        var m = Regex.Match(question, @"\b(?:section|class)\s*([A-Da-d])\b", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value.ToUpper() : null;
    }
}