using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Configs
{
    internal class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
    {
        public void Configure(EntityTypeBuilder<UploadedItem> builder)
        {
            builder.ToTable("T_FS_UploadedItems");
            //因为SQLServer对于Guid主键默认创建聚集索引
            //取消主键的默认的聚集索引
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.FileName).IsUnicode().HasMaxLength(1024);
            builder.Property(e => e.FileSHA256Hash).IsUnicode(false).HasMaxLength(64);
            //经常要按照这两个列进行查询，把两个组成复合索引，提高查询效率。
            builder.HasIndex(e => new { e.FileSHA256Hash, e.FileSizeInBytes });
        }
    }
}
