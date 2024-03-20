using MusicDomain.Entity;
using MusicDomain.Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain
{
    public interface IMusicRepository
    {
        public Task<Artist?> GetArtistByIdAsync(Guid ArtistId);
        public Task<Artist[]> GetArtistByNameAsync(string Name);
        public Task<Artist[]> GetArtistsAsync();
        public Task<Artist?> ArtistExist(string Name);

        public Task<Album?> GetAlbumByIdAsync(Guid AlbumId);
        public Task<Album[]> GetAlbumByNameAsync(string Name);
        public Task<Album[]> GetAlbumsByArtistIdAsync(Guid ArtistId);
        public Task<Album[]> GetAlbumsAsync();
        public Task<Album[]> GetAlbumsByMusicPostOrder();
        public Task<Album?> AlbumExist(string Title, string Artist, int PublishTime);

        public Task<Music?> GetMusicByIdAsync(Guid MusicId);
        public Task<Music[]> GetMusicsAsync();
        public Task<Music[]> GetMusicsByNameAsync(string Name);
        public Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId);
        public Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId);
        public Task<Music?> MusicExist(string Title, double Duration, string Artist, string Album);

        public Task<MusicDTO[]> GetMusicsByPlayListIdAsync(Guid PlayListId);
        public Task<PlayListDTO[]> GetPlayListAsync(Guid UserId);
        public Task<PlayListDTO?> GetPlayListByIdAsync(Guid UserId, Guid PlayListId);
        public Task<PlayList> AddPlayListAsync(
            Guid UserId,
            Uri? PicUrl,
            string Title,
            string Description
        );
        public Task<Music[]> AddMusicToPlayListAsync(Guid PlayListId, Music[] musics);
        public Task<bool> RemovePlayListAsync(Guid userId, Guid PlayListId);
        public Task<bool> RemoveMusicFromPlayListAsync(Guid PlayListId, Guid MusicId);

        public Task<SearchedResult> GetByKeyWordsAsync(string keyWords);
    }
}
