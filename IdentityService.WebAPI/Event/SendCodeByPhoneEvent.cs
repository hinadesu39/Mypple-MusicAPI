using MediatR;

namespace IdentityService.WebAPI.Event
{
    public record SendCodeByPhoneEvent(string PhoneNum, string token) : INotification;
}
