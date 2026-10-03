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
public class WorkoutCreationJourneyTests : TestHostBase
{
    public WorkoutCreationJourneyTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task WorkoutAndExerciseJourney_SecuredAgainstUnauthorizedFlow()
    {
        // 1. Browsing public exercise definitions
        var sectionsRes = await Client.GetAsync("/api/Exercises/get-sections");
        sectionsRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);

        // 2. Attempting to start a workout session without authorization fails safely
        var startSessionRes = await Client.PostAsJsonAsync("/api/Workouts/create-session", new
        {
            Date = DateTime.UtcNow,
            Notes = "Should fail without auth"
        });
        startSessionRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 3. Attempting to view workout history without auth fails safely
        var historyRes = await Client.GetAsync("/api/Workouts/get-workout-history");
        historyRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
