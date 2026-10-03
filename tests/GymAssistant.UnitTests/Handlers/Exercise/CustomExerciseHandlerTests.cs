using FluentAssertions;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Exercise;

public class CustomExerciseHandlerTests
{
    private readonly Mock<ILogger<CustomExerciseHandler>> _loggerMock = new();
    private readonly Mock<IExercise> _exerciseServiceMock = new();
    private readonly CustomExerciseHandler _sut;

    public CustomExerciseHandlerTests()
    {
        _sut = new CustomExerciseHandler(_loggerMock.Object, _exerciseServiceMock.Object);
    }

    [Fact]
    public async Task CreateCustomExercise_WhenServiceSucceeds_ReturnsResponse()
    {
        var sectionId = Guid.NewGuid();
        var req = new CustomExerciseReq("My Press", "Notes", null, null, null, null);
        var expectedRes = new CustomExerciseRes(Guid.NewGuid(), "user-1", sectionId, "Chest", "My Press", "Notes");

        _exerciseServiceMock
            .Setup(s => s.CreateCustomExerciseAsync("user-1", sectionId, req.Name, req.Description, req.Instructions, req.Equipment, req.ImageFile, req.DifficultyLevel, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRes);

        var result = await _sut.CreateCustomExercise("user-1", sectionId, req, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedRes);
    }

    [Fact]
    public async Task DeleteCustomExercise_WhenUserOwnsExercise_ReturnsDeleted()
    {
        var exId = Guid.NewGuid();
        _exerciseServiceMock
            .Setup(s => s.DeleteCustomExerciseAsync("user-1", exId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Deleted);

        var result = await _sut.DeleteCustomExercise("user-1", exId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
