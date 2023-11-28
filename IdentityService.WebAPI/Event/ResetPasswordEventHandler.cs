using IdentityServiceDomain;
using MediatR;

namespace IdentityService.WebAPI.Event
{
    public class ResetPasswordEventHandler : INotificationHandler<ResetPasswordEvent>
    {
        private readonly ISmsSender smsSender;

        public ResetPasswordEventHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }


        public Task Handle(ResetPasswordEvent notification, CancellationToken cancellationToken)
        {
            //发送密码给被创建用户的手机
            return smsSender.SendAsync(notification.PhoneNum, notification.Password);
        }
    }
}
