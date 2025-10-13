using Asp.Versioning;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.Exercise;
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
        public async Task<IActionResult> CreateCustomExercise([FromQuery] Guid sectionId, [FromForm] CustomExerciseReq req,
                                                              CancellationToken ct = default)
        {

            var result = await _customExercise.CreateCustomExercise(GetCurrentUserId(), sectionId, req, ct);
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
        public async Task<IActionResult> UpdateCustomExercise([FromQuery] Guid exerciseId, [FromQuery] Guid sectionId,
                                                              [FromForm] CustomExerciseReq req,
                                                              CancellationToken ct = default)
        {
            var result = await _customExercise.UpdateCustomExercise(GetCurrentUserId(), exerciseId, sectionId, req, ct);
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
        public async Task<IActionResult> GetCustomExercises([FromQuery] DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var result = await _customExercise.GetCustomExercises(GetCurrentUserId(), difficulty, ct);
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
        [HttpGet("get-sections")]
        [ProducesResponseType(typeof(List<SectionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all sections.")]
        [EndpointDescription("Fetches all available sections.")]
        [EndpointName("GetSections")]
        public async Task<IActionResult> GetSections(CancellationToken ct = default)
        {
            var result = await exercise.GetSections(ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("get-section-by-id")]
        [Authorize]
        [ProducesResponseType(typeof(SectionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a specific section by its ID.")]
        [EndpointDescription("Fetches a specific section by its ID.")]
        [EndpointName("GetSectionById")]
        public async Task<IActionResult> GetSectionById([FromQuery] Guid sectionId, CancellationToken ct = default)

        {
            var result = await exercise.GetSectionByIdAsync(GetCurrentUserId(), sectionId, ct);
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
        public async Task<IActionResult> ExercisesBySection([FromQuery] Guid sectionId, [FromQuery] string? searchTerm = null, [FromQuery] DifficultyLevel? difficulty = null,
                                                           CancellationToken ct = default)
        {

            var result = await exercise.ExercisesBySection(GetCurrentUserId(), sectionId, searchTerm, difficulty, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("create-section")]
        [Authorize]
        [ProducesResponseType(typeof(SectionGroupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a new section group within an exercise section.")]
        [EndpointDescription("Allows the authenticated user to create a new section group within a specified exercise section.")]
        [EndpointName("CreateSectionGroup")]
        public async Task<IActionResult> CreateSectionGroup([FromBody] SectionGroupReq req, CancellationToken ct = default)

        {
            var result = await exercise.CreateSectionGroup(GetCurrentUserId(), req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("all-section-groups")]
        [Authorize]
        [ProducesResponseType(typeof(List<SectionGroupResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all section groups within a specific exercise section.")]
        [EndpointDescription("Allows the authenticated user to retrieve all section groups within a specified exercise section.")]
        [EndpointName("GetAllSectionGroups")]
        public async Task<IActionResult> AllSectionGroups([FromQuery] Guid sectionId, CancellationToken ct = default)
        {
            var result = await exercise.AllSectionGroups(GetCurrentUserId(), sectionId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("add-exercise-to-section-group")]
        [Authorize]
        [ProducesResponseType(typeof(SectionGroupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Adds an exercise to a section group.")]
        [EndpointDescription("Allows the authenticated user to add an exercise or custom exercise to a specified section group.")]
        [EndpointName("AddExerciseToSectionGroup")]
        public async Task<IActionResult> AddExerciseToSectionGroup([FromQuery] Guid groupId,
                                                                   [FromQuery] Guid? exerciseId = null,
                                                                   [FromQuery] Guid? customExerciseId = null,
                                                                   CancellationToken ct = default)
        {
            var result = await exercise.AddExerciseToGroup(GetCurrentUserId(), groupId, exerciseId, customExerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPut("update-group")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates an existing exercises group.")]
        [EndpointDescription("Allows the authenticated user to update an existing exercise group by its ID.")]
        [EndpointName("UpdateSectionGroup")]
        public async Task<IActionResult> UpdateGroup([FromQuery] Guid sectionId, string name, string descripion, CancellationToken ct = default)
        {
            var result = await exercise.UpdateGroup(GetCurrentUserId(), sectionId, name, descripion, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("delete-group")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Deletes an existing exercise group.")]
        [EndpointDescription("Allows the authenticated user to delete an existing exercise group by its ID.")]
        [EndpointName("DeleteSectionGroup")]
        public async Task<IActionResult> DeleteGroup([FromQuery] Guid sectionId, CancellationToken ct = default)
        {
            var result = await exercise.DeleteGroup(GetCurrentUserId(), sectionId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("delete-exercise-section-group")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Deletes an existing exercise from section group.")]
        [EndpointDescription("Allows the authenticated user to delete an existing exercise section group by its ID.")]
        [EndpointName("DeleteExerciseFromSectionGroup")]
        public async Task<IActionResult> DeleteExerciseFromSectionGroup([FromQuery] Guid groupId,
                                                                        [FromQuery] Guid? exerciseId = null,
                                                                        [FromQuery] Guid? customExerciseId = null,
                                                                        CancellationToken ct = default)
        {
            var result = await exercise.DeleteExerciseFromGroup(GetCurrentUserId(), groupId, exerciseId, customExerciseId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }





        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
