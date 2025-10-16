using Asp.Versioning;
using GymAssistant_API.Handeler.Progress;
using GymAssistant_API.Req_Res.Response.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public sealed class ProgressController(ProgressHandler handler) : ApiController
    {
        private readonly ProgressHandler _handler = handler;

        [HttpGet("exercise/{exerciseId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ExerciseProgressData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Exercise Progress")]
        [EndpointDescription("Retrieves the progress data for a specific exercise over a defined number of days.")]
        [EndpointName("GetExerciseProgress")]
        public async Task<ActionResult> GetExerciseProgress(Guid exerciseId, [FromQuery] int days = 30, CancellationToken ct = default)
        {
            var result = await _handler.GetExerciseProgress(GetCurrentUserId(), exerciseId, days, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("exercise/custom/{userExerciseId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ExerciseProgressData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Custom Exercise Progress")]
        [EndpointDescription("Retrieves the progress data for a specific custom exercise over a defined number of days.")]
        [EndpointName("GetCustomExerciseProgress")]
        public async Task<ActionResult> GetCustomExerciseProgress(Guid userExerciseId, [FromQuery] int days = 30, CancellationToken ct = default)
        {
            var result = await _handler.GetCustomExerciseProgress(GetCurrentUserId(), userExerciseId, days, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("section/{sectionId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<SectionProgressData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Section Progress")]
        [EndpointDescription("Retrieves the progress data for a specific section over a defined number of days.")]
        [EndpointName("GetSectionProgress")]
        public async Task<ActionResult> GetSectionProgress(Guid sectionId, [FromQuery] int days = 30, CancellationToken ct = default)
        {
            var result = await _handler.GetSectionProgress(GetCurrentUserId(), sectionId, days, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("overview")]
        [Authorize]
        [ProducesResponseType(typeof(ProgressOverviewData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Progress Overview")]
        [EndpointDescription("Retrieves an overview of the user's progress over a defined number of days.")]
        [EndpointName("GetProgressOverview")]
        public async Task<ActionResult> GetProgressOverview([FromQuery] int days = 7, CancellationToken ct = default)
        {
            var result = await _handler.GetProgressOverview(GetCurrentUserId(), days, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("charts/exercise/{exerciseId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ExerciseProgressData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Exercise Progress Charts")]
        [EndpointDescription("Retrieves chart data for a specific exercise's progress over a defined number of days.")]
        [EndpointName("GetExerciseProgressCharts")]
        public async Task<ActionResult> GetExerciseProgressCharts(Guid exerciseId, [FromQuery] int days = 30, CancellationToken ct = default)
        {
            var result = await _handler.GetExerciseChartData(GetCurrentUserId(), exerciseId, days, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("charts/volume")]
        [Authorize]
        [ProducesResponseType(typeof(VolumeChartData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Volume Progress Charts")]
        [EndpointDescription("Retrieves volume chart data over a defined number of days, optionally filtered by section.")]
        [EndpointName("GetVolumeProgressCharts")]
        public async Task<ActionResult> GetVolumeProgressCharts([FromQuery] int days = 30, [FromQuery] Guid? sectionId = null, CancellationToken ct = default)
        {
            var result = await _handler.GetVolumeChartData(GetCurrentUserId(), days, sectionId, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
