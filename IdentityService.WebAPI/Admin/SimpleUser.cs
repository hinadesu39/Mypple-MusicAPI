using IdentityServiceDomain.Entity;

namespace IdentityService.WebAPI.Admin
{
    public record SimpleUser(Guid Id, string UserName, string PhoneNumber, string Gender, string UserAvatar,string Email,  DateTime CreationTime)
    {
        public static SimpleUser Create(User user)
        {
            return new SimpleUser(user.Id, user.UserName, user.PhoneNumber,user.Gender,user.UserAvatar,user.Email, user.CreationTime);
        }
    }
}
