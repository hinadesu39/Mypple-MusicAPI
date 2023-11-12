using CommonHelper;
using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics.Request
{
    public record MusicAddRequest(
        Uri AudioUrl,
        Uri? PicUrl,
        string Title,
        double Duration,
        Guid ArtistId,
        string Artist,
        string Album,
        Guid AlbumId,
        string? Type,
        string? Lyric,
        int PublishTime      
    );

    public class MusicAddRequestValidator : AbstractValidator<MusicAddRequest>
    {
        public MusicAddRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.AudioUrl).NotEmpty();
            RuleFor(x => x.Title).NotNull().Length(1, 200);                   
        }
    }
}
