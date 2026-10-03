namespace GymAssistant.TestCommon.Security;

public static class ProductionProtectionGuard
{
    private static readonly string[] DangerousIndicators =
    [
        "databaseasp.net",
        "db27400",
        "fitrixapp",
        "cQ-82!kBM7_q"
    ];

    public static void AssertSafeConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Safety violation: Connection string is null or empty!");
        }

        foreach (var indicator in DangerousIndicators)
        {
            if (connectionString.Contains(indicator, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"CRITICAL SAFETY GUARD VIOLATION: The connection string contains production indicator '{indicator}'. " +
                    "Tests must never run against production!");
            }
        }
    }
}
