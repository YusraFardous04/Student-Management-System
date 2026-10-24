using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Subjects;

public class DetailsModel : PageModel
{
    private readonly SubjectApiClient _subjectApi;
    private readonly TeacherApiClient _teacherApi;

    public DetailsModel(SubjectApiClient subjectApi, TeacherApiClient teacherApi)
    {
        _subjectApi = subjectApi;
        _teacherApi = teacherApi;
    }

    public Subject Subject { get; set; } = default!;

    public List<string> AllGrades { get; set; } = new()
    {
        "Kindergarten", "1st Grade", "2nd Grade", "3rd Grade", "4th Grade",
        "5th Grade", "6th Grade", "7th Grade", "8th Grade", "9th Grade",
        "10th Grade", "11th Grade", "12th Grade"
    };

    public List<string> AllSections { get; set; } = new() { "A", "B", "C", "D" };

    public HashSet<string> AssignedClassKeys { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var subject = await _subjectApi.GetByIdAsync(id);
        if (subject == null) return NotFound();
        Subject = subject;

        var teachers = await _teacherApi.GetAllAsync();
        if (teachers != null)
        {
            var assignments = teachers
                .Where(t => t.ClassAssignments != null)
                .SelectMany(t => t.ClassAssignments)
                .Where(ca => ca != null && ca.SubjectId == id)
                .ToList();

            foreach (var ca in assignments)
            {
                if (!string.IsNullOrEmpty(ca.GradeLevel) && !string.IsNullOrEmpty(ca.Section))
                {
                    AssignedClassKeys.Add($"{ca.GradeLevel}-{ca.Section}");
                }
            }
        }

        return Page();
    }
}