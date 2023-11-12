using FluentValidation;
using MusicAdmin.WebAPI.Musics.Request;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicMain.WebAPI.PlayLists.Request
{
    public record MusicAddToPlayListRequest(Guid PlayListId, MusicAddRequest[] musics);

    public class MusicAddToPlayListRequestValidator : AbstractValidator<MusicAddToPlayListRequest>
    {
        public MusicAddToPlayListRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.PlayListId).NotEmpty();
        }
    }

}
