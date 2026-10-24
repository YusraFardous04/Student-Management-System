using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Teacherss;

public class CreateModel : PageModel
{
    private readonly TeacherApiClient _teacherApi;
    private readonly SubjectApiClient _subjectApi;

    public CreateModel(TeacherApiClient teacherApi, SubjectApiClient subjectApi)
    {
        _teacherApi = teacherApi;
        _subjectApi = subjectApi;
    }

    [BindProperty]
    public Teacher Teacher { get; set; } = new();

    [BindProperty]
    public List<AssignmentInput> Assignments { get; set; } = new();

    public SelectList SubjectList { get; set; } = default!;

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSubjectsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSubjectsAsync();
            return Page();
        }

        // Map valid UI assignments into ClassAssignments
        if (Assignments != null && Assignments.Any())
        {
            foreach (var item in Assignments)
            {
                if (item.SubjectId > 0
                    && !string.IsNullOrWhiteSpace(item.GradeLevel)
                    && !string.IsNullOrWhiteSpace(item.Section))
                {
                    Teacher.ClassAssignments.Add(new ClassAssignment
                    {
                        SubjectId = item.SubjectId,   // ← THE MISSING LINE
                        GradeLevel = item.GradeLevel,
                        Section = item.Section
                    });
                }
            }
        }

        // Also keep GradeLevel string consistent for the [NotMapped] getter
        if (Teacher.ClassAssignments.Any())
        {
            Teacher.GradeLevel = string.Join(", ",
                Teacher.ClassAssignments.Select(a => $"{a.GradeLevel}-{a.Section}"));
        }

        var (success, error) = await _teacherApi.CreateAsync(Teacher);

        if (!success)
        {
            ErrorMessage = $"Could not save teacher: {error}";
            await LoadSubjectsAsync();
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task LoadSubjectsAsync()
    {
        var subjects = await _subjectApi.GetAllAsync();
        SubjectList = new SelectList(subjects, "Id", "Name");
    }

    public class AssignmentInput
    {
        public int SubjectId { get; set; }
        public string GradeLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
    }
}