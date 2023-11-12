using Microsoft.EntityFrameworkCore;
using MusicDomain;
using MusicDomain.Entity;
using MusicDomain.Entity.DTO;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MusicInfrastructure
{
    public class MusicRepository : IMusicRepository
    {
        private readonly MusicDBContext dbContext;
        private readonly IConnectionMultiplexer redisConn;

        public MusicRepository(MusicDBContext dbContext, IConnectionMultiplexer redisConn)
        {
            this.dbContext = dbContext;
            this.redisConn = redisConn;
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
            var musicList = await dbContext.Musics
                .OrderByDescending(m => m.CreationTime)
                .ToArrayAsync();
            var musicId = musicList.Select(m => m.AlbumId).Distinct();
            List<Album> albums = new List<Album>();
            var tempAlbum = await dbContext.Albums.ToArrayAsync();
            foreach (var id in musicId)
            {
                albums.Add(tempAlbum.FirstOrDefault(a => a.Id == id));
            }
            return albums.ToArray();
        }

        public async Task<MusicDTO[]> GetMusicsByPlayListIdAsync(Guid PlayListId)
        {
            var redisDb = redisConn.GetDatabase();
            var music = await redisDb.SetMembersAsync($"PlayList.{PlayListId}");
            return music.Select(m => JsonConvert.DeserializeObject<MusicDTO>(m)).ToArray();
        }

        public async Task<PlayListDTO[]> GetPlayListAsync()
        {
            var redisDb = redisConn.GetDatabase();
            var playList = await redisDb.SetMembersAsync("PlayList");
            return playList.Select(p => JsonConvert.DeserializeObject<PlayListDTO>(p)).ToArray();
        }

        public async Task<PlayList> AddPlayListAsync(Uri? PicUrl, string Title, string Description)
        {
            var playList = PlayList.Create(PicUrl, Title, Description);
            var redisDb = redisConn.GetDatabase();
            await redisDb.SetAddAsync("PlayList", JsonConvert.SerializeObject(playList));
            return playList;
        }

        public async Task<Music[]> AddMusicToPlayListAsync(Guid PlayListId, Music[] musics)
        {
            var redisDb = redisConn.GetDatabase();
            var tasks = musics.Select(
                async music =>
                    await redisDb.SetAddAsync(
                        $"PlayList.{PlayListId}",
                        JsonConvert.SerializeObject(music)
                    )
            );
            await Task.WhenAll(tasks);
            return musics;
        }

        public async Task<PlayListDTO?> GetPlayListByIdAsync(Guid PlayListId)
        {
            var redisDb = redisConn.GetDatabase();
            var playList = await redisDb.SetMembersAsync("PlayList");
            if (playList != null)
            {
                var res = playList
                    .Select(p => JsonConvert.DeserializeObject<PlayListDTO>(p))
                    .FirstOrDefault(p => p.Id == PlayListId);
                return res;
            }
            return null;
        }
    }
}
