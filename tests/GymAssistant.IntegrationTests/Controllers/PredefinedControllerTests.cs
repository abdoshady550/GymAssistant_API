using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class PredefinedControllerTests : TestHostBase
{
    public PredefinedControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetSections_ReturnsOkResult()
    {
        await AuthenticateAsAsync();
        var response = await Client.GetAsync("/api/predefined/sections");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetExercises_ReturnsOkResult()
    {
        await AuthenticateAsAsync();
        var response = await Client.GetAsync("/api/predefined/exercises");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
