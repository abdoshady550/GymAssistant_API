using FluentAssertions;
using GymAssistant_API.Handeler.Identity.Trainer;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Req_Res.Response.Trainer;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Trainer;

public class TrainerHandlerTests
{
    private readonly Mock<ILogger<TrainerHandler>> _loggerMock = new();
    private readonly Mock<ITrainerService> _trainerServiceMock = new();
    private readonly TrainerHandler _sut;

    public TrainerHandlerTests()
    {
        _sut = new TrainerHandler(_loggerMock.Object, _trainerServiceMock.Object);
    }

    [Fact]
    public async Task GetTrainees_ReturnsAvailableTrainees()
    {
        var trainees = new List<TraineeData>
        {
            new()
            {
                TraineeId = Guid.NewGuid(),
                FirstName = "Ali",
                LastName = "Hassan",
                Gender = Gender.Male
            }
        };

        _trainerServiceMock
            .Setup(t => t.GetTraineesAsync("trainer-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(trainees);

        var result = await _sut.GetTrainees("trainer-1", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTrainee_WhenNotFound_ReturnsError()
    {
        var traineeId = Guid.NewGuid();
        _trainerServiceMock
            .Setup(t => t.GetTraineeAsync("trainer-1", traineeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Trainee.NotFound", "Trainee not found"));

        var result = await _sut.GetTrainee("trainer-1", traineeId, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == "Trainee.NotFound");
    }
}
