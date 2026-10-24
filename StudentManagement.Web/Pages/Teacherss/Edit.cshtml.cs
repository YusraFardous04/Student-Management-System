using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Teacherss;

public class EditModel : PageModel
{
    private readonly TeacherApiClient _api;

    public EditModel(TeacherApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public Teacher Teacher { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var teacher = await _api.GetByIdAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        Teacher = teacher;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _api.UpdateAsync(Teacher.Id, Teacher);
        return RedirectToPage("./Index");
    }
}