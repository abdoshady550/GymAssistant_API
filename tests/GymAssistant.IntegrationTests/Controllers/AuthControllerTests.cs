using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Req_Res.Reqeust;
using GymAssistant_API.Req_Res.Reqeust.User;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class AuthControllerTests : TestHostBase
{
    public AuthControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetExternalProviders_ReturnsOkWithSupportedProviders()
    {
        // Act
        var response = await Client.GetAsync("/api/auth/external-providers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var providers = await response.Content.ReadFromJsonAsync<List<string>>();
        providers.Should().NotBeNull();
        providers.Should().Contain(new[] { "Google", "Facebook" });
    }

    [Fact]
    public async Task Register_WithInvalidModel_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange: Missing email and password
        var request = new RegisterRequest(
            UserName: "",
            Email: "not-an-email",
            Password: "",
            PhoneNumber: "123",
            Role: Role.User
        );

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsNotFoundOrBadRequest()
    {
        // Arrange
        var request = new LoginRequest(
            Email: "nonexistent_user_test@example.com",
            Password: "WrongPassword123!"
        );

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }
}
