using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class WorkoutsControllerTests : TestHostBase
{
    public WorkoutsControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetWorkoutHistory_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Workouts/get-workout-history");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSession_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.PostAsJsonAsync("/api/Workouts/create-session", new { Date = DateTime.UtcNow });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
