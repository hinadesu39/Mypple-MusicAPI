namespace IdentityService.WebAPI.Login.Request
{
    public record UpdateUserInfoRequest(string UserName, string? Gender, DateTime BirthDay, Uri? UserAvatar);
}
