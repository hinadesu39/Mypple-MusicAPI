using IdentityService.WebAPI.Event;
using IdentityService.WebAPI.Login.Request;
using IdentityServiceDomain.Entity;
using IdentityServiceDomain;
using IdentityServiceInfrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IdentityService.WebAPI.Admin;

namespace IdentityService.WebAPI.Login
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IIdRepository repository;
        private readonly IdDomainService idService;
        private readonly IdUserManager userManager;
        private IMediator mediator;

        public LoginController(IdDomainService idService, IIdRepository repository, IMediator mediator, IdUserManager userManager)
        {
            this.idService = idService;
            this.repository = repository;
            this.mediator = mediator;
            this.userManager = userManager;
        }
        /// <summary>
        /// 仅供测试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateWorld()
        {
            if (await repository.FindByNameAsync("admin") != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
            }
            User user = new User("admin");
            await repository.CreateAsync(user, "123456");

            var token = await repository.GenerateChangePhoneNumberTokenAsync(user, "19550595456");
            await repository.ChangePhoneNumAsync(user.Id, "19550595456", token);

            await repository.AddToRoleAsync(user, "User");

            await repository.AddToRoleAsync(user, "Admin");

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<SimpleUser>> GetUserInfo()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await repository.FindByIdAsync(Guid.Parse(userId));
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            //出于安全考虑，不要机密信息传递到客户端
            //除非确认没问题，否则尽量不要直接把实体类对象返回给前端
            return new SimpleUser(user.Id, user.UserName,user.PhoneNumber, user.CreationTime);
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string?>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
        {
            //todo：要通过行为验证码、图形验证码等形式来防止暴力破解
            (var checkResult, string? token) = await idService.LoginByPhoneAndPwdAsync(req.PhoneNum, req.Password);
            if (checkResult.Succeeded)
            {
                return token;
            }
            else if (checkResult.IsLockedOut)
            {
                //尝试登录次数太多
                return StatusCode((int)HttpStatusCode.Locked, "此账号已经锁定");
            }
            else
            {
                string msg = "登录失败";
                return StatusCode((int)HttpStatusCode.BadRequest, msg);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> LoginByUserNameAndPwd(
            LoginByUserNameAndPwdRequest req)
        {
            (var checkResult, var token) = await idService.LoginByUserNameAndPwdAsync(req.UserName, req.Password);
            if (checkResult.Succeeded) return token!;
            else if (checkResult.IsLockedOut)//尝试登录次数太多
                return StatusCode((int)HttpStatusCode.Locked, "用户已经被锁定");
            else
            {
                string msg = checkResult.ToString();
                return BadRequest("登录失败" + msg);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangeMyPassword(ChangeMyPasswordRequest req)
        {
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var resetPwdResult = await repository.ChangePasswordAsync(userId, req.Password);
            if (resetPwdResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(resetPwdResult.Errors);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CreateUserWithPhoneNumAndCode(CreateUserWithPhoneNumAndCodeRequest req)
        {
            string userName = req.userName;
            string code = req.code;
            string phoneNum = req.PhoneNum;
            string passWord = req.passWord;
            var result = await repository.CheckForCodeAsync(phoneNum, code);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            (result, var user) = await repository.AddUserAsync(userName, phoneNum, passWord);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ChangeMyPasswordWithCode(ChangeMyPasswordWithCodeRequest req)
        {
            string phoneNum = req.phoneNum;
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
            if (user == null)
            {
                return NotFound("未找到该用户");
            }
            string code = req.code;
            var result = await repository.CheckForCodeAsync(phoneNum, code);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var resetPwdResult = await repository.ChangePasswordAsync(user.Id, req.Password);
            if (resetPwdResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(resetPwdResult.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> SendCodeByPhone(SendCodeByPhoneRequest req)
        {
            string phoneNum = req.PhoneNumber;
            string code = await repository.BuildCodeAsync(phoneNum);
            var sendCodeEvent = new SendCodeByPhoneEvent(phoneNum, code);
            mediator.Publish(sendCodeEvent);
            return Ok();
        }
    }
}
