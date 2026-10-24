using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages;

public class IndexModel : PageModel
{
    private readonly DashboardApiClient _dashboardClient;

    public IndexModel(DashboardApiClient dashboardClient)
    {
        _dashboardClient = dashboardClient;
    }

    public List<StudentKpiPoint> KpiData { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Grade { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Section { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Subject { get; set; }

    public async Task OnGetAsync()
    {
        KpiData = await _dashboardClient.GetPerformanceAsync(Grade, Section, Subject);
    }
}