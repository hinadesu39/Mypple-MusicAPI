using IdentityServiceDomain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;
        public EmailSender(ILogger<EmailSender> logger)
        {
            this.logger = logger;
        }
        public Task SendAsync(string email, string subject, string body)
        {
            logger.LogInformation("Send Email to {0},title:{1}, body:{2}", email, subject, body);
            return Task.CompletedTask;
        }
    }
}
