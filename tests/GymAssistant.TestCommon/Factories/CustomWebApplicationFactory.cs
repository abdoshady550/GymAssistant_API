using GymAssistant.TestCommon.Fakes;
using GymAssistant.TestCommon.Fixtures;
using GymAssistant.TestCommon.Security;
using GymAssistant_API.Data;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Interfaces.Notifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GymAssistant.TestCommon.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqlDatabaseFixture _dbFixture;
    private readonly string _tempWwwRoot;

    public FakeEmailService EmailService { get; } = new();
    public FakePushNotificationService PushNotificationService { get; } = new();

    public CustomWebApplicationFactory(SqlDatabaseFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _tempWwwRoot = Path.Combine(Path.GetTempPath(), "GymAssistant_Test_WwwRoot_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempWwwRoot);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbFixture.ConnectionString,
                ["JWT:Key"] = "SuperSecretTestingKeyForJwtValidation1234567890!",
                ["JWT:Issuer"] = "TestIssuer",
                ["JWT:Audience"] = "TestAudience",
                ["JWT:ExpiresInMin"] = "60"
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Assert connection string safety before registering
            ProductionProtectionGuard.AssertSafeConnectionString(_dbFixture.ConnectionString);

            // Re-register AppDbContext pointing strictly to test container/db
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_dbFixture.ConnectionString);
                options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            // Re-configure JwtBearer authentication for testing tokens
            services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var testKey = System.Text.Encoding.UTF8.GetBytes("SuperSecretTestingKeyForJwtValidation1234567890!");
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "TestIssuer",
                    ValidAudience = "TestAudience",
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(testKey)
                };
            });

            // Replace email service with fake
            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null) services.Remove(emailDescriptor);
            services.AddSingleton<IEmailService>(EmailService);

            // Replace push notification service with fake
            var pushDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPushNotificationService));
            if (pushDescriptor != null) services.Remove(pushDescriptor);
            services.AddSingleton<IPushNotificationService>(PushNotificationService);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (Directory.Exists(_tempWwwRoot))
        {
            try
            {
                Directory.Delete(_tempWwwRoot, true);
            }
            catch
            {
                // Best effort
            }
        }
    }
}
