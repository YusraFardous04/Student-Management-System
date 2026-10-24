using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services;

public interface IIndexingService
{
    Task RebuildIndexAsync();
}

public class IndexingService : IIndexingService
{
    private readonly StudentDbContext _context;
    private readonly OllamaClient _ollama;
    private readonly bool _useEmbeddings;

    public IndexingService(StudentDbContext context, OllamaClient ollama, IConfiguration config)
    {
        _context = context;
        _ollama = ollama;
        _useEmbeddings = config.GetValue<bool>("Rag:UseEmbeddings");
    }

    public async Task RebuildIndexAsync()
    {
        _context.KnowledgeChunks.RemoveRange(_context.KnowledgeChunks);
        await _context.SaveChangesAsync();

        // One chunk per student
        foreach (var student in _context.Students.ToList())
        {
            var enrollments = _context.Enrollments
                .Where(e => e.StudentId == student.Id)
                .Select(e => new { SubjectName = e.Subject!.Name, e.Mark, e.Absences })
                .ToList();

            var statuses = _context.Attendances
                .Where(a => a.Enrollment!.StudentId == student.Id)
                .Select(a => a.Status)
                .ToList();

            var marks = enrollments.Where(e => e.Mark.HasValue).Select(e => e.Mark!.Value).ToList();

            var subjectsText = enrollments.Count == 0
                ? "not enrolled in any subjects"
                : string.Join("; ", enrollments.Select(e =>
                    $"{e.SubjectName}: mark {(e.Mark.HasValue ? e.Mark.ToString() : "not graded")}, {e.Absences} absences"));

            var averageText = marks.Count > 0 ? $"{marks.Average():F1}" : "not available";
            var totalAbsences = enrollments.Sum(e => e.Absences);

            var present = statuses.Count(s => s == AttendanceStatus.Present);
            var attendanceText = statuses.Count == 0
                ? ""
                : $" Daily attendance: {present} of {statuses.Count} recorded days present ({present * 100.0 / statuses.Count:F0}%).";

            var content =
                $"Student {student.FullName} is in {student.GradeLevel}, Section {student.Section}. " +
                $"Subjects: {subjectsText}. Average mark: {averageText}. Total absences: {totalAbsences}." +
                attendanceText;

            await AddChunkAsync("Student", student.Id, student.FullName, content);
        }

        // One chunk per teacher
        foreach (var teacher in _context.Teachers.Include(t => t.Subject).ToList())
        {
            var assignments = _context.ClassAssignments
                .Where(ca => ca.TeacherId == teacher.Id)
                .Select(ca => new { ca.GradeLevel, ca.Section })
                .ToList();

            var subjectName = teacher.Subject?.Name ?? "no subject";

            var classesText = assignments.Count == 0
                ? "no classes currently assigned"
                : string.Join("; ", assignments
                    .GroupBy(a => a.GradeLevel)
                    .Select(g => $"{g.Key} Sections {string.Join(", ", g.Select(a => a.Section).OrderBy(s => s))}"));

            await AddChunkAsync("Teacher", teacher.Id, teacher.FullName,
                $"Teacher {teacher.FullName} teaches {subjectName}. Classes: {classesText}.");
        }

        await _context.SaveChangesAsync();
    }

    private async Task AddChunkAsync(string type, int id, string title, string content)
    {
        var embeddingJson = string.Empty;
        if (_useEmbeddings)
        {
            var vector = await _ollama.GetEmbeddingAsync(content);
            embeddingJson = JsonSerializer.Serialize(vector);
        }

        _context.KnowledgeChunks.Add(new KnowledgeChunk
        {
            SourceType = type,
            SourceId = id,
            Title = title,
            Content = content,
            EmbeddingJson = embeddingJson
        });
    }
}