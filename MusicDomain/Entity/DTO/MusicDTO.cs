using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity.DTO
{
    public class MusicDTO
    {
        /// <summary>
        /// 歌曲id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 歌曲url
        /// </summary>
        public Uri AudioUrl { get; set; }

        /// <summary>
        /// 歌曲图片
        /// </summary>
        public Uri? PicUrl { get; set; }

        /// <summary>
        /// 歌曲名
        /// </summary>

        public string Title { get; set; }

        /// <summary>
        /// 歌曲时长
        /// </summary>

        public double Duration { get; set; }

        /// <summary>
        /// 歌手
        /// </summary>
        public Guid ArtistId { get; set; }
        public string Artist { get; set; }

        /// <summary>
        ///专辑Id
        /// </summary>
        public Guid AlbumId { get; set; }
        public string Album { get; set; }

        /// <summary>
        /// 类型
        /// </summary>

        public string? Type { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>

        public string? Lyric { get; set; }

        /// <summary>
        /// 是否喜欢
        /// </summary>

        public bool IsLiked { get; set; }

        /// <summary>
        /// 发行时间
        /// </summary>

        public int PublishTime { get; set; }

        /// <summary>
        /// 播放次数
        /// </summary>

        public int PlayTimes { get; set; }

        /// <summary>
        /// 通用属性
        /// </summary>
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }

        public void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }
    }
}
