using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity
{
    public class Music
    {
        /// <summary>
        /// 歌曲id
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// 歌曲url
        /// </summary>
        public Uri AudioUrl { get; private set; }

        /// <summary>
        /// 歌曲图片
        /// </summary>
        public Uri? PicUrl { get; private set; }

        /// <summary>
        /// 歌曲名
        /// </summary>

        public string Title { get; private set; }

        /// <summary>
        /// 歌曲时长
        /// </summary>

        public double Duration { get; private set; }

        /// <summary>
        /// 歌手
        /// </summary>
        public Guid ArtistId { get; private set; }
        public string Artist { get; private set; }

        /// <summary>
        ///专辑Id，因为Episode和Album都是聚合根，因此不能直接做对象引用。
        /// </summary>
        public Guid AlbumId { get; private set; }
        public string Album { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>

        public string? Type { get; private set; }

        /// <summary>
        /// 歌词
        /// </summary>

        public string? Lyric { get; private set; }

        /// <summary>
        /// 是否喜欢
        /// </summary>

        public bool IsLiked { get; private set; }

        /// <summary>
        /// 发行时间
        /// </summary>

        public int PublishTime { get; private set; }

        /// <summary>
        /// 播放次数
        /// </summary>

        public int PlayTimes { get; private set; }

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

        public static Music Create(
            Uri AudioUrl,
            Uri? PicUrl,
            string Title,
            double Duration,
            string Artist,
            Guid ArtistId,
            string Album,
            Guid AlbumId,
            string? Type,
            string? Lyric,
            int PublishTime
        )
        {
            return new Music()
            {
                AudioUrl = AudioUrl,
                PicUrl = PicUrl,
                Title = Title,
                Duration = Duration,
                Artist = Artist,
                ArtistId = ArtistId,
                Album = Album,
                AlbumId = AlbumId,
                Type = Type,
                Lyric = Lyric,
                PublishTime = PublishTime
            };
        }
    }
}
