using System.Net.Http.Json;
using StudentManagement.Web.models;

namespace StudentManagement.Web.Services;

public class ChatApiClient
{
    private readonly HttpClient _http;
    public ChatApiClient(HttpClient http) => _http = http;

    public async Task<AskResponse> AskAsync(string sessionId, string question)
    {
        var response = await _http.PostAsJsonAsync("api/chat/ask",
            new AskRequest { SessionId = sessionId, Question = question });

        if (!response.IsSuccessStatusCode)
            return new AskResponse { Answer = "Something went wrong. Please try again." };

        return await response.Content.ReadFromJsonAsync<AskResponse>()
               ?? new AskResponse { Answer = "No answer received." };
    }
    public async Task<bool> ReindexAsync()
    {
        var response = await _http.PostAsync("api/chat/reindex", null);
        return response.IsSuccessStatusCode;
    }
}