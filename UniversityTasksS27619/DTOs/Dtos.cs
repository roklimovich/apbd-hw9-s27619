namespace UniversityTasksDbFirstApi.DTOs;

// ---- Response DTOs ----

public class CourseDto
{
    public int CourseId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Credits { get; set; }
    public bool IsActive { get; set; }
    public int AssignmentCount { get; set; }
}

public class AssignmentDto
{
    public int AssignmentId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public int MaxPoints { get; set; }
    public bool IsPublished { get; set; }
    public int SubmissionCount { get; set; }
}

public class EnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public DateOnly EnrolledAt { get; set; }
    public string Status { get; set; } = null!;
}

public class SubmissionSummaryDto
{
    public int SubmissionId { get; set; }
    public int AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = null!;
    public string RepositoryUrl { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = null!;
}

public class StudentDashboardDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public string Email { get; set; } = null!;
    public DateOnly EnrollmentDate { get; set; }
    public List<EnrollmentDto> Enrollments { get; set; } = new();
    public List<SubmissionSummaryDto> Submissions { get; set; } = new();
}

public class StudentInfoDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
}

public class AssignmentInfoDto
{
    public int AssignmentId { get; set; }
    public string Title { get; set; } = null!;
    public int CourseId { get; set; }
}

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public StudentInfoDto Student { get; set; } = null!;
    public AssignmentInfoDto Assignment { get; set; } = null!;
    public string RepositoryUrl { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = null!;
}


public class CreateSubmissionDto
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string RepositoryUrl { get; set; } = null!;
}

public class GradeSubmissionDto
{
    public int Score { get; set; }
    public string? Feedback { get; set; }
}
