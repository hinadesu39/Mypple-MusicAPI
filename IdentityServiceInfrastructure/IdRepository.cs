using IdentityServiceDomain;
using IdentityServiceDomain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text;

namespace IdentityServiceInfrastructure
{
    public class IdRepository : IIdRepository
    {
        private readonly IConnectionMultiplexer redisConn;
        private readonly IdUserManager userManager;
        private readonly RoleManager<IdentityServiceDomain.Entity.Role> roleManager;
        private readonly ILogger<IdRepository> logger;

        public IdRepository(
            IdUserManager userManager,
            RoleManager<IdentityServiceDomain.Entity.Role> roleManager,
            ILogger<IdRepository> logger,
            IConnectionMultiplexer redisConn
        )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.redisConn = redisConn;
        }


        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return userManager.AccessFailedAsync(user);
        }

        public async Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(
            string userName,
            string phoneNum
        )
        {
            if (await FindByNameAsync(userName) != null)
            {
                return (ErrorResult($"该用户名已经存在{userName}"), null, null);
            }
            if (await FindByPhoneNumberAsync(phoneNum) != null)
            {
                return (ErrorResult($"该手机号已经存在{phoneNum}"), null, null);
            }
            User user = new User(userName);
            user.PhoneNumber = phoneNum;
            user.PhoneNumberConfirmed = true;
            string password = GeneratePassword();
            var result = await CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            result = await AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            return (IdentityResult.Success, user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                IdentityServiceDomain.Entity.Role role = new IdentityServiceDomain.Entity.Role { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    return result;
                }
            }
            return await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<(IdentityResult, User?)> AddUserAsync(
            string userName,
            string phoneNumber,
            string? passWord
        )
        {
            if (await FindByNameAsync(userName) != null)
            {
                return (ErrorResult($"该用户名已经存在{userName}"), null);
            }
            if (await FindByPhoneNumberAsync(phoneNumber) != null)
            {
                return (ErrorResult($"该手机号已经存在{phoneNumber}"), null);
            }
            User user = new User(userName);
            user.PhoneNumber = phoneNumber;
            user.PhoneNumberConfirmed = true;
            var result = await CreateAsync(user, passWord);
            if (!result.Succeeded)
            {
                return (result, null);
            }
            result = await AddToRoleAsync(user, "User");
            if (!result.Succeeded)
            {
                return (result, null);
            }
            return (IdentityResult.Success, user);
        }

        public async Task<string> BuildCodeAsync(string phoneNum)
        {
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();
            var db = redisConn.GetDatabase();
            await db.StringSetAsync(phoneNum, code, TimeSpan.FromMinutes(5));
            return code;
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
        {
            if (password.Length < 6)
            {
                return ErrorResult("密码长度不能少于6");
            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetPwdResult = await userManager.ResetPasswordAsync(user, token, password);
            return resetPwdResult;
        }

        public async Task<IdentityResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ErrorResult("该用户不存在");
            }
            var changeResult = await this.userManager.ChangePhoneNumberAsync(user, phoneNum, token);
            if (changeResult.Succeeded)
            {
                await ConfirmPhoneNumberAsync(userId);
                return changeResult;
            }
            else
            {
                return ErrorResult("修改手机号失败");
            }
        }

        public async Task<IdentityResult> CheckForCodeAsync(string phoneNum, string code)
        {
            var db = redisConn.GetDatabase();
            var value = db.StringGet(phoneNum).ToString();
            if (value != code || value == null)
            {
                return ErrorResult("验证码错误，请重新发送");
            }
            return IdentityResult.Success;
        }

        public async Task<SignInResult> CheckForSignInAsync(
            User user,
            string password,
            bool lockoutOnFailure
        )
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            var result = await userManager.CheckPasswordAsync(user, password);
            if (result)
            {
                await ResetAccessFailedCountAsync(user);
                return SignInResult.Success;
            }
            else//登录不成功设置访问失败次数
            {
                if (lockoutOnFailure)
                {
                    var r = await AccessFailedAsync(user);
                    if (!r.Succeeded)
                    {
                        throw new ApplicationException("设置访问失败次数失败");
                    }
                }
                return SignInResult.Failed;
            }
        }

        public async Task ConfirmPhoneNumberAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumberConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<User?> FindByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User?> FindByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<User?> FindByPhoneNumberAsync(string phoneNum)
        {
            return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
        }

        public async Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
        {
            return await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            var userLoginStore = userManager.UserLoginStore;
            var noneCT = default(CancellationToken);
            //一定要删除aspnetuserlogins表中的数据，否则再次用这个外部登录登录的话
            //就会报错：The instance of entity type 'IdentityUserLogin<Guid>' cannot be tracked because another instance with the same key value for {'LoginProvider', 'ProviderKey'} is already being tracked.
            //而且要先删除aspnetuserlogins数据，再软删除User
            var logins = await userLoginStore.GetLoginsAsync(user, noneCT);
            foreach (var login in logins)
            {
                await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);
            }
            user.SoftDelete();
            var result = await userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> ResetAccessFailedCountAsync(User user)
        {
            return await userManager.ResetAccessFailedCountAsync(user);
        }

        public async Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                return (ErrorResult("用户没找到"), null, null);
            }
            string password = GeneratePassword();
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            return (IdentityResult.Success, user, password);
        }



        private static IdentityResult ErrorResult(string msg)
        {
            IdentityError idError = new IdentityError { Description = msg };
            return IdentityResult.Failed(idError);
        }
            
        private string GeneratePassword()
        {
            var options = userManager.Options.Password;
            int length = options.RequiredLength;
            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));
            return password.ToString();
        }
    }
}
