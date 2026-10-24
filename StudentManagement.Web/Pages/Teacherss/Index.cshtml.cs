using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Teacherss;

public class IndexModel : PageModel
{
    private readonly TeacherApiClient _api;

    public IndexModel(TeacherApiClient api)
    {
        _api = api;
    }

    public List<Teacher> Teachers { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var all = await _api.GetAllAsync(Search);
        var teacherList = all ?? new List<Teacher>();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            // FIX: Safely check t.Subject.Name instead of calling .Contains on t.Subject directly
            Teachers = teacherList.Where(t =>
                (!string.IsNullOrEmpty(t.FullName) && t.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(t.Email) && t.Email.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (t.Subject != null && !string.IsNullOrEmpty(t.Subject.Name) && t.Subject.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        else
        {
            Teachers = teacherList;
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToPage();
    }
}