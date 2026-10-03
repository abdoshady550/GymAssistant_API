using FluentAssertions;
using GymAssistant_API.Handeler.Identity;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Identity;

public class RegisterHandlerTests
{
    private readonly Mock<ILogger<GymAssistant_API.Data.ApplicationDbContextInitialiser>> _loggerMock = new();
    private readonly Mock<IUserCreate> _userCreateMock = new();
    private readonly RegisterHandler _sut;

    public RegisterHandlerTests()
    {
        _sut = new RegisterHandler(_loggerMock.Object, _userCreateMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserCreationSucceeds_ReturnsAppUserDto()
    {
        // Arrange
        var request = new RegisterRequest(
            UserName: "testuser",
            Email: "test@example.com",
            Password: "Password123!",
            PhoneNumber: "+1234567890",
            Role: Role.User
        );

        var expectedDto = new AppUserDto(
            UserId: Guid.NewGuid().ToString(),
            Email: "test@example.com",
            Roles: new List<string> { "User" }
        );

        _userCreateMock
            .Setup(u => u.AddUserAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_ReturnsErrorsAndLogs()
    {
        // Arrange
        var request = new RegisterRequest(
            UserName: "existinguser",
            Email: "exist@example.com",
            Password: "Password123!",
            PhoneNumber: "+1234567890",
            Role: Role.User
        );

        var error = Error.Conflict("User.DuplicateEmail", "Email already exists");
        _userCreateMock
            .Setup(u => u.AddUserAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "User.DuplicateEmail");
    }
}
