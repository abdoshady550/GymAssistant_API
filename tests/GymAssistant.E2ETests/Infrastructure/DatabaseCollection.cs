using GymAssistant.TestCommon.Fixtures;
using Xunit;

namespace GymAssistant.E2ETests.Infrastructure;

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<SqlDatabaseFixture>
{
}
