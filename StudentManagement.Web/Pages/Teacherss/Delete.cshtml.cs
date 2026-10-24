using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Teacherss;
public class DeleteModel : PageModel
{
    public string? ErrorMessage { get; set; }
    private readonly TeacherApiClient _api;
    public DeleteModel(TeacherApiClient api) => _api = api;

    [BindProperty]
    public Teacher Teacher { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var teacher = await _api.GetByIdAsync(id);
        if (teacher is null) return NotFound();
        Teacher = teacher;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var success = await _api.DeleteAsync(Teacher.Id);
        if (!success)
        {
            ErrorMessage = "Could not delete this teacher — they may still be assigned to a subject.";
            return Page();
        }
        return RedirectToPage("Index");
    }
}