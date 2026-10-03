using FluentAssertions;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using GymAssistant_API.Req_Res.Response;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.User;

public class ProfileHandlerTests
{
    private readonly Mock<ILogger<CreateProfileHandler>> _createLoggerMock = new();
    private readonly Mock<ILogger<GetProfileHandler>> _getLoggerMock = new();
    private readonly Mock<IProfile> _profileServiceMock = new();
    private readonly CreateProfileHandler _createHandler;
    private readonly GetProfileHandler _getHandler;

    public ProfileHandlerTests()
    {
        _createHandler = new CreateProfileHandler(_createLoggerMock.Object, _profileServiceMock.Object);
        _getHandler = new GetProfileHandler(_getLoggerMock.Object, _profileServiceMock.Object);
    }

    [Fact]
    public async Task CreateProfile_WhenValid_ReturnsSuccess()
    {
        var req = new CreateProfileRequest(
            FirstName: "Ahmed",
            LastName: "Ali",
            Gender: Gender.Male
        );
        var measurementReq = new MeasurementRequest(
            WeightKg: 80,
            WeightGoal: 75
        );

        var profile = ClientProfile.CreateProfile(
            Guid.NewGuid(),
            "user-1",
            "Ahmed",
            "Ali",
            Gender.Male,
            UserRole.User
        );

        var measurement = BodyMeasurement.Create(
            Guid.NewGuid(),
            "user-1",
            80,
            75,
            15,
            12,
            60,
            65
        );

        _profileServiceMock
            .Setup(p => p.CreateProfileAsync("user-1", "Ahmed", "Ali", Gender.Male, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile.Value);

        _profileServiceMock
            .Setup(p => p.AddBodyMeasurementAsync("user-1", 80, 75, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(measurement.Value);

        var result = await _createHandler.Handle("user-1", req, measurementReq, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Ahmed");
    }

    [Fact]
    public async Task GetProfile_WhenNotFound_ReturnsNotFoundResult()
    {
        _profileServiceMock
            .Setup(p => p.GetProfileAsync("user-999", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Profile.NotFound", "Profile does not exist"));

        var result = await _getHandler.Handle("user-999", CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "Profile.NotFound");
    }
}
