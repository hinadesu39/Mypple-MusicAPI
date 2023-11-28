using FluentValidation;

namespace IdentityService.WebAPI.Login.Request
{
    public record CreateUserWithPhoneNumAndCodeRequest(string userName, string PhoneNum, string passWord, string code);
    public class CreateUserWithPhoneNumAndCodeRequestValiadator : AbstractValidator<CreateUserWithPhoneNumAndCodeRequest>
    {
        public CreateUserWithPhoneNumAndCodeRequestValiadator()
        {
            RuleFor(e => e.passWord).NotNull().NotEmpty();
            RuleFor(e => e.userName).NotNull().NotEmpty();
            RuleFor(e => e.PhoneNum).NotEmpty().NotNull();
            RuleFor(e => e.code).NotNull().NotEmpty();
        }
    }

}
