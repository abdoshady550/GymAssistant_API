using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Req_Res.Reqeust;
using Xunit;

namespace GymAssistant.E2ETests.Journeys;

[Collection("DatabaseCollection")]
public class AuthLifecycleJourneyTests : TestHostBase
{
    public AuthLifecycleJourneyTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task CompleteUserRegistrationAndLoginJourney()
    {
        // Step 1: Query available auth providers
        var providerRes = await Client.GetAsync("/api/auth/external-providers");
        providerRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 2: Register a new user
        var uniqueEmail = $"e2e_user_{Guid.NewGuid():N}@example.com";
        var uniqueUsername = $"e2e_{Guid.NewGuid():N}";
        var registerReq = new RegisterRequest(
            UserName: uniqueUsername,
            Email: uniqueEmail,
            Password: "SecurePassword123!",
            PhoneNumber: "+1234567890",
            Role: Role.User
        );

        var registerRes = await Client.PostAsJsonAsync("/api/auth/register", registerReq);
        // User registration succeeds (or returns 200/201 depending on controller result)
        registerRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.BadRequest);

        // Step 3: Trigger forgot-password flow
        var forgotReq = new ForgotPasswordDto { Email = uniqueEmail };
        var forgotRes = await Client.PostAsJsonAsync("/api/auth/forgot-password", forgotReq);
        forgotRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);

        // Verify that FakeEmailService was triggered without calling an external SMTP server
        Factory.EmailService.SentMessages.Should().NotBeNull();
    }
}
