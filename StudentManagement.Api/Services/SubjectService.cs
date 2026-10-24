
using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;


namespace StudentManagement.Api.Services
{

    public interface ISubjectService
    {
        List<Subject> GetAll();
        Subject? GetById(int id);
     
        List<Enrollment> GetRoster(int subjectId);

    }
    public class SubjectService: ISubjectService
    {

        private readonly StudentDbContext _context;
        public SubjectService(StudentDbContext context) => _context = context;
        public List<Subject> GetAll() => _context.Subjects.ToList();
       

        public Subject? GetById(int id) => _context.Subjects.Find(id);


        public List<Enrollment> GetRoster(int subjectId)=> 
            _context.Enrollments
                .Include(e=>e.Student)
                 .Include(e=>e.Subject)
                 .Where(e=>e.SubjectId==subjectId)
                 .ToList();


    }


}
