using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;
using System.Linq;

namespace StudentManagement.Web.Pages.Students;

public class EditModel : PageModel
{
    private readonly StudentApiClient _api;
    public EditModel(StudentApiClient api) => _api = api;

    [BindProperty]
    public Student Student { get; set; } = new();
    public List<SelectListItem> GradeOptions => GradeLevelOptions;
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _api.GetByIdAsync(id);
        if (student is null) return NotFound();
        Student = student;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.UpdateAsync(Student.Id, Student);
        return RedirectToPage("Index");
    }

    public List<SelectListItem> GradeLevelOptions { get; } = new()
{
    new("Kindergarten", "Kindergarten"),
    new("1st Grade", "1st Grade"),
    new("2nd Grade", "2nd Grade"),
    new("3rd Grade", "3rd Grade"),
    new("4th Grade", "4th Grade"),
    new("5th Grade", "5th Grade"),
    new("6th Grade", "6th Grade"),
    new("7th Grade", "7th Grade"),
    new("8th Grade", "8th Grade"),
    new("9th Grade", "9th Grade"),
    new("10th Grade", "10th Grade"),
    new("11th Grade", "11th Grade"),
    new("12th Grade", "12th Grade"),
};
}