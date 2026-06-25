using Microsoft.EntityFrameworkCore;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
