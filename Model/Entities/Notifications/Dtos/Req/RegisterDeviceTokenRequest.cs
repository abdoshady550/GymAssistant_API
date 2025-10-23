using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to register device token
    /// </summary>
    public record RegisterDeviceTokenRequest(
        [Required] string Token,
        [Required] DevicePlatform Platform
    );
}
