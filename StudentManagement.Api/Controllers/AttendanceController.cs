using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.models;
using StudentManagement.Api.Services;

namespace StudentManagement.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService) => _attendanceService = attendanceService;

        [HttpGet("enrollment/{enrollmentId}")]
        public IActionResult GetByEnrollment(int enrollmentId) => Ok(_attendanceService.GetByEnrollment(enrollmentId));

        [HttpPost]
        public IActionResult Mark([FromQuery] int enrollmentId, [FromQuery] DateTime date, [FromQuery] AttendanceStatus status)
            => Ok(_attendanceService.Mark(enrollmentId, date, status));
    }
}