using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private readonly StudentDbContext _context;

    public SubjectsController(ISubjectService subjectService, StudentDbContext context)
    {
        _subjectService = subjectService;
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_subjectService.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var subject = _subjectService.GetById(id);
        return subject is null ? NotFound() : Ok(subject);
    }

    [HttpGet("{id}/roster")]
    public IActionResult GetRoster(int id) => Ok(_subjectService.GetRoster(id));

    [HttpPost("{subjectId}/records")]
    public IActionResult SaveRecords(int subjectId, [FromBody] List<StudentRecordInput> records)
    {
        if (records == null || !records.Any()) return BadRequest("No record data provided.");

        foreach (var record in records)
        {
            var enrollment = _context.Enrollments
                .FirstOrDefault(e => e.SubjectId == subjectId && e.StudentId == record.StudentId);

            if (enrollment != null)
            {
                enrollment.Mark = record.Mark;
                enrollment.Absences = record.Absences;
            }
            else
            {
                _context.Enrollments.Add(new Enrollment
                {
                    SubjectId = subjectId,
                    StudentId = record.StudentId,
                    Mark = record.Mark,
                    Absences = record.Absences,
                    EnrolledOn = DateTime.UtcNow
                });
            }
        }

        _context.SaveChanges();
        return Ok(new { Message = "Records saved successfully." });
    }

    // 👇 Nested record class declared directly inside the controller file (no public modifier error)
    public class StudentRecordInput
    {
        public int StudentId { get; set; }
        public int Absences { get; set; }
        public int? Mark { get; set; }
    }
}