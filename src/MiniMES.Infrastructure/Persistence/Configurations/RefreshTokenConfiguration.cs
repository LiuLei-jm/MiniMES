using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasIndex(x => x.Token)
            .IsUnique();
        builder.Property(x => x.Token)
            .HasMaxLength(256);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}
