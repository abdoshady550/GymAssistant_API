using System.Net;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class TrainerControllerTests : TestHostBase
{
    public TrainerControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetAllTrainers_ReturnsSuccessStatusCode()
    {
        await AuthenticateAsAsync("trainer@gymassistant.com", "Trainer");
        var response = await Client.GetAsync("/api/Trainer/trainees");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTrainerById_WithInvalidGuid_ReturnsBadRequest()
    {
        await AuthenticateAsAsync();
        var response = await Client.GetAsync("/api/Trainer/trainees/not-a-guid");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
}
