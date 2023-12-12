using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain.Entity
{
    public class User : IdentityUser<Guid>
    {
        public string? Gender { get; private set; }

        public string? UserAvatar { get; private set; }

        public DateTime CreationTime { get; init; }

        public DateTime? DeletionTime { get; private set; }

        public bool IsDeleted { get; private set; }

        public User(string userName) : base(userName)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }

        public void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }
    }
}
