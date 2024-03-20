using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Albums.Request
{
    public record AlbumUpdateRequest(
        Guid Id,
        Uri? PicUrl,
        string Title,
        Guid ArtistId,
        string Artist,
        string? Type,
        int PublishTime
    );

    public class AlbumUpdateRequestValidator : AbstractValidator<AlbumUpdateRequest>
    {
        public AlbumUpdateRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.Title).NotEmpty().Length(1, 200);
            RuleFor(x => x.Artist).NotEmpty().Length(1, 200);
            RuleFor(x => x.ArtistId).NotEmpty();
        }
    }
}
