using Asp.Versioning;
using GymAssistant_API.Handeler.Progress;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Req_Res.Response.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public class RecordsController(RecordsHandler handler) : ApiController
    {
        private readonly RecordsHandler _handler = handler;

        [HttpGet("personal")]
        [Authorize]
        [ProducesResponseType(typeof(List<PersonalRecordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves personal records for the authenticated user.")]
        [EndpointDescription("Fetches all personal records for the current user, optionally filtered by record type.")]
        [EndpointName("GetPersonalRecords")]
        public async Task<IActionResult> GetPersonalRecords([FromQuery] RecordType? recordType = null, CancellationToken ct = default)
        {
            var result = await _handler.GetPersonalRecords(GetCurrentUserId(), recordType, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("exercise/{exerciseId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<PersonalRecordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves records for a specific exercise.")]
        [EndpointDescription("Fetches all personal records for the specified exercise for the current user.")]
        [EndpointName("GetExerciseRecords")]
        public async Task<IActionResult> GetExerciseRecords([FromRoute] Guid exerciseId, CancellationToken ct = default)
        {
            var result = await _handler.GetExerciseRecords(GetCurrentUserId(), exerciseId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("exercise/custom/{userExerciseId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<PersonalRecordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves records for a specific custom exercise.")]
        [EndpointDescription("Fetches all personal records for the specified custom exercise for the current user.")]
        [EndpointName("GetCustomExerciseRecords")]
        public async Task<IActionResult> GetCustomExerciseRecords([FromRoute] Guid userExerciseId, CancellationToken ct = default)
        {
            var result = await _handler.GetCustomExerciseRecords(GetCurrentUserId(), userExerciseId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("recent")]
        [Authorize]
        [ProducesResponseType(typeof(List<PersonalRecordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves recent records for the authenticated user.")]
        [EndpointDescription("Fetches a specified number of the most recent personal records for the current user.")]
        [EndpointName("GetRecentRecords")]
        public async Task<IActionResult> GetRecentRecords([FromQuery] int count = 10, CancellationToken ct = default)
        {
            var result = await _handler.GetRecentRecords(GetCurrentUserId(), count, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("achievements")]
        [Authorize]
        [ProducesResponseType(typeof(AchievementsData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves achievement data for the authenticated user.")]
        [EndpointDescription("Fetches achievement statistics for the current user within an optional date range.")]
        [EndpointName("GetAchievements")]
        public async Task<IActionResult> GetAchievements([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, CancellationToken ct = default)
        {
            var result = await _handler.GetAchievements(GetCurrentUserId(), fromDate, toDate, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);



    }
}
