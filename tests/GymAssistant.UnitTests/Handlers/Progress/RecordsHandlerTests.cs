using FluentAssertions;
using GymAssistant_API.Handeler.Progress;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Response.Records;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GymAssistant.UnitTests.Handlers.Progress;

public class RecordsHandlerTests
{
    private readonly Mock<ILogger<RecordsHandler>> _loggerMock = new();
    private readonly Mock<IRecordsService> _recordsServiceMock = new();
    private readonly RecordsHandler _sut;

    public RecordsHandlerTests()
    {
        _sut = new RecordsHandler(_loggerMock.Object, _recordsServiceMock.Object);
    }

    [Fact]
    public async Task GetPersonalRecords_ReturnsPersonalRecordsList()
    {
        var records = new List<PersonalRecordResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ClientProfileId = Guid.NewGuid(),
                Value = 100,
                CreatedAtUtc = DateTimeOffset.UtcNow
            }
        };

        _recordsServiceMock
            .Setup(r => r.GetPersonalRecordsAsync("user-1", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);

        var result = await _sut.GetPersonalRecords("user-1", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Value.Should().Be(100);
    }
}
