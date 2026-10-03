using System.Net;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

[Collection("DatabaseCollection")]
public class RecordsControllerTests : TestHostBase
{
    public RecordsControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetPersonalRecords_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Records/personal");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPersonalRecords_WithToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync("/api/Records/personal");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetExerciseRecords_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync($"/api/Records/exercise/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetExerciseRecords_WithToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync($"/api/Records/exercise/{Guid.NewGuid()}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
