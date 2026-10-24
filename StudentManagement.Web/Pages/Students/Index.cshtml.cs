using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Students;

public class IndexModel : PageModel
{
    private readonly StudentApiClient _studentApi;

    public IndexModel(StudentApiClient studentApi)
    {
        _studentApi = studentApi;
    }

    public List<Student> Students { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _studentApi.GetAllAsync();
        if (result != null)
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Students = result.Where(s =>
                    (s.FullName != null && s.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Email != null && s.Email.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                    (s.GradeLevel != null && s.GradeLevel.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Section != null && s.Section.Contains(Search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            else
            {
                Students = result;
            }
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _studentApi.DeleteAsync(id);
        return RedirectToPage();
    }
}