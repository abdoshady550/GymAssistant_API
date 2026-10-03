using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

[Collection("DatabaseCollection")]
public class UsersControllerTests : TestHostBase
{
    public UsersControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Users/get-profile");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync("/api/Users/get-profile");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMeasurements_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Users/get-measurements?pageSize=10&page=1");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMeasurements_WithValidToken_ReturnsSuccessOrNotFound()
    {
        await AuthenticateAsAsync("client_user@gymassistant.com", "User");

        var response = await Client.GetAsync("/api/Users/get-measurements?pageSize=10&page=1");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
