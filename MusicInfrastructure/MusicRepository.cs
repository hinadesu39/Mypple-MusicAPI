using Microsoft.AspNetCore.Mvc;
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

        public async Task<PlayListDTO[]> GetPlayListAsync(Guid UserId)
        {
            var redisDb = redisConn.GetDatabase();
            var playList = await redisDb.SetMembersAsync(UserId.ToString());
            return playList.Select(p => JsonConvert.DeserializeObject<PlayListDTO>(p)).ToArray();
        }

        public async Task<PlayList> AddPlayListAsync(
            Guid UserId,
            Uri? PicUrl,
            string Title,
            string Description
        )
        {
            var playList = PlayList.Create(PicUrl, Title, Description);
            var redisDb = redisConn.GetDatabase();
            await redisDb.SetAddAsync(UserId.ToString(), JsonConvert.SerializeObject(playList));
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

        public async Task<PlayListDTO?> GetPlayListByIdAsync(Guid UserId, Guid PlayListId)
        {
            var redisDb = redisConn.GetDatabase();
            var playList = await redisDb.SetMembersAsync(UserId.ToString());
            if (playList != null)
            {
                var res = playList
                    .Select(p => JsonConvert.DeserializeObject<PlayListDTO>(p))
                    .FirstOrDefault(p => p.Id == PlayListId);
                return res;
            }
            return null;
        }

        public async Task<SearchedResult> GetByKeyWordsAsync(string keyWords)
        {
            var musics = await dbContext.Musics
                .Where(
                    m =>
                        m.Title.Contains(keyWords)
                        || m.Album.Contains(keyWords)
                        || m.Artist.Contains(keyWords)
                )
                .ToArrayAsync();
            var albums = await dbContext.Albums
                .Where(a => a.Title.Contains(keyWords))
                .ToArrayAsync();
            var artists = await dbContext.Artists
                .Where(a => a.Name.Contains(keyWords))
                .ToArrayAsync();
            return new SearchedResult(musics, albums, artists);
        }

        public async Task<Music?> MusicExist(
            string Title,
            double Duration,
            string Artist,
            string Album
        )
        {
            return await dbContext.Musics.FirstOrDefaultAsync(
                m =>
                    m.Title == Title
                    && m.Duration == Duration
                    && m.Artist == Artist
                    && m.Album == Album
            );
        }

        public async Task<bool> RemovePlayListAsync(Guid userId, Guid PlayListId)
        {
            var redisDb = redisConn.GetDatabase();
            var user = await redisDb.SetMembersAsync(userId.ToString());
            var playList = user.Select(p => JsonConvert.DeserializeObject<PlayListDTO>(p))
                .FirstOrDefault(p => p.Id == PlayListId);
            var newitem = JsonConvert.SerializeObject(playList);
            await redisDb.KeyDeleteAsync($"PlayList.{PlayListId}");
            return await redisDb.SetRemoveAsync(userId.ToString(), newitem);
        }

        public async Task<bool> RemoveMusicFromPlayListAsync(Guid PlayListId, Guid MusicId)
        {
            var redisDb = redisConn.GetDatabase();
            var playList = await redisDb.SetMembersAsync($"PlayList.{PlayListId}");
            var item = playList
                .Select(m => JsonConvert.DeserializeObject<MusicDTO>(m))
                .FirstOrDefault(m => m.Id == MusicId);
            var newitem = JsonConvert.SerializeObject(item);
            return await redisDb.SetRemoveAsync($"PlayList.{PlayListId}", newitem);
        }

        public async Task<Artist?> ArtistExist(string Name)
        {
            return await dbContext.Artists.FirstOrDefaultAsync(a => a.Name == Name);
        }

        public async Task<Album?> AlbumExist(string Title, string Artist, int PublishTime)
        {
            return await dbContext.Albums.FirstOrDefaultAsync(
                a => a.Title == Title && a.Artist == Artist && a.PublishTime == PublishTime
            );
        }
    }
}
