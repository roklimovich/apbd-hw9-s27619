using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<(SubmissionDto? result, int statusCode, string? error)> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        // Validate RepositoryUrl
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) || !dto.RepositoryUrl.StartsWith("https://"))
            return (null, 400, "RepositoryUrl cannot be blank and must start with https://");

        // Check student exists and is active
        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
            return (null, 404, $"Student with id {dto.StudentId} not found.");
        if (!student.IsActive)
            return (null, 400, "Student is not active.");

        // Check assignment exists and is published
        var assignment = await _context.Assignments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId);
        if (assignment == null)
            return (null, 404, $"Assignment with id {dto.AssignmentId} not found.");
        if (!assignment.IsPublished)
            return (null, 400, "Assignment is not published.");

        // Check student is enrolled in the course (Active or Completed)
        var enrolled = await _context.Enrollments.AnyAsync(e =>
            e.StudentId == dto.StudentId &&
            e.CourseId == assignment.CourseId &&
            (e.Status == "Active" || e.Status == "Completed"));
        if (!enrolled)
            return (null, 400, "Student is not enrolled (Active or Completed) in the course that owns this assignment.");

        // Check duplicate submission
        var duplicate = await _context.Submissions.AnyAsync(s =>
            s.AssignmentId == dto.AssignmentId && s.StudentId == dto.StudentId);
        if (duplicate)
            return (null, 409, "Student has already submitted this assignment.");

        var now = DateTime.UtcNow;
        var status = assignment.IsOverdue(now) ? "Late" : "Submitted";

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = now,
            Status = status
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        // Reload with navigation properties for response
        var created = await _context.Submissions
            .AsNoTracking()
            .Include(s => s.Student)
            .Include(s => s.Assignment)
            .FirstAsync(s => s.SubmissionId == submission.SubmissionId);

        return (MapToSubmissionDto(created), 201, null);
    }

    public async Task<(SubmissionDto? result, int statusCode, string? error)> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return (null, 404, $"Submission with id {submissionId} not found.");

        if (dto.Score < 0)
            return (null, 400, "Score cannot be lower than 0.");

        if (dto.Score > submission.Assignment.MaxPoints)
            return (null, 400, $"Score cannot be higher than the assignment's MaxPoints ({submission.Assignment.MaxPoints}).");

        // Change Tracker: load entity, modify, save
        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync();

        return (MapToSubmissionDto(submission), 200, null);
    }

    public async Task<(bool success, int statusCode, string? error)> DeleteSubmissionAsync(int submissionId)
    {
        var submission = await _context.Submissions.FindAsync(submissionId);
        if (submission == null)
            return (false, 404, $"Submission with id {submissionId} not found.");

        if (submission.Status == "Graded")
            return (false, 400, "A graded submission cannot be deleted.");

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();

        return (true, 204, null);
    }

    public static SubmissionDto MapToSubmissionDto(Submission s) => new()
    {
        SubmissionId = s.SubmissionId,
        Student = new StudentInfoDto
        {
            StudentId = s.Student.StudentId,
            IndexNumber = s.Student.IndexNumber,
            FullName = s.Student.FullName
        },
        Assignment = new AssignmentInfoDto
        {
            AssignmentId = s.Assignment.AssignmentId,
            Title = s.Assignment.Title,
            CourseId = s.Assignment.CourseId
        },
        RepositoryUrl = s.RepositoryUrl,
        SubmittedAt = s.SubmittedAt,
        Score = s.Score,
        Feedback = s.Feedback,
        Status = s.Status
    };
}
