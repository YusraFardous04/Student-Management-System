using StudentManagement.Web.models;
using StudentManagement.Web.Pages.Subjects;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentManagement.Web.Services
{
    public class SubjectApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SubjectApiClient(HttpClient http) => _http = http;

        public async Task<List<Subject>> GetAllAsync()
            => await _http.GetFromJsonAsync<List<Subject>>("api/Subjects", _options) ?? new();

        public async Task<Subject?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Subjects/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Subject>(_options);
        }

        public async Task<List<Enrollment>> GetRosterAsync(int subjectId)
            => await _http.GetFromJsonAsync<List<Enrollment>>($"api/subjects/{subjectId}/roster") ?? new();

        public async Task<bool> SaveClassRecordsAsync(int subjectId, List<ClassViewModel.StudentRow> students)
        {
            var response = await _http.PostAsJsonAsync($"api/Subjects/{subjectId}/records", students);
            return response.IsSuccessStatusCode;
        }
    }
}