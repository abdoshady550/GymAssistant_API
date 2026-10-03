using System.Net;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

[Collection("DatabaseCollection")]
public class ProgressControllerTests : TestHostBase
{
    public ProgressControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetExerciseProgress_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync($"/api/Progress/exercise/{Guid.NewGuid()}?days=30");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetExerciseProgress_WithToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync($"/api/Progress/exercise/{Guid.NewGuid()}?days=30");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSectionProgress_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync($"/api/Progress/section/{Guid.NewGuid()}?days=30");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSectionProgress_WithToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync($"/api/Progress/section/{Guid.NewGuid()}?days=30");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }
}
