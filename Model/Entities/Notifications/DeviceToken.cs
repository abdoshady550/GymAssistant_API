using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Notifications
{
    /// <summary>
    /// Device Token Entity - stores user device tokens for push notifications
    /// </summary>
    public sealed class DeviceToken : Entity
    {
        public string UserId { get; private set; }
        public string Token { get; private set; }
        public DevicePlatform Platform { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTimeOffset LastUsedUtc { get; private set; }
        public AppUser User { get; set; } = default!;

        private DeviceToken() { }

        private DeviceToken(Guid id, string userId, string token, DevicePlatform platform) : base(id)
        {
            UserId = userId;
            Token = token;
            Platform = platform;
            LastUsedUtc = DateTimeOffset.UtcNow;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<DeviceToken> Create(Guid id, string userId, string token, DevicePlatform platform)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Error.Validation("UserId_Required", "User ID is required");

            if (string.IsNullOrWhiteSpace(token))
                return Error.Validation("Token_Required", "Device token is required");

            return new DeviceToken(id, userId, token, platform);
        }

        public void UpdateLastUsed() => LastUsedUtc = DateTimeOffset.UtcNow;
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
    public enum DevicePlatform
    {
        Android = 1,
        iOS = 2,
        Web = 3
    }
}
