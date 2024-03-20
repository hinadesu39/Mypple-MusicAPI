using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics.Request
{
    public record MusicUpdateRequest(
        Guid Id,
        Uri AudioUrl,
        Uri? MusicPicUrl,
        string Title,
        Guid ArtistId,
        string Artist,
        Guid AlbumId,
        string Album,
        string? Type,
        string? Lyric,
        int PublishTime
    );

    public class MusicUpdateRequestValidator : AbstractValidator<MusicUpdateRequest>
    {
        public MusicUpdateRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.AudioUrl).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().Length(1, 200);
            RuleFor(x => x.Artist).NotEmpty().Length(1, 200);
            RuleFor(x => x.ArtistId).NotEmpty();
            RuleFor(x => x.Album).NotEmpty().Length(1, 200);
            RuleFor(x => x.AlbumId).NotEmpty();
            RuleFor(x => x.PublishTime != 0);
        }
    }
}
