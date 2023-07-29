using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Showcase.Domain.Entities;
using System.Text.Json;

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
            builder.Property(x => x.CompanyId).HasConversion<CompanyId.EfValueConverter>();
            builder.Property(x => x.Id).HasConversion<GameId.EfValueConverter>();

            builder.Property(x => x.TagIds).HasConversion(v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                  v => JsonSerializer.Deserialize<List<TagId>>(v, (JsonSerializerOptions)null),
        new ValueComparer<ICollection<TagId>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
        }
    }
}