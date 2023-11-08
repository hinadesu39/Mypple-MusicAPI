using Microsoft.EntityFrameworkCore;
using MusicDomain;
using MusicDomain.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicInfrastructure
{
    public class MusicRepository : IMusicRepository
    {
        private readonly MusicDBContext dbContext;

        public MusicRepository(MusicDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Album?> GetAlbumByIdAsync(Guid AlbumId)
        {
            return await dbContext.FindAsync<Album>(AlbumId);
        }

        public async Task<Album[]> GetAlbumByNameAsync(string Name)
        {
            return await dbContext.Albums.Where(a => a.Title.Contains(Name)).ToArrayAsync();
        }

        public async Task<Album[]> GetAlbumsAsync()
        {
            return await dbContext.Albums.ToArrayAsync();
        }

        public async Task<Album[]> GetAlbumsByArtistIdAsync(Guid ArtistId)
        {
            return await dbContext.Albums.Where(a => a.ArtistId == ArtistId).ToArrayAsync();
        }

        public async Task<Artist?> GetArtistByIdAsync(Guid ArtistId)
        {
            return await dbContext.FindAsync<Artist>(ArtistId);
        }

        public async Task<Artist[]> GetArtistByNameAsync(string Name)
        {
            return await dbContext.Artists.Where(a => a.Name.Contains(Name)).ToArrayAsync();
        }

        public async Task<Artist[]> GetArtistsAsync()
        {
            return await dbContext.Artists.ToArrayAsync();
        }

        public async Task<Music[]> GetMusicsByNameAsync(string Name)
        {
            return await dbContext.Musics.Where(m => m.Title.Contains(Name)).ToArrayAsync();
        }

        public async Task<Music[]> GetMusicsAsync()
        {
            return await dbContext.Musics.ToArrayAsync();
        }

        public async Task<Music[]> GetMusicsByAlbumIdAsync(Guid AlbumId)
        {
            return await dbContext.Musics.Where(m => m.AlbumId == AlbumId).ToArrayAsync();
        }

        public async Task<Music?> GetMusicByIdAsync(Guid MusicId)
        {
            return await dbContext.Musics.FirstOrDefaultAsync(m => m.Id == MusicId);
        }

        public async Task<Music[]> GetMusicsByArtistIdAsync(Guid ArtistId)
        {
            return await dbContext.Musics.Where(m => m.ArtistId == ArtistId).ToArrayAsync();
        }

        public async Task<Album[]> GetAlbumsByMusicPostOrder()
        {
            var musicList = await dbContext.Musics.OrderByDescending(m => m.CreationTime).ToArrayAsync();
            var musicId = musicList.Select(m => m.AlbumId).Distinct();
            List<Album> albums = new List<Album>();
            var tempAlbum = await dbContext.Albums.ToArrayAsync();
            foreach (var id in musicId)
            {
                albums.Add(tempAlbum.FirstOrDefault(a => a.Id == id));
            }
            return albums.ToArray();


        }

        public Task<Music[]> GetMusicsByPlayListIdAsync(Guid PlayListId)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayList[]> GetPlayListAsync()
        {
           return await dbContext.PlayLists.ToArrayAsync();
        }

        public async Task<PlayList[]> GetPlayListByNameAsync(string Name)
        {
           return await dbContext.PlayLists.Where(p => p.Title.Contains(Name)).ToArrayAsync();
        }
    }
}
