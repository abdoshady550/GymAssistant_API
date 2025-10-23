using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to subscribe/unsubscribe from a topic
    /// </summary>
    public record SubscribeRequest(
        [Required(ErrorMessage = "Device token is required")]
        [MinLength(10, ErrorMessage = "Device token must be at least 10 characters")]
        string DeviceToken,

        [MaxLength(50, ErrorMessage = "Topic name cannot exceed 50 characters")]
        string? Topic = "all"
    );
    /// <summary>
    /// Request to send a notification to a topic
    /// </summary>
    public record SendNotificationToTopicRequest(
        [Required(ErrorMessage = "Topic is required")]
        [MaxLength(50, ErrorMessage = "Topic name cannot exceed 50 characters")]
        string Topic,

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        string Title,

        [Required(ErrorMessage = "Body is required")]
        [MaxLength(500, ErrorMessage = "Body cannot exceed 500 characters")]
        string Body,

        Dictionary<string, string>? Data = null,
        IFormFile? Image = null
    );
}
