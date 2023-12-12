using FluentValidation;

namespace IdentityService.WebAPI.Login.Request
{
    public record LoginByEmailAndCodeRequest(string Email, string Code);
    public class LoginByEmailAndCodeRequestValidator : AbstractValidator<LoginByEmailAndCodeRequest>
    {
        public LoginByEmailAndCodeRequestValidator()
        {
            RuleFor(e => e.Email).NotNull().NotEmpty();
            RuleFor(e => e.Code).NotNull().NotEmpty();
        }
    }
}
