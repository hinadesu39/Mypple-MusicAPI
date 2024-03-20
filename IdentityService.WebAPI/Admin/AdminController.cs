using IdentityService.WebAPI.Admin.Request;
using IdentityService.WebAPI.Event;
using IdentityServiceDomain;
using IdentityServiceInfrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace IdentityService.WebAPI.Admin
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IdUserManager userManager;
        private readonly IIdRepository repository;
        private IMediator mediator;

        public AdminController(
            IdUserManager userManager,
            IIdRepository repository,
            IMediator mediator
        )
        {
            this.userManager = userManager;
            this.repository = repository;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<SimpleUser[]> FindAllUsers()
        {
            var users = await userManager.Users.ToArrayAsync();
            return users
                .Select(u => SimpleUser.Create(u, userManager.IsInRoleAsync(u, "Admin").Result))
                .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req)
        {
            (var result, var user, var password) = await repository.AddAdminUserAsync(
                req.UserName,
                req.PhoneNum
            );
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //生成的密码短信发给对方
            //可以同时或者选择性的把新增用户的密码短信/邮件/打印给用户
            //体现了领域事件对于代码“高内聚、低耦合”的追求
            var userCreatedEvent = new UserCreatedEvent(
                user.Id,
                req.UserName,
                password,
                req.PhoneNum
            );
            await mediator.Publish(userCreatedEvent);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<string>> DeleteUser(Guid id)
        {
            var res = await repository.RemoveUserAsync(id);
            if (!res.Succeeded)
            {
                return BadRequest("BadRequest");
            }
            return Ok("Ok");
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<string>> UpdateUser(Guid id, SimpleUser req)
        {
            var user = await repository.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("NotFound");
            }

            user.UserAvatar = req.UserAvatar;
            user.UserName = req.UserName;
            user.Gender = req.Gender;
            user.Email = req.Email;
            user.BirthDay = req.BirthDay;
            user.PhoneNumber = req.PhoneNumber;

            if (req.isAdmin)
            {
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }
            await userManager.UpdateAsync(user);
            return Ok("Ok");
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult> ResetUserPassword(Guid id)
        {
            (var result, var user, var password) = await repository.ResetPasswordAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //生成的密码短信发给对方
            var eventData = new ResetPasswordEvent(
                user.Id,
                user.UserName,
                password,
                user.PhoneNumber
            );
            await mediator.Publish(eventData);
            return Ok();
        }
    }
}
