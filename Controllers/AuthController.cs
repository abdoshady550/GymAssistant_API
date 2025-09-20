using Asp.Versioning;
using GymAssistant_API.Handeler.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Req_Res.Reqeust;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]

    public sealed class AuthController(GenerateTokenQueryHandler GenerateToken,
                                       RefreshTokenQueryHandler RefreshToken,
                                       GetUserByIdQueryHanlder GetUserById,
                                       RegisterHandler registerHandler,
                                       ForgotPasswordHandler forgotPassword,
                                       ResetPasswordHandler resetPassword) : ApiController
    {
        private readonly GenerateTokenQueryHandler _generateToken = GenerateToken;
        private readonly RefreshTokenQueryHandler _refreshToken = RefreshToken;
        private readonly GetUserByIdQueryHanlder _getUserById = GetUserById;
        private readonly RegisterHandler _registerHandler = registerHandler;
        private readonly ForgotPasswordHandler _forgotPassword = forgotPassword;
        private readonly ResetPasswordHandler _resetPassword = resetPassword;



        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Registers a new user account.")]
        [EndpointDescription("Creates a new user with the provided registration details and returns an access and refresh token if successful.")]
        [EndpointName("RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest,
                                         CancellationToken ct = default)
        {
            var result = await _registerHandler.Handle(registerRequest, ct);

            return result.Match(
                response => Ok(response),
                Problem);

        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Generates an access and refresh token for a valid user.")]
        [EndpointDescription("Authenticates a user using provided credentials and returns a JWT token pair.")]
        [EndpointName("GenerateToken")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest,
                                               CancellationToken ct = default)
        {
            var result = await _generateToken.Handle(loginRequest, ct);

            return result.Match(
                response => Ok(response),
                Problem);

        }
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Refreshes access token using a valid refresh token.")]
        [EndpointDescription("Exchanges an expired access token and a valid refresh token for a new token pair.")]
        [EndpointName("RefreshToken")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenQuery request, CancellationToken ct)
        {
            var result = await _refreshToken.Handle(request, ct);
            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("current-user/claims")]
        [Authorize]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Gets the current authenticated user's info.")]
        [EndpointDescription("Returns user information for the currently authenticated user based on the access token.")]
        [EndpointName("GetCurrentUserClaims")]
        public async Task<IActionResult> GetCurrentUserInfo(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _getUserById.Handle(new GetUserByIdQuery(userId), ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Sends a password reset link to the user.")]
        [EndpointDescription("Accepts an email address, verifies the user exists, and sends a password reset link if successful.")]
        [EndpointName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request,
                              CancellationToken ct = default)
        {
            var result = await _forgotPassword.Handle(request, ct);
            return result.Match(
                response => Ok(response),
                Problem);

        }
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Resets the user's password using a reset token.")]
        [EndpointDescription("Accepts a reset token, email, and new password to reset the user's password. Typically used after clicking the link sent by Forgot Password.")]
        [EndpointName("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request,
                               CancellationToken ct = default)
        {
            var result = await _resetPassword.Handle(request, ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

    }

}
