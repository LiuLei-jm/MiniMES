namespace MiniMES.Infrastructure.Persistence;

public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MiniMES.Api"));

        IConfiguration configuration =
            new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var builder =
            new DbContextOptionsBuilder<AppDbContext>();

        builder.UseSqlServer(
            configuration.GetConnectionString("Default"));

        return new AppDbContext(builder.Options);
    }
}
