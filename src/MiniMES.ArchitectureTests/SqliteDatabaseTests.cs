using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class SqliteDatabaseTests
{
    [Fact]
    public async Task Sqlite_can_create_the_application_schema_and_persist_a_user()
    {
        const string connectionString = "Data Source=file:minimes-test?mode=memory&cache=shared";
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [DatabaseProvider.ConfigurationKey] = DatabaseProvider.Sqlite,
                ["ConnectionStrings:Default"] = connectionString
            })
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        DatabaseProvider.Configure(optionsBuilder, configuration);

        await using var context = new AppDbContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();

        context.Users.Add(new User("sqlite-test", "hashed-password"));
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Users.CountAsync());
    }
}
