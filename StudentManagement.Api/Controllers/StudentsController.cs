using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.models;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentServices _studentServices;
    private readonly IEnrollmentService _enrollmentService;

    public StudentsController(IStudentServices studentServices, IEnrollmentService enrollmentService)
    {
        _studentServices = studentServices;
        _enrollmentService = enrollmentService;
    }

    // GET: api/Students OR api/Students?searchTerm=john
   
    [HttpGet]
    public ActionResult<List<Student>> GetALL()
    {
        var students = _studentServices.GetALL() ?? new List<Student>();
        return Ok(students);
    }

       
    [HttpGet("{id}")]
    public ActionResult<Student> GetById(int id)
    {
        var student = _studentServices.GetById(id);
        if (student == null) return NotFound();

        var enrollments = _enrollmentService.GetByStudent(id);
        student.EnrolledSubjectIds = enrollments.Select(e => e.SubjectId).ToList();

        return Ok(student);
    }

    [HttpPost]
    public ActionResult<Student> Create([FromBody] Student student)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = _studentServices.Create(student);

            if (student.EnrolledSubjectIds != null && student.EnrolledSubjectIds.Any())
            {
                foreach (var subjectId in student.EnrolledSubjectIds)
                {
                    _enrollmentService.Enroll(created.Id, subjectId);
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database Error: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Student student)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = _studentServices.Update(id, student);
        if (!updated) return NotFound();

        // Sync subject enrollments: remove existing enrollments and re-add selected ones
        var existingEnrollments = _enrollmentService.GetByStudent(id);
        foreach (var enrollment in existingEnrollments)
        {
            _enrollmentService.Unenroll(enrollment.Id);
        }

        if (student.EnrolledSubjectIds != null && student.EnrolledSubjectIds.Any())
        {
            foreach (var subjectId in student.EnrolledSubjectIds)
            {
                _enrollmentService.Enroll(id, subjectId);
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _studentServices.Delete(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}