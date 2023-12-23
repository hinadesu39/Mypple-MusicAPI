using FluentValidation;

namespace IdentityService.WebAPI.Login.Request
{
    public record ConfirmRequest(string Account, string Code);
    public class ConfirmRequestValidator : AbstractValidator<ConfirmRequest>
    {
        public ConfirmRequestValidator() 
        {
            RuleFor(e => e.Account).NotNull().NotEmpty();
            RuleFor(e => e.Code).NotNull().NotEmpty();
        }
    }
}
