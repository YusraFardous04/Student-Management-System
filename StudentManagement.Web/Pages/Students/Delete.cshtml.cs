using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly StudentApiClient _api;
    public DeleteModel(StudentApiClient api) => _api = api;

    [BindProperty]
    public Student Student { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _api.GetByIdAsync(id);
        if (student is null) return NotFound();
        Student = student;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _api.DeleteAsync(Student.Id);
        return RedirectToPage("Index");
    }
}