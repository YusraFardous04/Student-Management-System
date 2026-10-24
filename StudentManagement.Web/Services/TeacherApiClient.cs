using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using StudentManagement.Web.models;

namespace StudentManagement.Web.Services;

public class TeacherApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public TeacherApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Teacher>> GetAllAsync(string? searchTerm = null)
    {
        string url = string.IsNullOrWhiteSpace(searchTerm)
            ? "api/Teachers"
            : $"api/Teachers?searchTerm={Uri.EscapeDataString(searchTerm)}";

        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return new List<Teacher>();

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content)) return new List<Teacher>();

        return JsonSerializer.Deserialize<List<Teacher>>(content, _jsonOptions) ?? new List<Teacher>();
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Teachers/{id}");
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content)) return null;

        return JsonSerializer.Deserialize<Teacher>(content, _jsonOptions);
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Teacher teacher)
    {
        var response = await _http.PostAsJsonAsync("api/Teachers", teacher, _jsonOptions);
        if (response.IsSuccessStatusCode) return (true, string.Empty);

        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrEmpty(error) ? response.ReasonPhrase ?? "API Error" : error);
    }

    public async Task<(bool Success, string ErrorMessage)> UpdateAsync(int id, Teacher teacher)
    {
        var response = await _http.PutAsJsonAsync($"api/Teachers/{id}", teacher, _jsonOptions);
        if (response.IsSuccessStatusCode) return (true, string.Empty);

        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrEmpty(error) ? response.ReasonPhrase ?? "API Error" : error);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Teachers/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveAssignmentAsync(int teacherId, int assignmentId)
    {
        var response = await _http.DeleteAsync($"api/Teachers/{teacherId}/assignments/{assignmentId}");
        return response.IsSuccessStatusCode;
    }
}