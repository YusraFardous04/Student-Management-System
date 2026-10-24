using System.Net.Http.Json;

using StudentManagement.Web.models;

namespace StudentManagement.Web.Services;

public class ClassAssignmentApiClient
{
    private readonly HttpClient _http;
    public ClassAssignmentApiClient(HttpClient http) => _http = http;

    public async Task<List<ClassAssignment>> GetByTeacherAsync(int teacherId)
        => await _http.GetFromJsonAsync<List<ClassAssignment>>($"api/classassignments/teacher/{teacherId}") ?? new();

    public async Task CreateAsync(int teacherId, int subjectId, string gradeLevel, string section)
        => await _http.PostAsync(
            $"api/classassignments?teacherId={teacherId}&subjectId={subjectId}" +
            $"&gradeLevel={Uri.EscapeDataString(gradeLevel)}&section={section}", null);

    public async Task DeleteAsync(int id)
        => await _http.DeleteAsync($"api/classassignments/{id}");
}