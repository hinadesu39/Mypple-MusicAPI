using MusicDomain.Entity.DTO;

namespace MusicMain.WebAPI.PlayLists.Request
{
    public record RemoveMusicFromPlayListRequest(Guid PlayListId,Guid MusicId);

}
