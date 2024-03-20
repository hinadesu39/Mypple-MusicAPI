using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Artists.Request
{
    public record ArtistUpdateRequest(Guid id,Uri? PicUrl, string Name);

    public class ArtistUpdateRequestValidator : AbstractValidator<ArtistUpdateRequest>
    {
        public ArtistUpdateRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(1, 200);
        }
    }
}
