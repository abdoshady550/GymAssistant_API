using Asp.Versioning;
using GymAssistant_API.Handeler.Identity.Trainer;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public class TrainerController(TrainerHandler handler) : ApiController
    {
        private readonly TrainerHandler _handler = handler;

        //[HttpPost("trainees/{traineeId:guid}")]
        //[Authorize]
        //[ProducesResponseType(typeof(TrainerTraineeResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //[EndpointSummary("Adds a trainee to the authenticated trainer.")]
        //[EndpointDescription("Creates a relationship between the trainer and the specified trainee.")]
        //[EndpointName("AddTrainee")]
        //public async Task<IActionResult> AddTrainee([FromRoute] Guid traineeId, CancellationToken ct = default)
        //{
        //    var result = await _handler.AddTrainee(GetCurrentUserId(), traineeId, ct);
        //    return result.Match(
        //        response => Ok(response),
        //        Problem);
        //}

        [HttpGet("trainees")]
        [Authorize]
        [ProducesResponseType(typeof(List<TraineeData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all trainees for the authenticated trainer.")]
        [EndpointDescription("Fetches a list of all trainees assigned to the current trainer.")]
        [EndpointName("GetTrainees")]
        public async Task<ActionResult> GetTrainees(CancellationToken ct = default)
        {
            var result = await _handler.GetTrainees(GetCurrentUserId(), ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("trainees/{traineeId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(TraineeData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a specific trainee for the authenticated trainer.")]
        [EndpointDescription("Fetches detailed information about the specified trainee.")]
        [EndpointName("GetTrainee")]
        public async Task<ActionResult> GetTrainee([FromRoute] Guid traineeId, CancellationToken ct = default)
        {
            var result = await _handler.GetTrainee(GetCurrentUserId(), traineeId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpDelete("trainees/{traineeId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Deleted), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Removes a trainee from the authenticated trainer.")]
        [EndpointDescription("Deletes the relationship between the trainer and the specified trainee.")]
        [EndpointName("RemoveTrainee")]
        public async Task<ActionResult> RemoveTrainee([FromRoute] Guid traineeId, CancellationToken ct = default)
        {
            var result = await _handler.RemoveTrainee(GetCurrentUserId(), traineeId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpPost("trainees/{traineeId:guid}/sessions")]
        [Authorize]
        [ProducesResponseType(typeof(WorkoutSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a workout session for a trainee.")]
        [EndpointDescription("Creates a new workout session for the specified trainee by the authenticated trainer.")]
        [EndpointName("CreateSessionForTrainee")]
        public async Task<ActionResult> CreateSessionForTrainee([FromRoute] Guid traineeId,
                                                                 [FromQuery] DateTime date,
                                                                 [FromQuery] string? notes = null,
                                                                 CancellationToken ct = default)
        {
            var result = await _handler.CreateSessionForTrainee(GetCurrentUserId(), traineeId, date, notes, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("trainees/{traineeId:guid}/sessions")]
        [Authorize]
        [ProducesResponseType(typeof(List<WorkoutSessionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves workout sessions for a trainee.")]
        [EndpointDescription("Fetches a paginated list of workout sessions for the specified trainee.")]
        [EndpointName("GetTraineeSessions")]
        public async Task<ActionResult> GetTraineeSessions([FromRoute] Guid traineeId,
                                                            [FromQuery] int pageSize = 10,
                                                            [FromQuery] int pageNumber = 1,
                                                            CancellationToken ct = default)
        {
            var result = await _handler.GetTraineeSessions(GetCurrentUserId(), traineeId, pageSize, pageNumber, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("trainees/{traineeId:guid}/sessions/{sessionId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(WorkoutSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a specific workout session for a trainee.")]
        [EndpointDescription("Fetches detailed information about a specific workout session for the specified trainee.")]
        [EndpointName("GetTraineeSession")]
        public async Task<ActionResult> GetTraineeSession([FromRoute] Guid traineeId,
                                                           [FromRoute] Guid sectionId = default,
                                                           CancellationToken ct = default)
        {
            var result = await _handler.GetTraineeSession(GetCurrentUserId(), traineeId, sectionId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("trainees/{traineeId:guid}/progress")]
        [Authorize]
        [ProducesResponseType(typeof(TraineeProgressData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves progress data for a trainee.")]
        [EndpointDescription("Fetches comprehensive progress information for the specified trainee over a given time period.")]
        [EndpointName("GetTraineeProgress")]
        public async Task<ActionResult> GetTraineeProgress([FromRoute] Guid traineeId,
                                                            [FromQuery] int days = 30,
                                                            [FromQuery] Guid sectionId = default,
                                                            CancellationToken ct = default)
        {
            var result = await _handler.GetTraineeProgress(GetCurrentUserId(), traineeId, days, sectionId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("dashboard")]
        [Authorize]
        [ProducesResponseType(typeof(TrainerDashboardData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves dashboard data for the authenticated trainer.")]
        [EndpointDescription("Fetches comprehensive dashboard statistics and information for the current trainer.")]
        [EndpointName("GetTrainerDashboard")]
        public async Task<ActionResult> GetTrainerDashboard(CancellationToken ct = default)
        {
            var result = await _handler.GetTrainerDashboard(GetCurrentUserId(), ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
