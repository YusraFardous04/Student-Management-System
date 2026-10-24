using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentsController(IEnrollmentService enrollmentService) => _enrollmentService = enrollmentService;

        [HttpGet("student/{studentId}")]
        public IActionResult GetByStudent(int studentId) => Ok(_enrollmentService.GetByStudent(studentId));

        [HttpPost]
        public IActionResult Enroll([FromQuery] int studentId, [FromQuery] int subjectId)
        {
            var (success, error) = _enrollmentService.Enroll(studentId, subjectId);
            return success ? Ok() : BadRequest(new { error });
        }

        [HttpDelete("{enrollmentId}")]
        public IActionResult Unenroll(int enrollmentId)
            => _enrollmentService.Unenroll(enrollmentId) ? NoContent() : NotFound();
    }
}