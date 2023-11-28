using Microsoft.AspNetCore.Identity;

namespace IdentityServiceDomain.Entity
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
