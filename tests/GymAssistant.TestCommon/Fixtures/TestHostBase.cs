using GymAssistant.TestCommon.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GymAssistant.TestCommon.Fixtures;

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<SqlDatabaseFixture>
{
}

[Collection("DatabaseCollection")]
public abstract class TestHostBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected TestHostBase(SqlDatabaseFixture dbFixture)
    {
        Factory = new CustomWebApplicationFactory(dbFixture);
        Client = Factory.CreateClient();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    protected async Task AuthenticateAsAsync(string email = "client@gymassistant.com", string role = "User")
    {
        using var scope = Factory.Services.CreateScope();
        var tokenProvider = scope.ServiceProvider.GetRequiredService<GymAssistant_API.Repository.Interfaces.Identity.ITokenProvider>();
        var userId = email == "admin@gymassistant.com" ? "11111111-1111-1111-1111-111111111111"
                   : email == "trainer@gymassistant.com" ? "22222222-2222-2222-2222-222222222222"
                   : "33333333-3333-3333-3333-333333333333";
        var userDto = new GymAssistant_API.Model.Identity.Dtos.AppUserDto(userId, email, new List<string> { role });
        var tokenResult = await tokenProvider.GenerateJwtTokenAsync(userDto);
        if (tokenResult.IsSuccess)
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult.Value.AccessToken);
        }
    }

    public virtual async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
    }
}
