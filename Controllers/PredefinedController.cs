using Asp.Versioning;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Predefined;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/predefined")]
    [ApiVersionNeutral]
    [Authorize]
    public sealed class PredefinedController(PredefinedDataHandler handler) : ApiController
    {
        private readonly PredefinedDataHandler _handler = handler;

        // ==========================================
        // LOCALIZED SECTIONS (MUSCLE GROUPS)
        // ==========================================

        [HttpGet("sections")]
        [ProducesResponseType(typeof(List<PredefinedSectionDto>), StatusCodes.Status200OK)]
        [EndpointSummary("Get all predefined exercise sections (localized based on Accept-Language / culture).")]
        public async Task<ActionResult> GetSections(CancellationToken ct = default)
        {
            var result = await _handler.GetSections(ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("sections/{id:guid}")]
        [ProducesResponseType(typeof(PredefinedSectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get a specific predefined section by ID (localized).")]
        public async Task<ActionResult> GetSectionById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.GetSectionById(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // LOCALIZED EXERCISES
        // ==========================================

        [HttpGet("exercises")]
        [ProducesResponseType(typeof(List<PredefinedExerciseDto>), StatusCodes.Status200OK)]
        [EndpointSummary("Get predefined exercises (localized, filterable by section, difficulty, or search term).")]
        public async Task<ActionResult> GetExercises(
            [FromQuery] Guid? sectionId = null,
            [FromQuery] DifficultyLevel? difficulty = null,
            [FromQuery] string? searchTerm = null,
            CancellationToken ct = default)
        {
            var result = await _handler.GetExercises(sectionId, difficulty, searchTerm, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("exercises/{id:guid}")]
        [ProducesResponseType(typeof(PredefinedExerciseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get a specific predefined exercise by ID (localized).")]
        public async Task<ActionResult> GetExerciseById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.GetExerciseById(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // LOCALIZED WORKDAYS (ROUTINE DAY SPLITS)
        // ==========================================

        [HttpGet("workdays")]
        [ProducesResponseType(typeof(List<PredefinedWorkdayDto>), StatusCodes.Status200OK)]
        [EndpointSummary("Get all active predefined workday splits/templates (localized).")]
        public async Task<ActionResult> GetWorkdays(CancellationToken ct = default)
        {
            var result = await _handler.GetWorkdays(ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("workdays/{id:guid}")]
        [ProducesResponseType(typeof(PredefinedWorkdayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get a specific predefined workday split by ID with its sessions (localized).")]
        public async Task<ActionResult> GetWorkdayById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.GetWorkdayById(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // LOCALIZED PREDEFINED WORKOUT SESSIONS
        // ==========================================

        [HttpGet("sessions")]
        [ProducesResponseType(typeof(List<PredefinedWorkoutSessionDto>), StatusCodes.Status200OK)]
        [EndpointSummary("Get predefined workout session templates (localized, filterable by workday or difficulty).")]
        public async Task<ActionResult> GetSessions(
            [FromQuery] Guid? workdayId = null,
            [FromQuery] DifficultyLevel? difficulty = null,
            CancellationToken ct = default)
        {
            var result = await _handler.GetSessions(workdayId, difficulty, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("sessions/{id:guid}")]
        [ProducesResponseType(typeof(PredefinedWorkoutSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get full details of a predefined workout session template including exercises (localized).")]
        public async Task<ActionResult> GetSessionById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.GetSessionById(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // PICK PREDEFINED SESSION TO WORKOUT
        // ==========================================

        [HttpPost("sessions/{id:guid}/pick-to-workout")]
        [ProducesResponseType(typeof(WorkoutSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Pick a predefined workout session template to create an actual workout session with exercises and sets for the user.")]
        public async Task<ActionResult> PickSessionToWorkout(
            [FromRoute] Guid id,
            [FromBody] PickSessionToWorkoutReq req,
            CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _handler.PickSessionToWorkout(userId, id, req, ct);
            return result.Match(response => Ok(response), Problem);
        }
    }
}
