using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.E2ETests.Journeys;

[Collection("DatabaseCollection")]
public class TrainerClientJourneyTests : TestHostBase
{
    public TrainerClientJourneyTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task TrainerDirectoryAndRequest_SecuredJourney()
    {
        // 1. Trainer trainees directory is secured
        var traineesRes = await Client.GetAsync("/api/Trainer/trainees");
        traineesRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 2. Client/Trainer session creation requires authorization
        var createSessionRes = await Client.PostAsJsonAsync($"/api/Trainer/trainees/{Guid.NewGuid()}/sessions", new { });
        createSessionRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
