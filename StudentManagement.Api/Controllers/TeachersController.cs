using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;

namespace StudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly StudentDbContext _context;

    public TeachersController(StudentDbContext context)
    {
        _context = context;
    }

    // GET: api/Teachers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers([FromQuery] string? searchTerm = null)
    {
        var query = _context.Teachers
            .Include(t => t.Subject)
            .Include(t => t.ClassAssignments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(t =>
                t.FullName.Contains(term) ||
                t.Email.Contains(term) ||
                (t.Subject != null && t.Subject.Name != null && t.Subject.Name.Contains(term))
            );
        }

        return await query.ToListAsync();
    }

    // GET: api/Teachers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Teacher>> GetTeacher(int id)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subject)
            .Include(t => t.ClassAssignments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }

        return teacher;
    }

    // POST: api/Teachers
    [HttpPost]
    public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
    {
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Reload the navigation reference for Subject to make sure it is sent back in JSON
        if (teacher.SubjectId.HasValue)
        {
            await _context.Entry(teacher).Reference(t => t.Subject).LoadAsync();
        }

        return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
    }

    // PUT: api/Teachers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
    {
        if (id != teacher.Id)
        {
            return BadRequest();
        }

        _context.Entry(teacher).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeacherExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Teachers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TeacherExists(int id)
    {
        return _context.Teachers.Any(e => e.Id == id);
    }
}