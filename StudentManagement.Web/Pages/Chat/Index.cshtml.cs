using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Chat;

public class IndexModel : PageModel
{
    private readonly ChatApiClient _api;
    public IndexModel(ChatApiClient api) => _api = api;

    private const string Greeting =
        "Hi! Ask me about a student (use their full name) or about a class. " +
        "You can follow up naturally, like \u201cwhat about his attendance?\u201d";

    [BindProperty]
    public string Question { get; set; } = string.Empty;

    [BindProperty]
    public string SessionId { get; set; } = string.Empty;

    public List<ChatTurn> Conversation { get; set; } = new();

    public IActionResult OnGet()
    {
        SessionId = Guid.NewGuid().ToString();
        Conversation.Add(new ChatTurn { Role = "bot", Text = Greeting });
        return Page();
    }

    public async Task<IActionResult> OnPostAskAsync()
    {
        Conversation = ChatTurn.FromHiddenFields(Request.Form["History"]);

        if (string.IsNullOrWhiteSpace(SessionId))
            SessionId = Guid.NewGuid().ToString();

        if (!string.IsNullOrWhiteSpace(Question))
        {
            Conversation.Add(new ChatTurn { Role = "user", Text = Question });

            var result = await _api.AskAsync(SessionId, Question);

            // The API may have started a new session if ours was empty.
            if (!string.IsNullOrWhiteSpace(result.SessionId))
                SessionId = result.SessionId;

            Conversation.Add(new ChatTurn
            {
                Role = "bot",
                Text = string.IsNullOrWhiteSpace(result.Answer) ? "No answer received." : result.Answer
            });
        }

        // Without this, the form re-displays the OLD posted values (empty session id,
        // old question) instead of the new ones set here.
        ModelState.Clear();
        Question = string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostReindexAsync()
    {
        Conversation = ChatTurn.FromHiddenFields(Request.Form["History"]);
        if (Conversation.Count == 0)
            Conversation.Add(new ChatTurn { Role = "bot", Text = Greeting });

        if (string.IsNullOrWhiteSpace(SessionId))
            SessionId = Guid.NewGuid().ToString();

        var ok = await _api.ReindexAsync();
        Conversation.Add(new ChatTurn
        {
            Role = "bot",
            Text = ok ? "Knowledge base rebuilt." : "Rebuilding failed."
        });

        ModelState.Clear();
        Question = string.Empty;
        return Page();
    }
}

public class ChatTurn
{
    public string Role { get; set; } = "bot";
    public string Text { get; set; } = string.Empty;

    public static string ToHiddenField(IEnumerable<ChatTurn> turns) =>
        string.Join(";", turns.Select(t =>
            $"{t.Role}|{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(t.Text))}"));

    public static List<ChatTurn> FromHiddenFields(string? raw)
    {
        var list = new List<ChatTurn>();
        if (string.IsNullOrWhiteSpace(raw)) return list;
        foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = part.IndexOf('|');
            if (idx < 0) continue;
            try
            {
                list.Add(new ChatTurn
                {
                    Role = part.Substring(0, idx),
                    Text = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(part.Substring(idx + 1)))
                });
            }
            catch { }
        }
        return list;
    }
}