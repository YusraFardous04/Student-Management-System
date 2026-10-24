namespace StudentManagement.Api.models;

public class KnowledgeChunk
{
    public int Id { get; set; }
    public string SourceType { get; set; } = string.Empty; // "Student" or "Teacher"
    public int SourceId { get; set; }                       // the real Student.Id or Teacher.Id
    public string Title { get; set; } = string.Empty;       // the person's full name
    public string Content { get; set; } = string.Empty;     // the profile paragraph
    public string EmbeddingJson { get; set; } = string.Empty; // empty unless embeddings are on
}