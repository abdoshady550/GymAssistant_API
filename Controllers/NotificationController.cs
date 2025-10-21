using Asp.Versioning;
using GymAssistant_API.Handeler.Notifications;
using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Req;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    [Authorize]
    public class NotificationController(NotificationsHandler notification) : ApiController
    {
        private readonly NotificationsHandler _notification = notification;

        [HttpPost("register-device")]
        [Authorize]
        [ProducesResponseType(typeof(DeviceTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Register Device")]
        [EndpointDescription("Register device token for push notifications.")]
        [EndpointName("RegisterDevice")]
        public async Task<ActionResult> RegisterDevice(string token, DevicePlatform platform, CancellationToken ct = default)
        {
            var result = await _notification.RegisterDevice(GetCurrentUserId(), token, platform, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpPost("unregister-device")]
        [Authorize]
        [ProducesResponseType(typeof(Updated), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Unregister Device")]
        [EndpointDescription("Unregister device token from push notifications.")]
        [EndpointName("UnregisterDevice")]
        public async Task<ActionResult> UnregisterDevice([FromBody] string token, CancellationToken ct = default)
        {
            var result = await _notification.UnregisterDevice(GetCurrentUserId(), token, ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }
        [HttpGet("my-notifications")]
        [Authorize]
        [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get My Notifications")]
        [EndpointDescription("Retrieve notifications for the current user.")]
        [EndpointName("GetMyNotifications")]
        public async Task<ActionResult> GetMyNotifications(
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1,
            [FromQuery] bool unreadOnly = false,
            CancellationToken ct = default)
        {
            var result = await _notification.GetMyNotifications(GetCurrentUserId(), pageSize, pageNumber, unreadOnly, ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpGet("unread-count")]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Unread Notification Count")]
        [EndpointDescription("Retrieve the count of unread notifications for the current user.")]
        [EndpointName("GetUnreadNotificationCount")]
        public async Task<ActionResult> GetUnreadNotificationCount(CancellationToken ct = default)
        {
            var result = await _notification.GetUnreadCount(GetCurrentUserId(), ct);
            return result.Match(
            response => Ok(response),
            Problem);
        }
        [HttpPut("{notificationId}/mark-read")]
        [Authorize]
        [ProducesResponseType(typeof(Updated), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Mark Notification as Read")]
        [EndpointDescription("Mark a specific notification as read for the current user.")]
        [EndpointName("MarkNotificationAsRead")]
        public async Task<ActionResult> MarkNotificationAsRead([FromRoute] Guid notificationId, CancellationToken ct = default)
        {
            var result = await _notification.MarkAsRead(GetCurrentUserId(), notificationId, ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }
        [HttpPut("mark-all-read")]
        [Authorize]
        [ProducesResponseType(typeof(Updated), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Mark All Notifications as Read")]
        [EndpointDescription("Mark all notifications as read for the current user.")]
        [EndpointName("MarkAllNotificationsAsRead")]
        public async Task<ActionResult> MarkAllNotificationsAsRead(CancellationToken ct = default)
        {
            var result = await _notification.MarkAllAsRead(GetCurrentUserId(), ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }
        [HttpDelete("{notificationId}")]
        [Authorize]
        [ProducesResponseType(typeof(Deleted), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Delete Notification")]
        [EndpointDescription("Delete a specific notification for the current user.")]
        [EndpointName("DeleteNotification")]
        public async Task<ActionResult> DeleteNotification([FromRoute] Guid notificationId, CancellationToken ct = default)
        {
            var result = await _notification.DeleteNotification(GetCurrentUserId(), notificationId, ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }
        [HttpPost("send")]
        [Authorize(Roles = "Admin,Trainer")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PushNotificationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Send Notification")]
        [EndpointDescription("Send a notification to a specific user.")]
        [EndpointName("SendNotification")]
        public async Task<ActionResult> SendNotification([FromForm] SendPushNotificationRequest request,
CancellationToken ct = default)
        {
            var result = await _notification.SendNotification(
                request.UserId,
                request.Title,
                request.Body,
                request.Type,
                request.Data,
                request.Image,
                ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }
        [HttpPost("send-bulk")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PushNotificationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Send Bulk Notifications")]
        [EndpointDescription("Send notifications to multiple users.")]
        [EndpointName("SendBulkNotifications")]
        public async Task<ActionResult> SendBulkNotifications([FromForm] SendBulkNotificationRequest request,
                                                              CancellationToken ct = default)
        {
            var result = await _notification.SendBulkNotification(
                request.UserIds,
                request.Title,
                request.Body,
                request.Type,
                request.Data,
                request.Image,
                ct);
            return result.Match(
            _ => Ok(),
            Problem);
        }



        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
