using StudentManagement.Web.models;

namespace StudentManagement.Web.Services
{
    public class AttendanceApiClient
    {
        private readonly HttpClient _http;
        public AttendanceApiClient(HttpClient http) => _http = http;

        public async Task<List<Attendance>> GetByEnrollmentAsync(int enrollmentId)
            => await _http.GetFromJsonAsync<List<Attendance>>($"api/attendance/enrollment/{enrollmentId}") ?? new();

        public async Task MarkAsync(int enrollmentId, DateTime date, AttendanceStatus status)
            => await _http.PostAsync(
                $"api/attendance?enrollmentId={enrollmentId}&date={date:yyyy-MM-dd}&status={status}", null);
    }
}
