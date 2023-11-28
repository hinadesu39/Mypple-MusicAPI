using IdentityService.WebAPI.Admin.Request;
using IdentityService.WebAPI.Event;
using IdentityServiceDomain;
using IdentityServiceInfrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public AdminController(IdUserManager userManager, IIdRepository repository, IMediator mediator)
        {
            this.userManager = userManager;
            this.repository = repository;
            this.mediator = mediator;
        }
        [HttpGet]
        public Task<SimpleUser[]> FindAllUsers()
        {
            return userManager.Users.Select(u => SimpleUser.Create(u)).ToArrayAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req)
        {
            (var result, var user, var password) = await repository
                .AddAdminUserAsync(req.UserName, req.PhoneNum);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //生成的密码短信发给对方
            //可以同时或者选择性的把新增用户的密码短信/邮件/打印给用户
            //体现了领域事件对于代码“高内聚、低耦合”的追求
            var userCreatedEvent = new UserCreatedEvent(user.Id, req.UserName, password, req.PhoneNum);
            mediator.Publish(userCreatedEvent);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAdminUser(Guid id)
        {
            await repository.RemoveUserAsync(id);
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAdminUser(Guid id, EditAdminUserRequest req)
        {
            var user = await repository.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("用户没找到");
            }
            user.PhoneNumber = req.PhoneNum;
            await userManager.UpdateAsync(user);
            return Ok();
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult> ResetAdminUserPassword(Guid id)
        {
            (var result, var user, var password) = await repository.ResetPasswordAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //生成的密码短信发给对方
            var eventData = new ResetPasswordEvent(user.Id, user.UserName, password, user.PhoneNumber);
            await mediator.Publish(eventData);
            return Ok();
        }
    }
}
