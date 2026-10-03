using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Exercise;

public class ExerciseHandlerTests
{
    private readonly Mock<ILogger<GymAssistant_API.Handeler.Exercise.ExerciseHandler>> _loggerMock = new();
    private readonly Mock<IExercise> _exerciseServiceMock = new();
    private readonly GymAssistant_API.Handeler.Exercise.ExerciseHandler _sut;

    public ExerciseHandlerTests()
    {
        _sut = new GymAssistant_API.Handeler.Exercise.ExerciseHandler(_loggerMock.Object, _exerciseServiceMock.Object);
    }

    [Fact]
    public async Task GetSections_WhenServiceSucceeds_ReturnsList()
    {
        var expectedSections = new List<SectionResponse>
        {
            new(Guid.NewGuid(), "Chest", "Desc")
        };

        _exerciseServiceMock
            .Setup(s => s.GetSectionsAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSections);

        var result = await _sut.GetSections("user-1", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedSections);
    }

    [Fact]
    public async Task GetSectionByIdAsync_WhenNotFound_ReturnsError()
    {
        var sectionId = Guid.NewGuid();
        _exerciseServiceMock
            .Setup(s => s.GetSectionByIdAsync("user-1", sectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Section.NotFound", "Section not found"));

        var result = await _sut.GetSectionByIdAsync("user-1", sectionId, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "Section.NotFound");
    }
}
