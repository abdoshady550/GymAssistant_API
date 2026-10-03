using DotNet.Testcontainers.Builders;
using GymAssistant.TestCommon.Security;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace GymAssistant.TestCommon.Fixtures;

public class SqlDatabaseFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;
    private string? _connectionString;
    private bool _useDocker = false;

    public string ConnectionString => _connectionString ?? throw new InvalidOperationException("Database fixture not initialized.");
    public bool IsDockerRunning => _useDocker;

    public async Task InitializeAsync()
    {
        // 1. Check if user launched Docker DB via docker-compose.test.yml (port 14333) or environment variable
        var envConn = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION");
        if (!string.IsNullOrWhiteSpace(envConn))
        {
            ProductionProtectionGuard.AssertSafeConnectionString(envConn);
            _connectionString = envConn;
            return;
        }

        var masterConn = "Server=localhost,14333;Database=master;User Id=sa;Password=Test_SqlServer_Pass123!;TrustServerCertificate=True;MultipleActiveResultSets=True;";
        if (await CanConnectAsync(masterConn))
        {
            var composeConn = "Server=localhost,14333;Database=GymAssistant_Test_Db;User Id=sa;Password=Test_SqlServer_Pass123!;TrustServerCertificate=True;MultipleActiveResultSets=True;";
            ProductionProtectionGuard.AssertSafeConnectionString(composeConn);
            await EnsureDatabaseExistsAsync(masterConn, "GymAssistant_Test_Db");
            _connectionString = composeConn;
            _useDocker = true;
            return;
        }

        // 2. Try starting Docker container dynamically via Testcontainers
        try
        {
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Strong_Test_Pwd123!")
                .Build();

            await _container.StartAsync();
            _connectionString = _container.GetConnectionString();
            _useDocker = true;
        }
        catch (Exception)
        {
            _container = null;
            _useDocker = false;
            // 3. Fallback for isolated local test database if Docker Desktop daemon isn't running
            var uniqueDbName = $"GymAssistant_Test_{Guid.NewGuid():N}";
            var masterFallback = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            await EnsureDatabaseExistsAsync(masterFallback, uniqueDbName);
            _connectionString = $"Server=(localdb)\\mssqllocaldb;Database={uniqueDbName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        }

        ProductionProtectionGuard.AssertSafeConnectionString(_connectionString);
    }

    private static async Task EnsureDatabaseExistsAsync(string masterConnectionString, string dbName)
    {
        using var conn = new SqlConnection(masterConnectionString);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}') CREATE DATABASE [{dbName}];";
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<bool> CanConnectAsync(string connectionString)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await conn.OpenAsync(cts.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
        else if (!_useDocker && _connectionString != null && _connectionString.Contains("GymAssistant_Test_"))
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = builder.InitialCatalog;
                builder.InitialCatalog = "master";
                using var conn = new SqlConnection(builder.ConnectionString);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"IF EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}') BEGIN ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]; END";
                await cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Best effort cleanup
            }
        }
    }
}
