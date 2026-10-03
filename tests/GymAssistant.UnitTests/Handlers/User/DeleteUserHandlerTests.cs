using FluentAssertions;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.User;

public class DeleteUserHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ILogger<DeleteUserHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_Success_ReturnsDeleted()
    {
        // Arrange
        var handler = new DeleteUserHandler(_loggerMock.Object, _identityServiceMock.Object);

        _identityServiceMock
            .Setup(s => s.DeleteUserAccountAsync("user-to-delete", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Deleted);

        // Act
        var result = await handler.Handle("user-to-delete", CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsError()
    {
        // Arrange
        var handler = new DeleteUserHandler(_loggerMock.Object, _identityServiceMock.Object);

        _identityServiceMock
            .Setup(s => s.DeleteUserAccountAsync("missing-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("User.NotFound", "User does not exist"));

        // Act
        var result = await handler.Handle("missing-user", CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("User.NotFound");
    }
}
