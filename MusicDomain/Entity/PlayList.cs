using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity
{
    public class PlayList
    {
        /// <summary>
        /// 播放列表id
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// 播放列表图片
        /// </summary>
        public Uri? PicUrl { get; private set; }

        /// <summary>
        /// 播放列表名
        /// </summary>

        public string Title { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>

        public string? Description { get; private set; }


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

        public static PlayList Create(
            Uri? PicUrl,
            string Title,
            string? Description
        )
        {
            return new PlayList()
            {
                PicUrl = PicUrl,
                Title = Title,
                Description = Description
            };
        }
    }
}
