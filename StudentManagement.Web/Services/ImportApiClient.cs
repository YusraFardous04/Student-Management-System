namespace StudentManagement.Web.Services;

public class ImportApiClient
{
    private readonly HttpClient _http;
    public ImportApiClient(HttpClient http) => _http = http;

    public async Task<string> ImportTeachersAsync(IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        content.Add(new StreamContent(stream), "file", file.FileName);
        var response = await _http.PostAsync("api/import/teachers", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ImportStudentsAsync(IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        content.Add(new StreamContent(stream), "file", file.FileName);
        var response = await _http.PostAsync("api/import/students", content);
        return await response.Content.ReadAsStringAsync();
    }
}