using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.IntegrationTests.Infrastructure;

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<SqlDatabaseFixture>
{
}
