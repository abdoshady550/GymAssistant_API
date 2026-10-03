using FluentAssertions;
using GymAssistant_API.Handeler.Notifications;
using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Req;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Notifications;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Notifications;

public class NotificationsHandlerTests
{
    private readonly Mock<IPushNotificationService> _pushServiceMock = new();
    private readonly Mock<ILogger<NotificationsHandler>> _loggerMock = new();

    [Fact]
    public async Task RegisterDevice_Success_ReturnsDeviceTokenResponse()
    {
        // Arrange
        var handler = new NotificationsHandler(_loggerMock.Object, _pushServiceMock.Object);
        var expectedResponse = new DeviceTokenResponse(
            Id: Guid.NewGuid(),
            Token: "fcm-token-123",
            Platform: DevicePlatform.Android,
            IsActive: true,
            LastUsedUtc: DateTimeOffset.UtcNow
        );

        _pushServiceMock
            .Setup(s => s.RegisterDeviceTokenAsync("user-1", "fcm-token-123", DevicePlatform.Android, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await handler.RegisterDevice("user-1", "fcm-token-123", DevicePlatform.Android, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().Be("fcm-token-123");
    }

    [Fact]
    public async Task RegisterDevice_WhenServiceFails_ReturnsErrors()
    {
        // Arrange
        var handler = new NotificationsHandler(_loggerMock.Object, _pushServiceMock.Object);
        var error = Error.Failure("Push.RegistrationFailed", "Failed to register push token");

        _pushServiceMock
            .Setup(s => s.RegisterDeviceTokenAsync("user-1", "invalid-token", DevicePlatform.Android, It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        // Act
        var result = await handler.RegisterDevice("user-1", "invalid-token", DevicePlatform.Android, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Push.RegistrationFailed");
    }

    [Fact]
    public async Task UnregisterDevice_Success_ReturnsUpdated()
    {
        // Arrange
        var handler = new NotificationsHandler(_loggerMock.Object, _pushServiceMock.Object);

        _pushServiceMock
            .Setup(s => s.UnregisterDeviceTokenAsync("user-1", "fcm-token-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Updated);

        // Act
        var result = await handler.UnregisterDevice("user-1", "fcm-token-123", CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Updated);
    }
}
