using FluentValidation;
using MusicInfrastructure;

namespace MusicMain.WebAPI.PlayLists.Request
{
    public record PlayListAddRequest(Uri? PicUrl, string Title, string? Description);

    public class PlayListAddRequestValidator : AbstractValidator<PlayListAddRequest>
    {
        public PlayListAddRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.Title).NotNull().Length(1, 200);
        }
    }
}
