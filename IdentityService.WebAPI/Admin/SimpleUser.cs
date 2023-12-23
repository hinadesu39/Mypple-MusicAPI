using IdentityServiceDomain.Entity;

namespace IdentityService.WebAPI.Admin
{
    public record SimpleUser(
        Guid Id,
        string UserName,
        string PhoneNumber,
        string? Gender,
        Uri? UserAvatar,
        string Email,
        DateTime? BirthDay,
        DateTime CreationTime
    )
    {
        public static SimpleUser Create(User user)
        {
            return new SimpleUser(
                user.Id,
                user.UserName,
                user.PhoneNumber,
                user.Gender,
                user.UserAvatar,
                user.Email,
                user.BirthDay,
                user.CreationTime
            );
        }
    }
}
