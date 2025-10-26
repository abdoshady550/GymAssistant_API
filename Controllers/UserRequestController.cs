using Asp.Versioning;
using GymAssistant_API.Handeler.Identity.Trainer;
using GymAssistant_API.Model.Entities.User;
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
    public class UserRequestController(UserRequestHandler userRequest) : ApiController
    {
        private readonly UserRequestHandler _userRequest = userRequest;

        [HttpPost("send")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Trainee sends a request to a trainer")]
        [EndpointDescription("Allows a trainee to send a request to a trainer with an optional message.")]
        [EndpointName("SendRequestToTrainer")]
        public async Task<IActionResult> SendRequestToTrainer([FromQuery] string TrainerId,
                                                              [FromBody] SendRequestDto req,
                                                              CancellationToken ct = default)
        {
            var result = await _userRequest.SendRequest(GetCurrentUserId(), TrainerId, req, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("sent")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("User gets all sent requests")]
        [EndpointDescription("Retrieves all requests sent by the authenticated User.")]
        [EndpointName("GetSentRequestsToTrainer")]
        public async Task<ActionResult> GetSentRequestsFromTrainer(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _userRequest.GetSentRequests(GetCurrentUserId(), pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpDelete("cancel/{requestId}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(Deleted), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("User cancels a sent request")]
        [EndpointDescription("Allows a User to cancel a previously sent request.")]
        [EndpointName("CancelRequestToTrainer")]
        public async Task<ActionResult> CancelRequestToTrainer(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _userRequest.CancelRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("received")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("User gets all received requests")]
        [EndpointDescription("Retrieves all requests received by the authenticated User.")]
        [EndpointName("GetReceivedRequestsFromTrainer")]
        public async Task<ActionResult> GetReceivedRequestsFromTrainer(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _userRequest.GetReceivedRequests(GetCurrentUserId(), pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("accept/{requestId:guid}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("User accepts a received request")]
        [EndpointDescription("Allows a User to accept a received trainee request.")]
        [EndpointName("AcceptRequestFromTrainer")]
        public async Task<ActionResult> AcceptRequestFromTrainer(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _userRequest.AcceptRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpPost("reject/{requestId:guid}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("User rejects a received request")]
        [EndpointDescription("Allows a User to reject a received trainee request.")]
        [EndpointName("RejectRequestFromTrainer")]
        public async Task<ActionResult> RejectRequestFromTrainer(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _userRequest.RejectRequest(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("{requestId:guid}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(TrainerRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get request by ID")]
        [EndpointDescription("Retrieves a specific trainee request by its ID.")]
        [EndpointName("GetRequestByIdFromTrainer")]
        public async Task<ActionResult> GetRequestToTrainerById(
            [FromRoute] Guid requestId,
            CancellationToken ct)
        {
            var result = await _userRequest.GetRequestById(GetCurrentUserId(), requestId, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }
        [HttpGet("all-trainers")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get all trainers")]
        [EndpointDescription("Retrieves a list of all trainers with optional search term and pagination.")]
        [EndpointName("GetAllTrainers")]
        public async Task<ActionResult> GetAllTrainer(
                 [FromQuery] string? searchTerm,
                 [FromQuery] int pageSize = 10,
                 [FromQuery] int pageNumber = 1,
                 CancellationToken ct = default)
        {
            var result = await _userRequest.GetAllTrainer(searchTerm, pageSize, pageNumber, ct);
            return result.Match(
               response => Ok(response),
               Problem);
        }


        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}