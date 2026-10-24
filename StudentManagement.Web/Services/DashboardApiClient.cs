using StudentManagement.Web.models;

namespace StudentManagement.Web.Services;

public class DashboardApiClient
{
    private readonly HttpClient _http;

    public DashboardApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<StudentKpiPoint>> GetPerformanceAsync(string? grade, string? section, string? subject)
    {
        var query = $"api/Dashboard/school-performance?grade={Uri.EscapeDataString(grade ?? "")}&section={Uri.EscapeDataString(section ?? "")}&subject={Uri.EscapeDataString(subject ?? "")}";
        return await _http.GetFromJsonAsync<List<StudentKpiPoint>>(query) ?? new List<StudentKpiPoint>();
    }
}