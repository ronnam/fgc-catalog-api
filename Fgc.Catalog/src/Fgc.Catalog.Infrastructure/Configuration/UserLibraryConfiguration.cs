using Fgc.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fgc.Catalog.Infrastructure.Configuration;

public sealed class UserLibraryConfiguration : IEntityTypeConfiguration<UserLibrary>
{
    public void Configure(EntityTypeBuilder<UserLibrary> builder)
    {
        builder.ToTable("UserLibraries");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserId)
            .IsRequired();
        builder.Property(u => u.GameId)
            .IsRequired();
        builder.Property(u => u.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(u => u.PurchasedAt)
            .IsRequired();
    }
}