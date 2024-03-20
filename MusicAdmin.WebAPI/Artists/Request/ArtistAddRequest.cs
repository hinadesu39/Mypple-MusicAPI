using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Artists.Request
{
    public record ArtistAddRequest(Uri? PicUrl, string Name);

    public class ArtistAddRequestValidator : AbstractValidator<ArtistAddRequest>
    {
        public ArtistAddRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 200);
        }
    }
}
