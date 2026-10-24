using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Subjects;

public class IndexModel : PageModel
{
    private readonly SubjectApiClient _api;
    public IndexModel(SubjectApiClient api) => _api = api;

    public List<Subject> Subjects { get; set; } = new();
    public async Task OnGetAsync() => Subjects = await _api.GetAllAsync();
}