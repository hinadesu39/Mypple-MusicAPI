using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MusicDomain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicInfrastructure.Config
{
    public class PlayListConfig : IEntityTypeConfiguration<PlayList>
    {
        public void Configure(EntityTypeBuilder<PlayList> builder)
        {
            builder.ToTable("T_PlayList");
            builder.HasKey(e => e.Id).IsClustered(false);//对于Guid主键，不要建聚集索引，否则插入性能很差
            //配置值类型
            //builder.OwnsOne(e => e.Title, nv =>
            //{
            //    nv.Property(c => c.Title).IsRequired().HasMaxLength(200).IsUnicode();
            //    nv.Property(c => c.TitleTrans).HasMaxLength(200).IsUnicode();
            //});
            //将为这两个属性创建一个复合索引，以提高查询性能。
            builder.HasIndex(e => new { e.Title, e.IsDeleted });
        }
    }
}
