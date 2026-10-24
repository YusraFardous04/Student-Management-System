using StudentManagement.Api.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;
using StudentManagement.Api.Data;
using LicenseContext = System.ComponentModel.LicenseContext;
namespace StudentManagement.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher> AddTeacherAsync(Teacher teacher, string subjectName);
    }

    public class TeacherService : ITeacherService
    {
        private readonly StudentDbContext _context;

        public TeacherService(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Include(t => t.Subject)
                .ToListAsync();
        }

        public async Task<Teacher> AddTeacherAsync(Teacher teacher, string subjectName)
        {
            // Find the Subject entity in SQL Server and set SubjectId (Foreign Key)
            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                var matchedSubject = await _context.Subjects
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == subjectName.ToLower());

                if (matchedSubject != null)
                {
                    teacher.SubjectId = matchedSubject.Id; // Sets the FK integer
                }
            }

            // Save Teacher to SQL Server
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            // Append record to Excel file
            AppendTeacherToExcel(teacher, subjectName);

            return teacher;
        }

        private void AppendTeacherToExcel(Teacher teacher, string subjectName)
        {
            // Fully qualify OfficeOpenXml.LicenseContext to resolve CS0104 ambiguity
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "teachers_dataset.xlsx");
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                using var package = new ExcelPackage(fileInfo);
                var worksheet = package.Workbook.Worksheets[0];
                int newRow = (worksheet.Dimension?.End.Row ?? 1) + 1;

                worksheet.Cells[newRow, 1].Value = teacher.FullName;
                worksheet.Cells[newRow, 2].Value = teacher.Email;
                worksheet.Cells[newRow, 3].Value = subjectName;
                worksheet.Cells[newRow, 4].Value = teacher.GradeLevel;

                package.Save();
            }
        }
    }
}