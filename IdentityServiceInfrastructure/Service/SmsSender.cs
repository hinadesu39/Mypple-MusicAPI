using IdentityServiceDomain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure.Service
{
    public class SmsSender : ISmsSender
    {
        private readonly ILogger<SmsSender> logger;
        public SmsSender(ILogger<SmsSender> logger)
        {
            this.logger = logger;
        }
        public Task SendAsync(string phoneNum, params string[] args)
        {
            logger.LogInformation("Send Sms to {0},args:{1}", phoneNum,
                string.Join(",", args));
            return Task.CompletedTask;
        }
    }
}
