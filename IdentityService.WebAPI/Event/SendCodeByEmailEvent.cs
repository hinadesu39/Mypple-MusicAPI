using MediatR;

namespace IdentityService.WebAPI.Event
{
    public record SendCodeByEmailEvent(string Email,string token) : INotification;
}
