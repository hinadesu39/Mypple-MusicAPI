using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace IdentityService.WebAPI.Login.Request
{
    public record ChangeMyPasswordWithCodeRequest(string phoneNum, string code, string Password, string Password2);
    public class ChangeMyPasswordWithCodeRequestValidator : AbstractValidator<ChangeMyPasswordWithCodeRequest>
    {
        public ChangeMyPasswordWithCodeRequestValidator()
        {
            RuleFor(e => e.phoneNum).NotNull().NotEmpty();
            RuleFor(e => e.code).NotEmpty().NotNull();
            RuleFor(e => e.Password).NotEmpty().NotNull()
                .Equal(e => e.Password2);
        }
    }

}
