using FCG_Games.Domain.Games.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG_Games.Infrastructure.Games.Mappings
{
    internal class GameMap : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(g => g.Id)
                .HasName("PK_Game");

            builder.Property(g => g.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Price)
                .HasColumnName("Price")
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(g => g.LaunchYear)
                .HasColumnName("LaunchYear")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(g => g.Developer)
                .HasColumnName("Developer")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Genre)
                .HasColumnName("Genre")
                .IsRequired();

            builder.Property(g => g.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(g => g.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2")
                .IsRequired(false);
        }
    }
}
