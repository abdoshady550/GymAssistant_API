using FluentAssertions;
using GymAssistant_API.Handeler.Exercise.Workout;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Exercise;

public class WorkoutHandlerTests
{
    private readonly Mock<ILogger<WorkoutHandler>> _loggerMock = new();
    private readonly Mock<IWorkoutService> _workoutServiceMock = new();
    private readonly WorkoutHandler _sut;

    public WorkoutHandlerTests()
    {
        _sut = new WorkoutHandler(_loggerMock.Object, _workoutServiceMock.Object);
    }

    [Fact]
    public async Task CreateWorkoutSession_WhenServiceSucceeds_ReturnsWorkoutSessionRes()
    {
        var request = new CreateWorkoutSessionRequest(
            Date: DateTime.UtcNow,
            Notes: "Heavy squats",
            TraineeId: null
        );

        var expectedSession = new WorkoutSessionRes(
            Id: Guid.NewGuid(),
            ClientProfileId: Guid.NewGuid(),
            CreatedByTrainerId: null,
            Date: request.Date,
            StartTime: null,
            EndTime: null,
            IsCompleted: false,
            DurationMinutes: null,
            Notes: "Heavy squats",
            WorkoutExercises: new List<WorkoutExerciseRes>()
        );

        _workoutServiceMock
            .Setup(s => s.CreateWorkoutSessionAsync("user-123", request.Date, request.Notes, request.TraineeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSession);

        var result = await _sut.CreateWorkoutSession("user-123", request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedSession);
    }

    [Fact]
    public async Task CompleteWorkoutSession_WhenServiceFails_ReturnsFailure()
    {
        var sessionId = Guid.NewGuid();
        var endTime = DateTime.UtcNow;

        _workoutServiceMock
            .Setup(s => s.CompleteWorkoutSessionAsync("user-123", sessionId, endTime, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Workout.NotFound", "Session not found"));

        var result = await _sut.CompleteWorkoutSession("user-123", sessionId, endTime, null, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "Workout.NotFound");
    }
}
