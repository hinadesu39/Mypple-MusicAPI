using MediatR;

namespace IdentityService.WebAPI.Event
{
    public record UserCreatedEvent(Guid Id, string UserName, string Password, string PhoneNum) : INotification;
}

