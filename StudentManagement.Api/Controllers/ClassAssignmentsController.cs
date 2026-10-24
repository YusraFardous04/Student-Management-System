using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassAssignmentsController : ControllerBase
{
    private readonly IClassAssignmentService _service;

    public ClassAssignmentsController(IClassAssignmentService service) => _service = service;

    [HttpGet("teacher/{teacherId}")]
    public IActionResult GetByTeacher(int teacherId) => Ok(_service.GetByTeacher(teacherId));

    [HttpPost]
    public IActionResult Create([FromQuery] int teacherId, [FromQuery] int subjectId,
        [FromQuery] string gradeLevel, [FromQuery] string section)
    {
        _service.Assign(teacherId, subjectId, gradeLevel ?? "", section ?? "A");
        return Ok(new { Message = "Class assigned successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => _service.Delete(id) ? NoContent() : NotFound();


}