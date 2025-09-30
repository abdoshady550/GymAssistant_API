using Asp.Versioning;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public sealed class ExercisesController(CustomExerciseHandler customExercise,
                                            ExerciseHandler exercise) : ApiController
    {
        private readonly CustomExerciseHandler _customExercise = customExercise;
        private readonly ExerciseHandler exercise = exercise;

        [HttpPost("create-custom-exercise")]
        [Consumes("multipart/form-data")]
        [Authorize]
        [ProducesResponseType(typeof(CustomExerciseRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a custom exercise for the authenticated user.")]
        [EndpointDescription("Allows the authenticated user to create a custom exercise with specified details.")]
        [EndpointName("CreateCustomExercise")]
        public async Task<IActionResult> CreateCustomExercise([FromForm] CustomExerciseReq req,
                                                              CancellationToken ct = default)
        {

            var result = await _customExercise.CreateCustomExercise(GetCurrentUserId(), req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPut("update-custom-exercise")]
        [Consumes("multipart/form-data")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates a custom exercise for the authenticated user.")]
        [EndpointDescription("Allows the authenticated user to update a custom exercise by its ID.")]
        [EndpointName("UpdateCustomExercise")]
        public async Task<IActionResult> UpdateCustomExercise([FromQuery] Guid exerciseId,
                                                              [FromForm] CustomExerciseReq req,
                                                              CancellationToken ct = default)
        {
            var result = await _customExercise.UpdateCustomExercise(GetCurrentUserId(), exerciseId, req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("delete-custom-exercise")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Deletes a custom exercise for the authenticated user.")]
        [EndpointDescription("Allows the authenticated user to delete a custom exercise by its ID.")]
        [EndpointName("DeleteCustomExercise")]

        public async Task<IActionResult> DeleteCustomExercise([FromQuery] Guid exerciseId,
                                                              CancellationToken ct = default)
        {
            var result = await _customExercise.DeleteCustomExercise(GetCurrentUserId(), exerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("get-custom-exercises")]
        [Authorize]
        [ProducesResponseType(typeof(List<CustomExerciseRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all custom exercises for the authenticated user.")]
        [EndpointDescription("Fetches all custom exercises created by the authenticated user.")]
        [EndpointName("GetCustomExercises")]
        public async Task<IActionResult> GetCustomExercises(CancellationToken ct = default)
        {
            var result = await _customExercise.GetCustomExercises(GetCurrentUserId(), ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("get-custom-exercise")]
        [Authorize]
        [ProducesResponseType(typeof(CustomExerciseRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a specific custom exercise for the authenticated user.")]
        [EndpointDescription("Fetches a specific custom exercise by its ID for the authenticated user.")]
        [EndpointName("GetCustomExercise")]
        public async Task<IActionResult> GetCustomExercise([FromQuery] Guid exerciseId,
                                                           CancellationToken ct = default)
        {
            var result = await _customExercise.GetCustomExercise(GetCurrentUserId(), exerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("get-exercise-by-id")]
        [Authorize]
        [ProducesResponseType(typeof(ExerciseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a specific exercise by its ID.")]
        [EndpointDescription("Fetches a specific exercise by its ID.")]
        [EndpointName("GetExerciseById")]
        public async Task<IActionResult> GetExerciseById([FromQuery] Guid exerciseId,
                                                           CancellationToken ct = default)
        {
            var result = await exercise.GetExerciseById(exerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("exercises-by-section")]
        [Authorize]
        [ProducesResponseType(typeof(List<ExerciseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves exercises by section ID.")]
        [EndpointDescription("Fetches exercises belonging to a specific section, optionally filtered by difficulty level.")]
        [EndpointName("GetExercisesBySection")]
        public async Task<IActionResult> ExercisesBySection([FromQuery] Guid sectionId, [FromQuery] DifficultyLevel? difficulty = null,
                                                           CancellationToken ct = default)
        {

            var result = await exercise.ExercisesBySection(sectionId, difficulty, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("get-sections")]
        [Authorize]
        [ProducesResponseType(typeof(List<SectionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all exercise sections.")]
        [EndpointDescription("Fetches all available exercise sections.")]
        [EndpointName("GetSections")]
        public async Task<IActionResult> GetSections(CancellationToken ct = default)
        {
            var result = await exercise.GetSections(ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("create-exercises")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ExerciseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a new exercise.")]
        [EndpointDescription("Allows an admin user to create a new exercise with specified details.")]
        [EndpointName("CreateExercise")]
        public async Task<IActionResult> CreateExercise([FromQuery] Guid sectionId, [FromForm] ExerciseReq req,
                                                       CancellationToken ct = default)
        {

            var result = await exercise.CreateExercise(sectionId, req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPut("update-exercises")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates an existing exercise.")]
        [EndpointDescription("Allows an admin user to update an existing exercise by its ID.")]
        [EndpointName("UpdateExercise")]
        public async Task<IActionResult> UpdateExercise([FromQuery] Guid exerciseId,
                                                       [FromQuery] Guid sectionId,
                                                       [FromForm] UpdateExerciseReq req,
                                                       CancellationToken ct = default)
        {

            var result = await exercise.UpdateExercise(exerciseId, sectionId, req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("delete-exercises")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Deletes an existing exercise.")]
        [EndpointDescription("Allows an admin user to delete an existing exercise by its ID.")]
        [EndpointName("DeleteExercise")]
        public async Task<IActionResult> DeleteExercise([FromQuery] Guid exerciseId,
                                                       CancellationToken ct = default)
        {

            var result = await exercise.DeleteExercise(exerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
