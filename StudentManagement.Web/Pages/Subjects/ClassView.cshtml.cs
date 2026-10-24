using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Subjects;

public class ClassViewModel : PageModel
{
    private readonly SubjectApiClient _subjectApi;
    private readonly TeacherApiClient _teacherApi;
    private readonly StudentApiClient _studentApi;

    public ClassViewModel(
        SubjectApiClient subjectApi,
        TeacherApiClient teacherApi,
        StudentApiClient studentApi)
    {
        _subjectApi = subjectApi;
        _teacherApi = teacherApi;
        _studentApi = studentApi;
    }

    public Subject Subject { get; set; } = default!;
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public Teacher? Teacher { get; set; }

    [BindProperty]
    public List<StudentRow> Students { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int subjectId, string gradeLevel, string section)
    {
        GradeLevel = gradeLevel;
        Section = section;

        var subject = await _subjectApi.GetByIdAsync(subjectId);
        if (subject == null) return NotFound();
        Subject = subject;

        // 1. Load assigned teacher
        var teachers = await _teacherApi.GetAllAsync();
        if (teachers != null)
        {
            Teacher = teachers.FirstOrDefault(t =>
                t.ClassAssignments != null &&
                t.ClassAssignments.Any(ca => ca.SubjectId == subjectId && ca.GradeLevel == gradeLevel && ca.Section == section));
        }

        // 2. Fetch roster to load saved Marks and Absences
        var roster = await _subjectApi.GetRosterAsync(subjectId);

        // 3. Populate student rows for this grade level & section
        var allStudents = await _studentApi.GetAllAsync();
        if (allStudents != null)
        {
            Students = allStudents
                .Where(s => s.GradeLevel == gradeLevel && s.Section == section)
                .Select(s =>
                {
                    var enrollment = roster.FirstOrDefault(r => r.StudentId == s.Id);
                    return new StudentRow
                    {
                        StudentId = s.Id,
                        FullName = s.FullName,
                        Absences = enrollment?.Absences ?? 0,
                        Mark = enrollment?.Mark ?? 100
                    };
                })
                .ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSaveRecordsAsync(int subjectId, string gradeLevel, string section)
    {
        await _subjectApi.SaveClassRecordsAsync(subjectId, Students);
        return RedirectToPage(new { subjectId, gradeLevel, section });
    }

    public class StudentRow
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Absences { get; set; }
        public int? Mark { get; set; }
    }
}