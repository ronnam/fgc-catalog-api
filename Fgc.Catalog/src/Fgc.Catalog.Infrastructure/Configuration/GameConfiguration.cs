using Fgc.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fgc.Catalog.Infrastructure.Configurations;

public sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Category)
            .IsRequired()
            .HasMaxLength(100);
    }
}