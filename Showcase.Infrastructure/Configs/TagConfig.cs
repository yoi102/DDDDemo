using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Showcase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Infrastructure.Configs
{
    internal class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("T_Tags");
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.Text).HasMaxLength(10).IsUnicode(false).IsRequired();


        }
    }
}