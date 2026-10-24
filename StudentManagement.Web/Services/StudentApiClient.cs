using StudentManagement.Web.models;

namespace StudentManagement.Web.Services;

public class StudentApiClient
{
    private readonly HttpClient _http;

    public StudentApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Student>> GetAllAsync(string? searchTerm = null)
    {
        string url = string.IsNullOrWhiteSpace(searchTerm)
            ? "api/Students"
            : $"api/Students?searchTerm={Uri.EscapeDataString(searchTerm)}";

        return await _http.GetFromJsonAsync<List<Student>>(url) ?? new List<Student>();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<Student>($"api/Students/{id}");
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Student student)
    {
        var response = await _http.PostAsJsonAsync("api/Students", student);
        if (response.IsSuccessStatusCode) return (true, string.Empty);

        var errorContent = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrEmpty(errorContent) ? response.ReasonPhrase ?? "API Error" : errorContent);
    }

    public async Task<(bool Success, string ErrorMessage)> UpdateAsync(int id, Student student)
    {
        var response = await _http.PutAsJsonAsync($"api/Students/{id}", student);
        if (response.IsSuccessStatusCode) return (true, string.Empty);

        var errorContent = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrEmpty(errorContent) ? response.ReasonPhrase ?? "API Error" : errorContent);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Students/{id}");
        return response.IsSuccessStatusCode;
    }
}