using Asp.Versioning;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust;
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
                                        GetProfileHandler getProfile) : ApiController
    {
        private readonly CreateProfileHandler _createProfile = createProfile;
        private readonly UpdateProfileHandler _updateProfile = updateProfile;
        private readonly GetProfileHandler _getProfile = getProfile;

        [HttpPost("create-profile")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a user profile with optional body measurements.")]
        [EndpointDescription("Creates or updates the user's profile information.")]
        [EndpointName("CreateUserProfile")]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileRequest request,
                                                       [FromForm] MeasurementRequest measurementRequest,
                                                       CancellationToken ct = default)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _createProfile.Handle(userId, request, measurementRequest);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpPut("update-profile")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Updates a user profile with optional body measurements.")]
        [EndpointDescription("Creates or updates the user's profile information.")]
        [EndpointName("UpdateUserProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request,
                                                       CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _updateProfile.Handle(userId, request);
            return result.Match(
                response => Ok(response),
                Problem);
        }
        [HttpGet("get-profile")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the user's profile information.")]
        [EndpointDescription("Fetches the profile details of the authenticated user.")]
        [EndpointName("GetUserProfile")]
        public async Task<IActionResult> GetProfile(CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _getProfile.Handle(userId, ct);
            return result.Match(
                response => Ok(response),
                Problem);

        }
    }
}
