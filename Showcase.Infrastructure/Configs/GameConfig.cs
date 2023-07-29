using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure.Configs
{
    internal class GameConfig : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("T_Games");
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.OwnsOneMultilingualString(e => e.Title);
            builder.HasIndex(e => new { e.CompanyId, e.IsDeleted });
            builder.Property(x => x.Id).HasConversion<GameId.EfValueConverter>();
        }
    }
}