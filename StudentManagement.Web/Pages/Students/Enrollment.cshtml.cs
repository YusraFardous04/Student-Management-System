using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;
using System.Linq;

namespace StudentManagement.Web.Pages.Students;

public class SubjectCheckboxItem
{
    public int SubjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class EnrollmentModel : PageModel
{
    private readonly StudentApiClient _studentApi;
    private readonly EnrollmentApiClient _enrollmentApi;
    private readonly SubjectApiClient _subjectApi;

    public EnrollmentModel(StudentApiClient studentApi, EnrollmentApiClient enrollmentApi, SubjectApiClient subjectApi)
    {
        _studentApi = studentApi;
        _enrollmentApi = enrollmentApi;
        _subjectApi = subjectApi;
    }

    public Student? Student { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new();
    public List<string> Errors { get; set; } = new();

    [BindProperty]
    public List<SubjectCheckboxItem> SubjectOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var loaded = await LoadAsync(id);
        return loaded ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostEnrollAsync(int id)
    {
        foreach (var option in SubjectOptions.Where(o => o.IsSelected))
        {
            var (success, error) = await _enrollmentApi.EnrollAsync(id, option.SubjectId);
            if (!success && error is not null)
                Errors.Add(error);
        }

        if (Errors.Count > 0)
        {
            await LoadAsync(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnenrollAsync(int id, int enrollmentId)
    {
        await _enrollmentApi.UnenrollAsync(enrollmentId);
        return RedirectToPage(new { id });
    }

    private async Task<bool> LoadAsync(int id)
    {
        Student = await _studentApi.GetByIdAsync(id);
        if (Student is null) return false;

        Enrollments = await _enrollmentApi.GetByStudentAsync(id);
        var alreadyEnrolledIds = Enrollments.Select(e => e.SubjectId).ToHashSet();

        var allSubjects = await _subjectApi.GetAllAsync();
        SubjectOptions = allSubjects
            .Where(s => !alreadyEnrolledIds.Contains(s.Id))
            .Select(s => new SubjectCheckboxItem { SubjectId = s.Id, Name = s.Name })
            .ToList();

        return true;
    }
}