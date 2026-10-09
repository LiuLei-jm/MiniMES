using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(x => x.Category).HasMaxLength(30);
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.UserName).HasMaxLength(100);
        builder.Property(x => x.TargetType).HasMaxLength(100);
        builder.Property(x => x.TargetId).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.TraceId).HasMaxLength(100);
        builder.Property(x => x.Details).HasMaxLength(1000);
        builder.HasIndex(x => new { x.Category, x.OccurredAt });
        builder.HasIndex(x => x.UserId);
    }
}
