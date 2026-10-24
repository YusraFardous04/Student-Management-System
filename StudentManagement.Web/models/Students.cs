namespace StudentManagement.Web.models; // and StudentManagement.Api.models

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string GradeLevel { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;

    // Primary property for enrolled subject IDs
    public List<int> EnrolledSubjectIds { get; set; } = new();

    // Alias property so both 'SubjectIds' and 'EnrolledSubjectIds' work seamlessly
    public List<int> SubjectIds
    {
        get => EnrolledSubjectIds;
        set => EnrolledSubjectIds = value ?? new List<int>();
    }
}