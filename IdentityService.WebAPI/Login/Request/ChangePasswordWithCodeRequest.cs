using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace IdentityService.WebAPI.Login.Request
{
    public record ChangePasswordWithCodeRequest(string account, string code, string Password, string Password2);
    public class ChangePasswordWithCodeRequestValidator : AbstractValidator<ChangePasswordWithCodeRequest>
    {
        public ChangePasswordWithCodeRequestValidator()
        {
            RuleFor(e => e.account).NotNull().NotEmpty();
            RuleFor(e => e.code).NotEmpty().NotNull();
            RuleFor(e => e.Password).NotEmpty().NotNull()
                .Equal(e => e.Password2);
        }
    }

}
