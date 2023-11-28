using FluentValidation;

namespace IdentityService.WebAPI.Admin.Request
{
    public record EditAdminUserRequest(string PhoneNum);
    public class EditAdminUserRequestValidator : AbstractValidator<EditAdminUserRequest>
    {
        public EditAdminUserRequestValidator()
        {
            RuleFor(e => e.PhoneNum).NotNull().NotEmpty();
        }
    }
}
