using MediatR;

namespace IdentityService.WebAPI.Event
{
    public record ResetPasswordEvent(Guid Id, string UserName, string Password, string PhoneNum) : INotification;
}
