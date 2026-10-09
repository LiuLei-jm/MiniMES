using Microsoft.EntityFrameworkCore;

namespace MiniMES.Infrastructure.Persistence;

public static class DatabaseProvider
{
    public const string ConfigurationKey = "DatabaseProvider";
    public const string SqlServer = "SqlServer";
    public const string Sqlite = "Sqlite";

    public static void Configure(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var provider = configuration[ConfigurationKey]?.Trim();
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:Default is required for the configured database provider.");
        }

        switch (provider)
        {
            case null:
            case "":
            case SqlServer:
                // SQL Server remains the production provider. Development selects SQLite in appsettings.Development.json.
                options.UseSqlServer(connectionString);
                break;

            case Sqlite:
                options.UseSqlite(connectionString);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider '{provider}'. Use '{SqlServer}' or '{Sqlite}'.");
        }
    }

    public static bool IsSqlite(IConfiguration configuration) =>
        string.Equals(configuration[ConfigurationKey]?.Trim(), Sqlite, StringComparison.OrdinalIgnoreCase);
}
