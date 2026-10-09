namespace MiniMES.Infrastructure.Persistence.Seed;

public sealed class AdminSeedOptions
{
    public const string SectionName = "DataSeed:Admin";

    public bool Enabled { get; set; }
    public string UserName { get; set; } = "admin";
    public string? Password { get; set; }
}

public sealed record AdminSeedResult(
    string UserName,
    bool Created,
    string? GeneratedPassword);
