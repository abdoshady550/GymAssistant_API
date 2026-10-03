using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant_API.Req_Res.Response.Exercise;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class ExercisesControllerTests : TestHostBase
{
    public ExercisesControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetAllSections_ReturnsOkResult()
    {
        await AuthenticateAsAsync();
        var response = await Client.GetAsync("/api/Exercises/get-sections");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSectionById_WithInvalidGuid_ReturnsNotFoundOrBadRequest()
    {
        await AuthenticateAsAsync();
        var response = await Client.GetAsync("/api/Exercises/get-section-by-id?id=not-a-guid");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCustomExercise_WhenUnauthorized_ReturnsUnauthorized()
    {
        var response = await Client.PostAsync("/api/Exercises/create-custom-exercise?sectionId=" + Guid.NewGuid(), null);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
