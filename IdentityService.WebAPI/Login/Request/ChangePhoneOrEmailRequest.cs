using FluentValidation;

namespace IdentityService.WebAPI.Login.Request
{
    public record ChangePhoneOrEmailRequest(string Account, string Token);

    public class ChangePhoneOrEmailRequesttValidator : AbstractValidator<ChangePhoneOrEmailRequest>
    {
        public ChangePhoneOrEmailRequesttValidator()
        {
            RuleFor(e => e.Account).NotNull().NotEmpty();
            RuleFor(e => e.Token).NotNull().NotEmpty();
        }
    }
}
