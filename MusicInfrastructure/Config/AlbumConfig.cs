using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MusicDomain.Entity;

namespace MusicInfrastructure.Config
{
    public class AlbumConfig : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.ToTable("T_Album");
            builder.HasKey(e => e.Id).IsClustered(false);//对于Guid主键，不要建聚集索引，否则插入性能很差
            //配置值类型
            //builder.OwnsOne(e => e.Title, nv =>
            //{
            //    nv.Property(c => c.Title).IsRequired().HasMaxLength(200).IsUnicode();
            //    nv.Property(c => c.TitleTrans).HasMaxLength(200).IsUnicode();
            //});
            //将为这两个属性创建一个复合索引，以提高查询性能。
            builder.HasIndex(e => new { e.ArtistId, e.IsDeleted });
        }
    }
}
