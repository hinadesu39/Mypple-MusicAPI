using IdentityServiceDomain;
using MediatR;

namespace IdentityService.WebAPI.Event
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }


        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            //发送初始密码给被创建用户的手机
            return smsSender.SendAsync(notification.PhoneNum, notification.Password);
        }
    }
}
