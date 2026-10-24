using System.Text.RegularExpressions;

namespace StudentManagement.Api.Services;

public interface IChatService
{
    Task<string> AskAsync(string sessionId, string question);
}

public class ChatService : IChatService
{
    private readonly IRetrievalService _retrieval;
    private readonly IAnalyticsRetrievalService _analytics;
    private readonly OllamaClient _ollama;
    private readonly ChatMemoryStore _memory;

    // Words that mean "the person we were just talking about"
    private static readonly Regex FollowUpPronouns = new(
        @"\b(he|she|him|his|her|hers|they|them|their|theirs|that student|this student)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public ChatService(
        IRetrievalService retrieval,
        IAnalyticsRetrievalService analytics,
        OllamaClient ollama,
        ChatMemoryStore memory)
    {
        _retrieval = retrieval;
        _analytics = analytics;
        _ollama = ollama;
        _memory = memory;
    }

    public async Task<string> AskAsync(string sessionId, string question)
    {
        var parts = new List<string>();
        var history = _memory.GetHistory(sessionId);

        // Search with the question on its own. Only when it is a follow-up ("what about her marks?")
        // do we add the recent earlier questions, so old names can't hijack a new, unrelated question.
        string retrievalText = question;
        if (FollowUpPronouns.IsMatch(question))
        {
            var recentUserQuestions = history
                .Where(t => t.Role == "user")
                .TakeLast(3)
                .Select(t => t.Text);
            retrievalText = string.Join(" ", recentUserQuestions) + " " + question;
        }

        var profiles = await _retrieval.FindRelevantChunksAsync(retrievalText);
        if (profiles.Count > 0)
            parts.Add("Student and teacher profiles:\n" + string.Join("\n", profiles));

        if (_analytics.IsAnalyticalQuestion(question))
            parts.Add("Calculated statistics:\n" + _analytics.BuildAnalyticsSummary(question));

        var context = parts.Count > 0
            ? string.Join("\n\n", parts)
            : "(no matching information was found)";

        var systemPrompt =
            "You are an assistant for a school's student management system. " +
            "Answer the user's question using ONLY the context below. " +
            "Quote numbers exactly as given and never invent marks, names or attendance. " +
            "If the context does not contain the answer, say you don't have that information " +
            "and suggest including the student's full name or the grade and section.\n\n" +
            "Context:\n" + context;

        // Call Ollama with the full conversation history.
        var answer = await _ollama.GetChatCompletionAsync(systemPrompt, history, question);

        // Save both turns for next time.
        _memory.AddTurn(sessionId, "user", question);
        _memory.AddTurn(sessionId, "assistant", answer);

        return answer;
    }
}