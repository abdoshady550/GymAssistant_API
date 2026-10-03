using FluentAssertions;
using GymAssistant_API.Handeler.Progress;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Response.Progress;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Progress;

public class ProgressHandlerTests
{
    private readonly Mock<ILogger<ProgressHandler>> _loggerMock = new();
    private readonly Mock<IProgressService> _progressServiceMock = new();
    private readonly ProgressHandler _sut;

    public ProgressHandlerTests()
    {
        _sut = new ProgressHandler(_loggerMock.Object, _progressServiceMock.Object);
    }

    [Fact]
    public async Task GetProgressOverview_WhenSuccess_ReturnsOverviewData()
    {
        var expectedOverview = new ProgressOverviewData
        {
            TotalWorkouts = 10,
            TotalSets = 50,
            TotalVolume = 2500,
            AverageDuration = 45
        };

        _progressServiceMock
            .Setup(p => p.GetProgressOverviewAsync("user-1", 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOverview);

        var result = await _sut.GetProgressOverview("user-1", 7, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalWorkouts.Should().Be(10);
        result.Value.TotalVolume.Should().Be(2500);
    }

    [Fact]
    public async Task GetExerciseProgress_WhenNoData_ReturnsEmptyList()
    {
        var exerciseId = Guid.NewGuid();
        _progressServiceMock
            .Setup(p => p.GetExerciseProgressAsync("user-1", exerciseId, 30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExerciseProgressData { ExerciseId = exerciseId, ExerciseName = "Squat", Sessions = new() });

        var result = await _sut.GetExerciseProgress("user-1", exerciseId, 30, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Sessions.Should().BeEmpty();
    }
}
