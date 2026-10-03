using System.Net;
using FluentAssertions;
using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Controllers;

public class NotificationControllerTests : TestHostBase
{
    public NotificationControllerTests(SqlDatabaseFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task GetUserNotifications_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Notification/my-notifications");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUnreadCount_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/Notification/unread-count");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
