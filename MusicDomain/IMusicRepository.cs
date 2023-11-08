using MusicDomain.Entity;
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

        public Task<Album?> GetAlbumByIdAsync(Guid AlbumId);
        public Task<Album[]> GetAlbumByNameAsync(string Name);
        public Task<Album[]> GetAlbumsByArtistIdAsync(Guid ArtistId);
        public Task<Album[]> GetAlbumsAsync();
        public Task<Album[]> GetAlbumsByMusicPostOrder();

        public Task<Music?> GetMusicByIdAsync(Guid MusicId);
        public Task<Music[]> GetMusicsAsync();
        public Task<Music[]> GetMusicsByNameAsync(string Name);
        public Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId);
        public Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId);
        public Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId);

        public Task<PlayList[]> GetPlayListAsync();
        public Task<PlayList[]> GetPlayListByNameAsync(string Name);
    }
}
