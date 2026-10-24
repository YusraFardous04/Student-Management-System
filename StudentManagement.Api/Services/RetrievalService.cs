using System.Text.Json;
using System.Text.RegularExpressions;
using StudentManagement.Api.Data;

namespace StudentManagement.Api.Services;

public interface IRetrievalService
{
    Task<List<string>> FindRelevantChunksAsync(string question, int topN = 3);
}

public class RetrievalService : IRetrievalService
{
    private readonly StudentDbContext _context;
    private readonly OllamaClient _ollama;
    private readonly bool _useEmbeddings;

    // Very common words that say nothing about WHICH person is meant.
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "how", "what", "who", "whom", "is", "are", "was", "the", "and", "about", "tell", "show",
        "does", "doing", "did", "for", "with", "has", "have", "his", "her", "their", "from",
        "that", "this", "student", "teacher", "can", "you", "me", "please", "give", "to"
    };

    // "teach", "teaches", "teachs", "techer", "teacher" ...
    private static readonly Regex TeachWord = new(@"\bt[ea]{1,2}ch", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public RetrievalService(StudentDbContext context, OllamaClient ollama, IConfiguration config)
    {
        _context = context;
        _ollama = ollama;
        _useEmbeddings = config.GetValue<bool>("Rag:UseEmbeddings");
    }

    public async Task<List<string>> FindRelevantChunksAsync(string question, int topN = 3)
    {
        // Class roster questions: "students in grade 9", "list grade 10 section B"
        var roster = FindClassRoster(question);
        if (roster != null) return new List<string> { roster };

        return _useEmbeddings
            ? await FindByVectorAsync(question, topN)
            : FindByKeyword(question, topN);
    }

    // ---------- class roster (reads straight from the Students table) ----------
    private string? FindClassRoster(string question)
    {
        // Only for "list the students" style questions, not "who teaches..." or one student's details
        if (TeachWord.IsMatch(question)) return null;
        if (!Regex.IsMatch(question, @"\b(stud\w*|names?|list|roster|everyone|who)\b", RegexOptions.IgnoreCase))
            return null;

        // "grade 9", "9th grade", "9 grade" - the last match wins so a newer question beats older history
        var gradeMatches = Regex.Matches(question,
            @"grade\s*(\d{1,2})|(\d{1,2})(?:st|nd|rd|th)?\s*grade", RegexOptions.IgnoreCase);
        if (gradeMatches.Count == 0) return null;

        var gm = gradeMatches[^1];
        int n = int.Parse(gm.Groups[1].Success ? gm.Groups[1].Value : gm.Groups[2].Value);

        string suffix = (n % 100 is >= 11 and <= 13)
            ? "th"
            : (n % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" };
        var grade = $"{n}{suffix} Grade";

        var sectionMatches = Regex.Matches(question, @"(?:section|class)\s*([A-Za-z])\b", RegexOptions.IgnoreCase);
        string? section = sectionMatches.Count > 0 ? sectionMatches[^1].Groups[1].Value.ToUpper() : null;

        var query = _context.Students.Where(s => s.GradeLevel == grade);
        if (section != null) query = query.Where(s => s.Section == section);

        var students = query.OrderBy(s => s.Section).ThenBy(s => s.FullName).ToList();
        if (students.Count == 0) return null;

        var where = section != null ? $"{grade} Section {section}" : grade;
        return $"{where} has {students.Count} students: " +
               string.Join("; ", students.Select(s => $"{s.FullName} (Section {s.Section})"));
    }

    // ---------- keyword retrieval (with spelling correction) ----------
    private List<string> FindByKeyword(string question, int topN)
    {
        var chunks = _context.KnowledgeChunks.ToList()
            .Select(c => new
            {
                c.SourceType,
                c.Content,
                TitleWords = Tokenize(c.Title).ToList(),
                ContentWords = Tokenize(c.Content).ToHashSet()
            })
            .ToList();

        // Every word the knowledge base knows (names, subjects, "absences", ...)
        var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in chunks)
        {
            foreach (var w in c.TitleWords) known.Add(w);
            foreach (var w in c.ContentWords) known.Add(w);
        }

        // Fix typos / glued words in the question before matching
        var questionWords = Tokenize(question).Select(w => CorrectWord(w, known)).ToHashSet();

        // 1. A person's name appears in the question
        var byName = chunks
            .Select(c => new
            {
                c.Content,
                NameHits = c.TitleWords.Count(w => questionWords.Contains(w)),
                OtherHits = questionWords.Count(w => c.ContentWords.Contains(w))
            })
            .Where(x => x.NameHits > 0)
            .OrderByDescending(x => x.NameHits * 3 + x.OtherHits)
            .Take(topN)
            .Select(x => x.Content)
            .ToList();

        if (byName.Count > 0) return byName;

        // 2. No name: general question about subjects, grades or teachers
        bool teacherQuestion = TeachWord.IsMatch(question);

        return chunks
            .Where(c => !teacherQuestion || c.SourceType == "Teacher")
            .Select(c => new { c.Content, Hits = questionWords.Count(w => c.ContentWords.Contains(w)) })
            .Where(x => x.Hits >= 2)
            .OrderByDescending(x => x.Hits)
            .Take(5)
            .Select(x => x.Content)
            .ToList();
    }

    // "mathat" -> "math", "abigal" -> "abigail", "biolgy" -> "biology"
    private static string CorrectWord(string word, HashSet<string> known)
    {
        if (word.Length < 4 || known.Contains(word) || word.Any(char.IsDigit))
            return word;

        // Words stuck together: use the longest known word found inside it
        var inside = known
            .Where(k => k.Length >= 4 && !k.Any(char.IsDigit) && word.Contains(k, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(k => k.Length)
            .FirstOrDefault();
        if (inside != null) return inside;

        // Small spelling mistakes: closest known word within 1 (short words) or 2 (long words) edits
        int maxDistance = word.Length <= 5 ? 1 : 2;
        string? best = null;
        int bestDistance = maxDistance + 1;

        foreach (var k in known)
        {
            if (k.Length < 4 || k.Any(char.IsDigit) || Math.Abs(k.Length - word.Length) > maxDistance)
                continue;

            int d = EditDistance(word, k);
            if (d < bestDistance)
            {
                bestDistance = d;
                best = k;
            }
        }

        return best ?? word;
    }

    private static int EditDistance(string a, string b)
    {
        var prev = new int[b.Length + 1];
        var curr = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) prev[j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            curr[0] = i;
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                curr[j] = Math.Min(Math.Min(curr[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
            }
            (prev, curr) = (curr, prev);
        }
        return prev[b.Length];
    }

    // ---------- vector retrieval ----------
    private async Task<List<string>> FindByVectorAsync(string question, int topN)
    {
        var questionVector = await _ollama.GetEmbeddingAsync(question);

        return _context.KnowledgeChunks.ToList()
            .Where(c => !string.IsNullOrEmpty(c.EmbeddingJson))
            .Select(c => new
            {
                c.Content,
                Score = CosineSimilarity(questionVector, JsonSerializer.Deserialize<float[]>(c.EmbeddingJson)!)
            })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Content)
            .ToList();
    }

    private static IEnumerable<string> Tokenize(string text) =>
        Regex.Split(text.ToLowerInvariant(), @"[^\p{L}\p{N}]+")
             .Where(w => w.Length >= 2 && !StopWords.Contains(w));

    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}