using IdentityServiceDomain.Entity;

namespace IdentityService.WebAPI.Admin
{
    public record SimpleUser(
        Guid Id,
        string UserName,
        string PhoneNumber,
        string? Gender,
        Uri? UserAvatar,
        string? Email,
        DateTime? BirthDay,
        DateTime CreationTime,
        bool isAdmin
    )
    {
        public static SimpleUser Create(User user,bool isAdmin)
        {
            return new SimpleUser(
                user.Id,
                user.UserName,
                user.PhoneNumber,
                user.Gender,
                user.UserAvatar,
                user.Email,
                user.BirthDay,
                user.CreationTime,
                isAdmin
            );
        }
    }
}
