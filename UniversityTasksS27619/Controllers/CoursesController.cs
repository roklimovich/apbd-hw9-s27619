using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public CoursesController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    // GET /api/courses?activeOnly=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses([FromQuery] bool activeOnly = false)
    {
        var query = _context.Courses.AsNoTracking();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var courses = await query
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Code = c.Code,
                Name = c.Name,
                Credits = c.Credits,
                IsActive = c.IsActive,
                AssignmentCount = c.Assignments.Count
            })
            .ToListAsync();

        return Ok(courses);
    }

    // GET /api/courses/{idCourse}/assignments?publishedOnly=true
    [HttpGet("{idCourse:int}/assignments")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetCourseAssignments(
        int idCourse,
        [FromQuery] bool publishedOnly = false)
    {
        var courseExists = await _context.Courses.AsNoTracking().AnyAsync(c => c.CourseId == idCourse);
        if (!courseExists)
            return NotFound($"Course with id {idCourse} not found.");

        var query = _context.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == idCourse);

        if (publishedOnly)
            query = query.Where(a => a.IsPublished);

        var assignments = await query
            .Select(a => new AssignmentDto
            {
                AssignmentId = a.AssignmentId,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints,
                IsPublished = a.IsPublished,
                SubmissionCount = a.Submissions.Count
            })
            .ToListAsync();

        return Ok(assignments);
    }
}
