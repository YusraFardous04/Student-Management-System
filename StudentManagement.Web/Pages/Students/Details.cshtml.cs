using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Web.models;
using StudentManagement.Web.Services;

namespace StudentManagement.Web.Pages.Students;

public class DetailsModel : PageModel
{
    private readonly StudentApiClient _studentApi;
    private readonly TeacherApiClient _teacherApi;
    private readonly SubjectApiClient _subjectApi;

    public DetailsModel(
        StudentApiClient studentApi,
        TeacherApiClient teacherApi,
        SubjectApiClient subjectApi)
    {
        _studentApi = studentApi;
        _teacherApi = teacherApi;
        _subjectApi = subjectApi;
    }

    public Student Student { get; set; } = default!;

    // Holds subjects selected via checkboxes along with their matching teacher
    public List<(Subject Subject, string TeacherName)> EnrolledSubjectsWithTeachers { get; set; } = new();

    // Preserved to maintain compatibility across all views
    public List<ClassAssignment> AssignedClasses { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // 1. Fetch student details
        var student = await _studentApi.GetByIdAsync(id);
        if (student == null) return NotFound();

        Student = student;

        // 2. Fetch all teachers and subjects
        var teachers = await _teacherApi.GetAllAsync() ?? new List<Teacher>();
        var subjects = await _subjectApi.GetAllAsync() ?? new List<Subject>();

        // 3. Populate AssignedClasses matching Grade & Section
        AssignedClasses = teachers
            .Where(t => t.ClassAssignments != null)
            .SelectMany(t => t.ClassAssignments.Select(ca =>
            {
                ca.Teacher = t;
                if (ca.Subject == null)
                {
                    ca.Subject = subjects.FirstOrDefault(s => s.Id == ca.SubjectId);
                }
                return ca;
            }))
            .Where(ca =>
                string.Equals(ca.GradeLevel?.Trim(), Student.GradeLevel?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(ca.Section?.Trim(), Student.Section?.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        // 4. Map checked EnrolledSubjectIds to their matching Subject and Teacher
        var selectedSubjects = subjects
            .Where(s => Student.EnrolledSubjectIds != null && Student.EnrolledSubjectIds.Contains(s.Id))
            .ToList();

        foreach (var sub in selectedSubjects)
        {
            // Find teacher assigned to this subject for student's Grade and Section
            var matchingAssignment = AssignedClasses.FirstOrDefault(ca => ca.SubjectId == sub.Id);
            string teacherName = matchingAssignment?.Teacher?.FullName ?? "Unassigned";

            EnrolledSubjectsWithTeachers.Add((sub, teacherName));
        }

        return Page();
    }
}