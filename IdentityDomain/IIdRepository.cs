using IdentityServiceDomain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain
{
    public interface IIdRepository
    {
        Task<User?> FindByIdAsync(Guid userId); //根据Id获取用户
        Task<User?> FindByNameAsync(string userName); //根据用户名获取用户
        Task<User?> FindByPhoneNumberAsync(string phoneNum); //根据手机号获取用户
        Task<User?> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(User user, string password); //创建用户
        Task<IdentityResult> AccessFailedAsync(User user); //记录一次登陆失败
        Task<IdentityResult> ResetAccessFailedCountAsync(User user); //重制登录失败次数

        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        Task<IdentityResult> CheckForCodeAsync(string account, string code);

        /// <summary>
        /// 为新注册的用户的指定的手机号发验证码
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        Task<string> BuildCodeAsync(string phoneNum);

        /// <summary>
        /// 为已创建的用户生成重置密码的令牌
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber);

        /// <summary>
        /// 为已创建的用户生成重置密码的令牌
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<string> GenerateChangeEmailTokenAsync(User user, string email);

        /// <summary>
        /// 检查VCode，然后设置用户手机号为phoneNum
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNum"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangeEmailAsync(Guid userId, string email, string token);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangePasswordAsync(Guid userId, string password);

        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<string>> GetRolesAsync(User user);

        /// <summary>
        /// 把用户user加入角色role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IdentityResult> AddToRoleAsync(User user, string role);

        /// <summary>
        /// 为了登录而检查用户名、密码是否正确
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure">如果登录失败，则记录一次登陆失败</param>
        /// <returns></returns>
        public Task<SignInResult> CheckForSignInAsync(
            User user,
            string password,
            bool lockoutOnFailure
        );

        /// <summary>
        /// 确认手机号保存修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task ConfirmPhoneNumberAsync(Guid id);

        /// <summary>
        /// 确认邮箱号保存修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task ConfirmEmailAsync(Guid id);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IdentityResult> RemoveUserAsync(Guid id);

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNum"></param>
        /// <returns>返回值第三个是生成的密码</returns>
        public Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(
            string userName,
            string phoneNum
        );

        /// <summary>
        /// 添加普通用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public Task<(IdentityResult, User?)> AddUserAsync(
            string userName,
            string phoneNumber,
            string? passWord
        );

        /// <summary>
        /// 重置密码。
        /// </summary>
        /// <param name="id"></param>
        /// <returns>返回值第三个是生成的密码</returns>
        public Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id);

        public Task<(IdentityResult, User?)> UpdateUserInfoAsync(Guid userId, string userName, string? Gender, DateTime? BirthDay, Uri? UserAvatar);
    }
}
