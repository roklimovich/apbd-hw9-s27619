using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    // POST /api/submissions
    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> CreateSubmission([FromBody] CreateSubmissionDto dto)
    {
        var (result, statusCode, error) = await _submissionService.CreateSubmissionAsync(dto);

        return statusCode switch
        {
            201 => CreatedAtAction(nameof(CreateSubmission), new { id = result!.SubmissionId }, result),
            400 => BadRequest(new { message = error }),
            404 => NotFound(new { message = error }),
            409 => Conflict(new { message = error }),
            _ => StatusCode(statusCode, new { message = error })
        };
    }

    // PUT /api/submissions/{idSubmission}/grade
    [HttpPut("{idSubmission:int}/grade")]
    public async Task<ActionResult<SubmissionDto>> GradeSubmission(int idSubmission, [FromBody] GradeSubmissionDto dto)
    {
        var (result, statusCode, error) = await _submissionService.GradeSubmissionAsync(idSubmission, dto);

        return statusCode switch
        {
            200 => Ok(result),
            400 => BadRequest(new { message = error }),
            404 => NotFound(new { message = error }),
            _ => StatusCode(statusCode, new { message = error })
        };
    }

    // DELETE /api/submissions/{idSubmission}
    [HttpDelete("{idSubmission:int}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        var (success, statusCode, error) = await _submissionService.DeleteSubmissionAsync(idSubmission);

        return statusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(new { message = error }),
            404 => NotFound(new { message = error }),
            _ => StatusCode(statusCode, new { message = error })
        };
    }
}
