using CommonHelper;
using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics.Request
{
    public record MusicAddRequest(
        Uri AudioUrl,
        Uri? MusicPicUrl,
        Uri? AlbumPicUrl,
        Uri? ArtistPicUrl,
        string Title,
        double Duration,
        string Artist,
        string Album,
        string? Type,
        string? Lyric,
        int PublishTime
       
    );

    public class EpisodeAddRequestValidator : AbstractValidator<MusicAddRequest>
    {
        public EpisodeAddRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.AudioUrl).NotEmpty();
            RuleFor(x => x.Title).NotNull().Length(1, 200);                   
        }
    }
}
