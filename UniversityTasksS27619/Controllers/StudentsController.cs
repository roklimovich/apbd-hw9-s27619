using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    // GET /api/students/{idStudent}/dashboard
    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(int idStudent)
    {
        // Use projection to avoid N+1 and load all data in one query
        var dashboard = await _context.Students
            .AsNoTracking()
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FirstName + " " + s.LastName,
                IsActive = s.IsActive,
                Email = s.Email,
                EnrollmentDate = s.EnrollmentDate,
                Enrollments = s.Enrollments.Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    CourseCode = e.Course.Code,
                    CourseName = e.Course.Name,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status
                }).ToList(),
                Submissions = s.Submissions.Select(sub => new SubmissionSummaryDto
                {
                    SubmissionId = sub.SubmissionId,
                    AssignmentId = sub.AssignmentId,
                    AssignmentTitle = sub.Assignment.Title,
                    RepositoryUrl = sub.RepositoryUrl,
                    SubmittedAt = sub.SubmittedAt,
                    Score = sub.Score,
                    Feedback = sub.Feedback,
                    Status = sub.Status
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (dashboard == null)
            return NotFound($"Student with id {idStudent} not found.");

        return Ok(dashboard);
    }
}
