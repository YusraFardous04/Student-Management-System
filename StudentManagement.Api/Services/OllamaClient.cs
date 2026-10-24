using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentManagement.Api.Services;

public class OllamaClient
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;
    private readonly string _chatModel;
    private readonly string _embedModel;

    public OllamaClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Ollama:ApiKey"];
        _chatModel = config["Ollama:ChatModel"] ?? "gpt-oss:20b";
        _embedModel = config["Ollama:EmbedModel"] ?? "nomic-embed-text";

        _http.BaseAddress = new Uri(config["Ollama:BaseUrl"] ?? "https://ollama.com/");
        _http.Timeout = TimeSpan.FromMinutes(2);

        if (!string.IsNullOrWhiteSpace(_apiKey))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GetChatCompletionAsync(string systemPrompt, string userPrompt)
    {
        EnsureKey();

        var body = new
        {
            model = _chatModel,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            stream = false,
            options = new { temperature = 0.2 }
        };

        var response = await _http.PostAsJsonAsync("api/chat", body);
        using var doc = await ReadOrThrowAsync(response);
        return doc.RootElement.GetProperty("message").GetProperty("content").GetString() ?? "";
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        EnsureKey();

        var response = await _http.PostAsJsonAsync("api/embed", new { model = _embedModel, input = text });
        using var doc = await ReadOrThrowAsync(response);
        return doc.RootElement.GetProperty("embeddings")[0]
            .EnumerateArray().Select(v => v.GetSingle()).ToArray();
    }

    private void EnsureKey()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException(
                "Ollama:ApiKey is missing. Run: dotnet user-secrets set \"Ollama:ApiKey\" \"<your key>\"");
    }

    // Turns a failed HTTP call into a readable error that includes what Ollama actually said.
    private static async Task<JsonDocument> ReadOrThrowAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Ollama returned {(int)response.StatusCode}: {body}");
        return JsonDocument.Parse(body);
    }
    public async Task<string> GetChatCompletionAsync(
    string systemPrompt,
    IEnumerable<ChatTurn> history,
    string newUserMessage)
    {
        EnsureKey();

        var messages = new List<object>
    {
        new { role = "system", content = systemPrompt }
    };

        foreach (var turn in history)
        {
            messages.Add(new
            {
                role = turn.Role == "assistant" ? "assistant" : "user",
                content = turn.Text
            });
        }

        messages.Add(new { role = "user", content = newUserMessage });

        var body = new
        {
            model = _chatModel,
            messages,
            stream = false,
            options = new { temperature = 0.2 }
        };

        var response = await _http.PostAsJsonAsync("api/chat", body);
        using var doc = await ReadOrThrowAsync(response);
        return doc.RootElement.GetProperty("message").GetProperty("content").GetString() ?? "";
    }
}