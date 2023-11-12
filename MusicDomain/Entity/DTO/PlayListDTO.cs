using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity.DTO
{
    public class PlayListDTO
    {
        /// <summary>
        /// 播放列表id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 播放列表图片
        /// </summary>
        public Uri? PicUrl { get; set; }

        /// <summary>
        /// 播放列表名
        /// </summary>

        public string Title { get; set; }

        /// <summary>
        /// 类型
        /// </summary>

        public string? Description { get; set; }

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
