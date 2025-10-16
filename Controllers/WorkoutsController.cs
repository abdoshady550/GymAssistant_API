using Asp.Versioning;
using GymAssistant_API.Handeler.Exercise.Workout;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public sealed class WorkoutsController(WorkoutHandler workout) : ApiController
    {
        private readonly WorkoutHandler _workout = workout;

        [HttpPost("create-session")]
        [ProducesResponseType(typeof(WorkoutSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a new workout session for a client profile.")]
        [EndpointDescription("Creates a new workout session for a specified client profile with optional notes and trainer information.")]
        [EndpointName("CreateWorkoutSession")]
        public async Task<ActionResult> CreateSession([FromBody] CreateWorkoutSessionRequest request,
                                                       CancellationToken ct = default)
        {
            var result = await _workout.CreateWorkoutSession(GetCurrentUserId(), request, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("get-session")]
        [ProducesResponseType(typeof(WorkoutSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a workout session by its ID.")]
        [EndpointDescription("Fetches a workout session for the current user based on the provided session ID.")]
        [EndpointName("GetWorkoutSessionById")]
        public async Task<ActionResult> GetSessionById([FromQuery] Guid id,
                                                        CancellationToken ct = default)
        {
            var result = await _workout.GetWorkoutSession(GetCurrentUserId(), id, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("start-Workout-session")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Starts a workout session by setting its start time.")]
        [EndpointDescription("Marks the start of a workout session for the current user by updating the start time.")]
        [EndpointName("StartWorkoutSession")]
        public async Task<ActionResult> StartWorkoutSession([FromQuery] Guid id,
                                                             CancellationToken ct = default)
        {
            var startTime = DateTime.UtcNow;
            var result = await _workout.StartWorkoutSession(GetCurrentUserId(), id, startTime, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("complete-workout-session")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Completes a workout session by setting its end time and marking it as completed.")]
        [EndpointDescription("Marks the completion of a workout session for the current user by updating the end time and setting it as completed.")]
        [EndpointName("CompleteWorkoutSession")]
        public async Task<ActionResult> CompleteWorkoutSession([FromQuery] Guid id, string? notes = null, CancellationToken ct = default)
        {
            var endTime = DateTime.UtcNow;
            var result = await _workout.CompleteWorkoutSession(GetCurrentUserId(), id, endTime, notes, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPost("add-exercise-Workout")]
        [ProducesResponseType(typeof(WorkoutExerciseRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Adds an exercise to a workout session.")]
        [EndpointDescription("Adds a specified exercise to an existing workout session for the current user.")]
        [EndpointName("AddExerciseToWorkoutSession")]
        public async Task<ActionResult> AddExerciseToWorkoutSession([FromQuery] Guid sessionId,
                                                                     [FromQuery] Guid? exerciseId = null,
                                                                     [FromQuery] Guid? userExerciseId = null,
                                                                     [FromQuery] CancellationToken ct = default)
        {
            var result = await _workout.AddExerciseToWorkout(GetCurrentUserId(), sessionId, exerciseId, userExerciseId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("get-workout-exercise")]
        [ProducesResponseType(typeof(WorkoutExerciseRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a workout exercise by its ID.")]
        [EndpointDescription("Fetches a workout exercise for the current user based on the provided exercise ID.")]
        [EndpointName("GetWorkoutExerciseById")]
        public async Task<ActionResult> GetWorkoutExerciseById([FromQuery] Guid sessionId, [FromQuery] Guid exerciseId, CancellationToken ct = default)
        {
            var result = await _workout.GetWorkoutExercise(GetCurrentUserId(), sessionId, exerciseId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPost("add-set-to-exercise")]
        [ProducesResponseType(typeof(ExerciseSetRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Adds a set to a workout exercise.")]
        [EndpointDescription("Adds a specified set to an existing workout exercise for the current user.")]
        [EndpointName("AddSetToWorkoutExercise")]
        public async Task<ActionResult> AddSetToWorkoutExercise([FromQuery] Guid sessionId,
                                                                 [FromQuery] Guid exerciseId,
                                                                 [FromBody] AddExerciseSetRequest request,
                                                                 CancellationToken ct = default)
        {
            var result = await _workout.AddSetToExercise(GetCurrentUserId(), sessionId, exerciseId, request, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("exercise-set")]
        [ProducesResponseType(typeof(ExerciseSetRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves an exercise set by its ID.")]
        [EndpointDescription("Fetches an exercise set for the current user based on the provided set ID.")]
        [EndpointName("GetExerciseSetById")]
        public async Task<ActionResult> GetExerciseSetById([FromQuery] Guid sessionId,
                                                            [FromQuery] Guid exerciseId,
                                                            [FromQuery] Guid setId,
                                                            CancellationToken ct = default)
        {
            var result = await _workout.GetExerciseSet(GetCurrentUserId(), sessionId, exerciseId, setId, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("update-exercise-set")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates an existing exercise set.")]
        [EndpointDescription("Updates the details of an existing exercise set for the current user.")]
        [EndpointName("UpdateExerciseSet")]
        public async Task<ActionResult> UpdateExerciseSet([FromQuery] Guid sessionId,
                                                           [FromQuery] Guid exerciseId,
                                                           [FromQuery] Guid setId,
                                                           [FromBody] UpdateExerciseSetRequest request,
                                                           CancellationToken ct = default)
        {
            var result = await _workout.UpdateExerciseSet(GetCurrentUserId(), sessionId, exerciseId, setId, request, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("get-workout-history")]
        [ProducesResponseType(typeof(List<WorkoutSessionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the workout history for the current user.")]
        [EndpointDescription("Fetches all past workout sessions for the current user, ordered by date in descending order.")]
        [EndpointName("GetWorkoutHistory")]
        public async Task<ActionResult> GetWorkoutHistory([FromQuery] int pageSize = 20,
                                                           [FromQuery] int pageNumber = 1,
                                                           [FromBody] DateTime? fromDate = null,
                                                           DateTime? toDate = null,
                                                           CancellationToken ct = default)
        {
            var result = await _workout.GetWorkoutHistory(GetCurrentUserId(), pageSize, pageNumber, fromDate, toDate, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }


}

