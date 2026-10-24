using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    // Absences are stored as a count, so an attendance rate needs a number of school days to compare against.
    // Change this to match your school year.
    private const int SchoolDays = 180;

    private readonly StudentDbContext _context;

    public DashboardController(StudentDbContext context)
    {
        _context = context;
    }

    [HttpGet("school-performance")]
    public async Task<ActionResult<List<StudentKpiPoint>>> GetSchoolPerformance(
        [FromQuery] string? grade,
        [FromQuery] string? section,
        [FromQuery] string? subject)
    {
        var studentQuery = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(grade))
            studentQuery = studentQuery.Where(s => s.GradeLevel == grade);

        if (!string.IsNullOrWhiteSpace(section))
            studentQuery = studentQuery.Where(s => s.Section == section);

        var students = await studentQuery
            .Select(s => new { s.Id, s.FullName, s.GradeLevel, s.Section })
            .ToListAsync();

        var studentIds = students.Select(s => s.Id).ToList();

        // Real enrollments (optionally only the chosen subject)
        var enrollmentQuery = _context.Enrollments.Where(e => studentIds.Contains(e.StudentId));
        if (!string.IsNullOrWhiteSpace(subject))
            enrollmentQuery = enrollmentQuery.Where(e => e.Subject!.Name == subject);

        var enrollments = await enrollmentQuery
            .Select(e => new { e.Id, e.StudentId, e.Mark, e.Absences })
            .ToListAsync();

        // Daily attendance records, if any have been entered
        var enrollmentOwner = enrollments.ToDictionary(e => e.Id, e => e.StudentId);
        var enrollmentIds = enrollmentOwner.Keys.ToList();
        var attendance = await _context.Attendances
            .Where(a => enrollmentIds.Contains(a.EnrollmentId))
            .Select(a => new { a.EnrollmentId, a.Status })
            .ToListAsync();

        var points = new List<StudentKpiPoint>();

        foreach (var s in students)
        {
            var mine = enrollments.Where(e => e.StudentId == s.Id).ToList();
            var marks = mine.Where(e => e.Mark.HasValue).Select(e => e.Mark!.Value).ToList();

            // Skip students with nothing graded (or not in the chosen subject)
            if (marks.Count == 0) continue;

            var records = attendance.Where(a => enrollmentOwner[a.EnrollmentId] == s.Id).ToList();

            double rate = records.Count > 0
                ? records.Count(r => r.Status == AttendanceStatus.Present) * 100.0 / records.Count
                : Math.Max(0, 100 - mine.Average(e => e.Absences) * 100.0 / SchoolDays);

            points.Add(new StudentKpiPoint
            {
                StudentName = s.FullName,
                GradeLevel = s.GradeLevel ?? "N/A",
                Section = s.Section ?? "N/A",
                Marks = Math.Round(marks.Average(), 1),
                AttendanceRate = Math.Round(rate, 1)
            });
        }

        return Ok(points);
    }
}

public class StudentKpiPoint
{
    public string StudentName { get; set; } = string.Empty; // X-Axis
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public double Marks { get; set; }          // Blue Line
    public double AttendanceRate { get; set; } // Green Line
}