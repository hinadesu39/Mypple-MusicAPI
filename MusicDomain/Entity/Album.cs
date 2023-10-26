using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity
{
    public class Album
    {
        /// <summary>
        /// 专辑id
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// 专辑图片
        /// </summary>
        public Uri? PicUrl { get; private set; }

        /// <summary>
        /// 专辑名
        /// </summary>

        public string Title { get; private set; }

        /// <summary>
        /// 歌手Id
        /// </summary>

        public Guid ArtistId { get; private set; }

        public string Artist { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>

        public string? Type { get; private set; }

        /// <summary>
        /// 发行时间
        /// </summary>

        public int PublishTime { get; private set; }

        /// <summary>
        /// 通用属性
        /// </summary>
        public bool IsDeleted { get; private set; }
        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public DateTime? DeletionTime { get; private set; }

        public void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }

        public static Album Create(
            Uri? PicUrl,
            string Title,
            string Artist,
            Guid ArtistId,
            string? Type,
            int PublishTime
        )
        {
            return new Album()
            {
                PicUrl = PicUrl,
                Title = Title,
                Artist = Artist,
                ArtistId = ArtistId,
                Type = Type,
                PublishTime = PublishTime
            };
        }
    }
}
