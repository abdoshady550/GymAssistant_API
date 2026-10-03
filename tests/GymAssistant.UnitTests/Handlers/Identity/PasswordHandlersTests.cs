using FluentAssertions;
using GymAssistant_API.Handeler.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Req_Res.Reqeust.User;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Identity;

public class PasswordHandlersTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ILogger<ChangePasswordHandler>> _changeLoggerMock = new();
    private readonly Mock<ILogger<ForgotPasswordHandler>> _forgotLoggerMock = new();
    private readonly Mock<ILogger<ResetPasswordHandler>> _resetLoggerMock = new();

    [Fact]
    public async Task ChangePasswordHandler_Success_ReturnsUpdatedResult()
    {
        // Arrange
        var handler = new ChangePasswordHandler(_changeLoggerMock.Object, _identityServiceMock.Object);
        var request = new ChangePasswordRequest("OldPass123!", "NewPass123!", "NewPass123!");

        _identityServiceMock
            .Setup(s => s.ChangeUserPasswordAsync("user123", request.CurrentPassword, request.NewPassword, request.ConfirmPassword))
            .ReturnsAsync(Result.Updated);

        // Act
        var result = await handler.Handle("user123", request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Updated);
    }

    [Fact]
    public async Task ChangePasswordHandler_ServiceReturnsError_PropagatesError()
    {
        // Arrange
        var handler = new ChangePasswordHandler(_changeLoggerMock.Object, _identityServiceMock.Object);
        var request = new ChangePasswordRequest("WrongOld!", "NewPass123!", "NewPass123!");
        var expectedError = Error.Validation("Auth.InvalidPassword", "Incorrect current password");

        _identityServiceMock
            .Setup(s => s.ChangeUserPasswordAsync("user123", request.CurrentPassword, request.NewPassword, request.ConfirmPassword))
            .ReturnsAsync(expectedError);

        // Act
        var result = await handler.Handle("user123", request);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(expectedError.Code);
    }

    [Fact]
    public async Task ForgotPasswordHandler_Success_ReturnsMessage()
    {
        // Arrange
        var handler = new ForgotPasswordHandler(_forgotLoggerMock.Object, _identityServiceMock.Object);
        var dto = new ForgotPasswordDto { Email = "user@example.com" };

        _identityServiceMock
            .Setup(s => s.ForgotPasswordAsync(dto.Email))
            .ReturnsAsync("Password reset email sent.");

        // Act
        var result = await handler.Handle(dto);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("Password reset email sent.");
    }

    [Fact]
    public async Task ForgotPasswordHandler_UserNotFound_ReturnsError()
    {
        // Arrange
        var handler = new ForgotPasswordHandler(_forgotLoggerMock.Object, _identityServiceMock.Object);
        var dto = new ForgotPasswordDto { Email = "missing@example.com" };

        _identityServiceMock
            .Setup(s => s.ForgotPasswordAsync(dto.Email))
            .ReturnsAsync(Error.NotFound("Auth.InvalidEmail", "Email not found"));

        // Act
        var result = await handler.Handle(dto);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Auth.InvalidEmail");
    }

    [Fact]
    public async Task ResetPasswordHandler_Success_ReturnsConfirmation()
    {
        // Arrange
        var handler = new ResetPasswordHandler(_resetLoggerMock.Object, _identityServiceMock.Object);
        var dto = new ResetPasswordDto { Email = "user@example.com", Token = "token123", NewPassword = "NewPassword123!" };

        _identityServiceMock
            .Setup(s => s.ResetPasswordAsync(dto))
            .ReturnsAsync("Password reset successfully.");

        // Act
        var result = await handler.Handle(dto);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("Password reset successfully.");
    }
}
