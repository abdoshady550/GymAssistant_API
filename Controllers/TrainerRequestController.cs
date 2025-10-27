using Asp.Versioning;
using GymAssistant_API.Handeler.Identity.Trainer;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.User.Trainer;
using GymAssistant_API.Req_Res.Response.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public class TrainerRequestController(TrainerRequestHandler trainerRequest) : ApiController
    {
        private readonly TrainerRequestHandler _trainerRequest = trainerRequest;
        [HttpPost("send")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer sends a request to a trainee")]
        [EndpointDescription("Allows a trainer to send a request to a trainee with an optional message.")]
        [EndpointName("SendRequest")]
        public async Task<IActionResult> SendRequest(
          [FromQuery] string traineeId, [FromBody] SendRequestDto request,
            CancellationToken ct)
        {
            var result = await _trainerRequest.SendRequest(GetCurrentUserId(), traineeId, request, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("sent")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer gets all sent requests")]
        [EndpointDescription("Retrieves all requests sent by the authenticated trainer.")]
        [EndpointName("GetSentRequests")]
        public async Task<ActionResult> GetSentRequests(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _trainerRequest.GetSentRequests(GetCurrentUserId(), pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("cancel/{requestId}")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(Deleted), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer cancels a sent request")]
        [EndpointDescription("Allows a trainer to cancel a previously sent request.")]
        [EndpointName("CancelRequest")]
        public async Task<ActionResult> CancelRequest(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _trainerRequest.CancelRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("received")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer gets all received requests")]
        [EndpointDescription("Retrieves all requests received by the authenticated trainer.")]
        [EndpointName("GetReceivedRequests")]
        public async Task<ActionResult> GetReceivedRequests(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _trainerRequest.GetReceivedRequests(GetCurrentUserId(), pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("accept/{requestId:guid}")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer accepts a received request")]
        [EndpointDescription("Allows a trainer to accept a received trainee request.")]
        [EndpointName("AcceptRequest")]
        public async Task<ActionResult> AcceptRequest(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _trainerRequest.AcceptRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("reject/{requestId:guid}")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainer rejects a received request")]
        [EndpointDescription("Allows a trainer to reject a received trainee request.")]
        [EndpointName("RejectRequest")]
        public async Task<ActionResult> RejectRequest(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _trainerRequest.RejectRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("{requestId:guid}")]
        [Authorize(Roles = "Trainer")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get request by ID")]
        [EndpointDescription("Retrieves a specific trainee request by its ID.")]
        [EndpointName("GetRequestById")]
        public async Task<ActionResult> GetRequestById(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _trainerRequest.GetRequestById(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("all-users")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get all users")]
        [EndpointDescription("Retrieves a list of all users with optional search term and pagination.")]
        [EndpointName("GetAllUsers")]
        public async Task<ActionResult> GetAllUser(
            [FromQuery] string? searchTerm,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _trainerRequest.GetAllUser(GetCurrentUserId(), searchTerm, pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }


        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
