namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Res
{
    /// <summary>
    /// Device token response
    /// </summary>
    public record DeviceTokenResponse(
        Guid Id,
        string Token,
        DevicePlatform Platform,
        bool IsActive,
        DateTimeOffset LastUsedUtc
    );
}
