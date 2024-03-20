using FluentValidation;

namespace IdentityService.WebAPI.Admin.Request
{
    public record EditUserRequest(string PhoneNum);
    public class EditAdminUserRequestValidator : AbstractValidator<EditUserRequest>
    {
        public EditAdminUserRequestValidator()
        {
            RuleFor(e => e.PhoneNum).NotNull().NotEmpty();
        }
    }
}
