namespace StudentManagement.Web.models;

public class AskRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
}

public class AskResponse
{
    public string Answer { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}