using CommonHelper;
using IdentityServiceDomain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain
{
    public class IdDomainService
    {
        private readonly IIdRepository repository;
        private readonly IOptions<JWTOptions> optJWT;

        public IdDomainService(IIdRepository repository, IOptions<JWTOptions> optJWT)
        {
            this.repository = repository;
            this.optJWT = optJWT;
        }

        /// <summary>
        /// 根据给定用户的信息（包括其ID和角色）构建一个JWT令牌。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> BuildTokenAsync(User user)
        {
            //获取该用户所有的角色
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return TokenService.BuildToken(claims, optJWT.Value);
        }

        /// <summary>
        /// 手机号登录
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<(SignInResult result, string? token)> LoginByPhoneAndPwdAsync(
            string phoneNum,
            string password
        )
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNum);
            if (user == null)
            {
                return (SignInResult.Failed, null);
            }

            var result = await repository.CheckForSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (result, null);
            }
        }

        /// <summary>
        /// 用户名登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<(SignInResult result, string? token)> LoginByUserNameAndPwdAsync(
            string userName,
            string password
        )
        {
            var user = await repository.FindByNameAsync(userName);
            if (user == null)
            {
                return (SignInResult.Failed, null);
            }

            var result = await repository.CheckForSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (result, null);
            }
        }
    }
}
