using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Students;

public class CreateModel : PageModel
{
    private readonly StudentApiClient _studentApi;
    private readonly SubjectApiClient _subjectApi;

    public CreateModel(StudentApiClient studentApi, SubjectApiClient subjectApi)
    {
        _studentApi = studentApi;
        _subjectApi = subjectApi;
    }

    [BindProperty]
    public Student Student { get; set; } = new();

    public List<Subject> AvailableSubjects { get; set; } = new();

    public List<string> GradeOptions { get; set; } = new()
    {
        "Kindergarten", "1st Grade", "2nd Grade", "3rd Grade", "4th Grade",
        "5th Grade", "6th Grade", "7th Grade", "8th Grade", "9th Grade",
        "10th Grade", "11th Grade", "12th Grade"
    };

    public List<string> SectionOptions { get; set; } = new() { "A", "B", "C", "D" };

    public async Task OnGetAsync()
    {
        await LoadSubjectsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSubjectsAsync();
            return Page();
        }

        var (success, errorMessage) = await _studentApi.CreateAsync(Student);
        if (success)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, $"Unable to save student: {errorMessage}");
        await LoadSubjectsAsync();
        return Page();
    }

    private async Task LoadSubjectsAsync()
    {
        var subjects = await _subjectApi.GetAllAsync();
        AvailableSubjects = subjects ?? new List<Subject>();
    }
}