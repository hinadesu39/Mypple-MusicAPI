using IdentityServiceDomain;
using MediatR;

namespace IdentityService.WebAPI.Event
{
    public class SendCodeByEmailEventHandler : INotificationHandler<SendCodeByEmailEvent>
    {
        private readonly IEmailSender emailSender;
        public SendCodeByEmailEventHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }
        public async Task Handle(SendCodeByEmailEvent notification, CancellationToken cancellationToken)
        {
            await emailSender.SendAsync(notification.Email, "您的MyppleMusic登录验证码", notification.token);
        }
    }
}
