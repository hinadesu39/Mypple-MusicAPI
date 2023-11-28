using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain
{
    public interface IEmailSender
    {
        public Task SendAsync(string email, string subject, string body);
    }
}
