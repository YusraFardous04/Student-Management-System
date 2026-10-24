using System.Net.Http.Json;
using StudentManagement.Web.models;

namespace StudentManagement.Web.Services
{
    public class EnrollmentApiClient
    {

        private readonly HttpClient _http;
        public EnrollmentApiClient(HttpClient http) => _http = http;

        public async Task<List<Enrollment>> GetByStudentAsync(int studentId)
            => await _http.GetFromJsonAsync<List<Enrollment>>($"api/enrollments/student/{studentId}") ?? new();

        public async Task<(bool Success, string? Error)> EnrollAsync(int studentId, int subjectId)
        {
            var response = await _http.PostAsync($"api/enrollments?studentId={studentId}&subjectId={subjectId}", null);
            if (response.IsSuccessStatusCode) return (true, null);
            var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return (false, body?.GetValueOrDefault("error") ?? "Enrollment failed.");
        }

        public async Task UnenrollAsync(int enrollmentId)
            => await _http.DeleteAsync($"api/enrollments/{enrollmentId}");
    }
}
