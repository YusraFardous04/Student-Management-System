using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Subjects;

public class AttendanceModel : PageModel
{
    private readonly AttendanceApiClient _api;
    public AttendanceModel(AttendanceApiClient api) => _api = api;

    public List<Attendance> Records { get; set; } = new();

    [BindProperty]
    public DateTime SelectedDate { get; set; } = DateTime.Today;

    [BindProperty]
    public AttendanceStatus SelectedStatus { get; set; }

    public async Task OnGetAsync(int enrollmentId)
        => Records = await _api.GetByEnrollmentAsync(enrollmentId);

    public async Task<IActionResult> OnPostMarkAsync(int enrollmentId)
    {
        await _api.MarkAsync(enrollmentId, SelectedDate, SelectedStatus);
        return RedirectToPage(new { enrollmentId });
    }
}