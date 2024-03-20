using CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity
{
    public class Artist
    {
        /// <summary>
        /// 歌手id
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// 歌手图片
        /// </summary>
        public Uri? PicUrl { get; set; }

        /// <summary>
        /// 歌手名
        /// </summary>

        public string Name { get; set; }

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
        public static Artist Create(Uri picUrl, string Name)
        {
            return new Artist() { PicUrl = picUrl, Name = Name };
        }

        public void Update(Uri? PicUrl, string Name)
        {
            this.PicUrl = PicUrl;
            this.Name = Name;
        }
    }
}
