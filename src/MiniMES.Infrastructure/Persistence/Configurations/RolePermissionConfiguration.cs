using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => new { x.RoleId, x.PermissionId });
    }
}
