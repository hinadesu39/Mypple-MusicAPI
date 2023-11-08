using CommonHelper;
using MusicDomain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain
{
    public class MusicDomainService
    {
        private readonly IMusicRepository musicRepository;

        public MusicDomainService(IMusicRepository musicRepository)
        {
            this.musicRepository = musicRepository;
        }

        /// <summary>
        /// 添加歌手
        /// </summary>
        /// <param name="picUrl">歌手图片</param>
        /// <param name="name">歌手姓名</param>
        /// <returns></returns>
        public async Task<Artist> AddArtist(Uri? picUrl, string name)
        {
            return Artist.Create(picUrl, name);
        }

        /// <summary>
        /// 添加专辑
        /// </summary>
        /// <param name="PicUrl">专辑图片路径</param>
        /// <param name="Title">专辑名称</param>
        /// <param name="ArtistId">所属歌手</param>
        /// <param name="Type">类型</param>
        /// <param name="PublishTime">发行时间</param>
        /// <returns></returns>
        public async Task<Album> AddAlbum(
            Uri? PicUrl,
            string Title,
            string Artist,
            Guid ArtistId,
            string? Type,
            int PublishTime
        )
        {
            return Album.Create(PicUrl, Title, Artist, ArtistId, Type, PublishTime);
        }

        /// <summary>
        /// 添加歌曲
        /// </summary>
        /// <param name="AudioUrl">音频路径</param>
        /// <param name="PicUrl">歌曲图片路径</param>
        /// <param name="Title">名称</param>
        /// <param name="Duration">时长</param>
        /// <param name="Artist">歌手</param>
        /// <param name="Album">所属专辑</param>
        /// <param name="Type">类型</param>
        /// <param name="Lyric">歌词</param>
        /// <param name="PublishTime">发行时间</param>
        /// <returns></returns>
        public async Task<Music> AddMusic(
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
            return Music.Create(
                AudioUrl,
                PicUrl,
                Title,
                Duration,
                Artist,
                ArtistId,
                Album,
                AlbumId,
                Type,
                Lyric,
                PublishTime
            );
        }

        public async Task<PlayList> AddPlayList()
        {

        }

        public async Task<Music> AddMusicToPlayList()
        {

        }
    }
}
