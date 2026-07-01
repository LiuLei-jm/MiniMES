using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder
            .HasKey(x => new { x.UserId, x.RoleId });
    }
}
