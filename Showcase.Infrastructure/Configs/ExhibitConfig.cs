using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Showcase.Domain.Entities;

namespace Showcase.Infrastructure.Configs
{
    internal class ExhibitConfig : IEntityTypeConfiguration<Exhibit>
    {
        public void Configure(EntityTypeBuilder<Exhibit> builder)
        {
            builder.ToTable("T_Exhibits");
            builder.HasKey(e => e.Id).IsClustered(false);//Guid类型不要聚集索引，否则会影响性能
            builder.HasIndex(e => new { e.GameId, e.IsDeleted });
            builder.Property(e => e.ItemUrl).HasMaxLength(1000).IsUnicode().IsRequired();
        }
    }
}
