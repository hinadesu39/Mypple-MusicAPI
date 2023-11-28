using IdentityServiceDomain;
using MediatR;

namespace IdentityService.WebAPI.Event
{
    public class SendCodeByPhoneEventHandler : INotificationHandler<SendCodeByPhoneEvent>
    {
        private readonly ISmsSender smsSender;

        public SendCodeByPhoneEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }


        public Task Handle(SendCodeByPhoneEvent notification, CancellationToken cancellationToken)
        {
            //发送验证码给被创建用户的手机
            return smsSender.SendAsync(notification.PhoneNum, notification.token);
        }
    }
}
