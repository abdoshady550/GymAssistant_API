using GymAssistant_API.Data;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant.TestCommon.Fixtures;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString("N"))
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
