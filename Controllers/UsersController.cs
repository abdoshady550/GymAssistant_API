using Asp.Versioning;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Model.Entities.User.Dto;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust;
using GymAssistant_API.Req_Res.Reqeust.User;
using GymAssistant_API.Req_Res.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    public sealed class UsersController(CreateProfileHandler createProfile,
                                        UpdateProfileHandler updateProfile,
                                        GetProfileHandler getProfile,
                                        GetMeasurementHandler getMeasurement) : ApiController
    {
        private readonly CreateProfileHandler _createProfile = createProfile;
        private readonly UpdateProfileHandler _updateProfile = updateProfile;
        private readonly GetProfileHandler _getProfile = getProfile;
        private readonly GetMeasurementHandler _getMeasurement = getMeasurement;

        [HttpPost("create-profile")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a user profile with optional body measurements.")]
        [EndpointDescription("Creates or updates the user's profile information.")]
        [EndpointName("CreateUserProfile")]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileRequest request,
                                                       [FromForm] MeasurementRequest measurementRequest,
                                                       CancellationToken ct = default)
        {

            var result = await _createProfile.Handle(GetCurrentUserId(), request, measurementRequest, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("update-profile")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates a user profile with optional body measurements.")]
        [EndpointDescription("Creates or updates the user's profile information.")]
        [EndpointName("UpdateUserProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request,
                                                       CancellationToken ct = default)
        {
            var result = await _updateProfile.Handle(GetCurrentUserId(), request, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("get-profile")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the user's profile information.")]
        [EndpointDescription("Fetches the profile details of the authenticated user.")]
        [EndpointName("GetUserProfile")]
        public async Task<IActionResult> GetProfile([FromQuery] int pageSize, [FromQuery] int page, CancellationToken ct = default)
        {
            var result = await _getProfile.Handle(GetCurrentUserId(), pageSize, page, ct);
            return result.Match(
                response => Ok(response),
                Problem);

        }
        [HttpGet("get-measurements")]
        [Authorize]
        [ProducesResponseType(typeof(BodyMeasurementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the user's body measurement history.")]
        [EndpointDescription("Fetches the body measurement history of the authenticated user.")]
        [EndpointName("GetUserMeasurements")]
        public async Task<IActionResult> GetMeasurements([FromQuery] int pageSize, [FromQuery] int page, CancellationToken ct = default)
        {
            var result = await _getMeasurement.GetMeasurementHistoryAsync(GetCurrentUserId(), pageSize, page, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("get-measurement-charts")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the user's body measurement charts.")]
        [EndpointDescription("Fetches the body measurement charts of the authenticated user over a specified number of days.")]
        [EndpointName("GetUserMeasurementCharts")]
        public async Task<IActionResult> GetMeasurementCharts([FromQuery] int days, CancellationToken ct = default)
        {
            var result = await _getMeasurement.GetMeasurementChartsAsync(GetCurrentUserId(), days, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("update-measurement")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates a specific body measurement entry.")]
        [EndpointDescription("Updates a specific body measurement entry for the authenticated user.")]
        [EndpointName("UpdateUserMeasurement")]
        public async Task<IActionResult> UpdateMeasurement([FromQuery] Guid Id,
                                                           [FromBody] UpdateMeasurementRequest request,
                                                           CancellationToken ct = default)
        {
            var result = await _getMeasurement.UpdateMeasurementAsync(Id, request.WeightKg, request.BodyFatPercent, request.MuscleMassKg, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpDelete("delete-measurement")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Deletes a specific body measurement entry.")]
        [EndpointDescription("Deletes a specific body measurement entry for the authenticated user.")]
        [EndpointName("DeleteUserMeasurement")]
        public async Task<IActionResult> DeleteMeasurement([FromQuery] Guid Id, CancellationToken ct = default)
        {
            var result = await _getMeasurement.DeleteMeasurementAsync(Id, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }



        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
