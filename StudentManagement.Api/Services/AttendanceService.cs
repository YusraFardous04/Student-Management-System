using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Services
{
    public interface IAttendanceService
    {
        List<Attendance> GetByEnrollment(int enrollmentId);
        Attendance Mark(int enrollmentId, DateTime date, AttendanceStatus status);

    }


    public class AttendanceService : IAttendanceService
    {
        private readonly StudentDbContext _context;
        public AttendanceService(StudentDbContext context) => _context = context;


        public List<Attendance> GetByEnrollment(int enrollmentId) =>
            _context.Attendances
                .Where(a => a.EnrollmentId == enrollmentId)
                .OrderByDescending(a => a.Date)
                .ToList(); 
        public Attendance Mark(int enrollmentId, DateTime date, AttendanceStatus status)
        {

            var existing=_context.Attendances.FirstOrDefault(a => a.EnrollmentId == enrollmentId && a.Date.Date == date.Date);
            if(existing is not null)
            {
                existing.Status = status;
                _context.SaveChanges();
                return existing;

            }

            var attendance = new Attendance { EnrollmentId = enrollmentId, Date = date.Date, Status = status };
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
            
            return attendance;


        }


    }



}
