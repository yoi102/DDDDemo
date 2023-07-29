using Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Showcase.Domain.Entities;
using System.Reflection.Emit;

namespace Showcase.Infrastructure.Configs
{
    internal class GameTagConfig : IEntityTypeConfiguration<GameTag>
    {
        public void Configure(EntityTypeBuilder<GameTag> builder)
        {
            builder.ToTable("T_Games_Tags");
            builder.HasKey(x => new { x.GameId, x.TagId });
            builder.Property(x => x.GameId).HasConversion<GameId.EfValueConverter>();
            builder.Property(x => x.TagId).HasConversion<TagId.EfValueConverter>();
        }
    }
}