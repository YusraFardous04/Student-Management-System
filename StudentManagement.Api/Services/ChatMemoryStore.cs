using System.Collections.Concurrent;

namespace StudentManagement.Api.Services;

public class ChatMemoryStore
{
    private class Session
    {
        public List<ChatTurn> Turns { get; } = new();
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private const int MaxTurns = 12; // ~6 user + 6 bot messages

    public List<ChatTurn> GetHistory(string sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
            return new List<ChatTurn>();

        session.LastActivity = DateTime.UtcNow;
        return session.Turns.ToList();
    }

    public void AddTurn(string sessionId, string role, string text)
    {
        var session = _sessions.GetOrAdd(sessionId, _ => new Session());
        session.Turns.Add(new ChatTurn { Role = role, Text = text });
        session.LastActivity = DateTime.UtcNow;

        // Keep only the most recent turns so the prompt doesn't grow forever.
        if (session.Turns.Count > MaxTurns)
            session.Turns.RemoveRange(0, session.Turns.Count - MaxTurns);
    }

    public void RemoveIdleSessions(TimeSpan maxIdle)
    {
        var cutoff = DateTime.UtcNow - maxIdle;
        foreach (var kvp in _sessions)
            if (kvp.Value.LastActivity < cutoff)
                _sessions.TryRemove(kvp.Key, out _);
    }
}

public class ChatTurn
{
    public string Role { get; set; } = "user"; // "user" or "assistant"
    public string Text { get; set; } = string.Empty;
}