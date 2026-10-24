using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Teacherss;

public class DetailsModel : PageModel
{
    private readonly TeacherApiClient _api;

    public DetailsModel(TeacherApiClient api)
    {
        _api = api;
    }

    public Teacher? Teacher { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Teacher = await _api.GetByIdAsync(id);

        if (Teacher == null)
        {
            return NotFound();
        }

        return Page();
    }
}