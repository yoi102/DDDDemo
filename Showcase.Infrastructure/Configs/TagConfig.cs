using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure.Configs
{
    internal class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("T_Tags");
            builder.HasIndex(x => x.Text).IsUnique();
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.Text).HasMaxLength(10).IsUnicode(false).IsRequired();
            builder.Property(x => x.Id).HasConversion<TagId.EfValueConverter>();
        }
    }
}