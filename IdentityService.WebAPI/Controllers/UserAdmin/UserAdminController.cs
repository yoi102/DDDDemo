using Commons;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.Controllers.UserAdmin.Models;
using IdentityService.WebAPI.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.WebAPI.Controllers.UserAdmin;

[Route("useradmin")]
[ApiController]
[Authorize(Roles = UserRoles.Administrator)]
public class UserAdminController : ControllerBase
{
    private readonly IdentityUserManager identityUserManager;
    private readonly IMediator mediator;
    private readonly IIdentityRepository identityRepository;

    public UserAdminController(IdentityUserManager identityUserManager, IMediator mediator, IIdentityRepository identityRepository)
    {
        this.identityUserManager = identityUserManager;
        this.mediator = mediator;
        this.identityRepository = identityRepository;

    }

    [HttpGet]
    public Task<UserDTO[]> FindAllUsers()
    {
        return identityUserManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<UserDTO>> FindById(Guid id)
    {
        var user = await identityUserManager.FindByIdAsync(id.ToString());
        return user is null ? (ActionResult<UserDTO>)NotFound() : (ActionResult<UserDTO>)UserDTO.Create(user);
    }


    [HttpPost]
    public async Task<ActionResult> AddAdminUser(AddAdminUserRequest request)
    {
        (var result, var user, var password) = await identityRepository.AddAdminUserAsync(request.UserName, request.PhoneNumber);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.SumErrors());
        }

        var userCreatedEvent = new UserCreatedEvent(user!.Id, request.UserName, password!, request.PhoneNumber);
        await mediator.Publish(userCreatedEvent);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteAdminUser(Guid id)
    {
        await identityRepository.RemoveUserAsync(id);
        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdateAdminUser(Guid id, EditAdminUserRequest request)
    {
        var user = await identityRepository.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("用户没找到");
        }
        user.PhoneNumber = request.PhoneNumber;
        await identityUserManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<ActionResult> ResetAdminUserPassword(Guid id)
    {
        (var result, var user, var password) = await identityRepository.ResetPasswordAsync(id);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.SumErrors());
        }
        //生成的密码短信发给对方
        ResetPasswordEvent resetPassword = new(user!.Id, user!.UserName!, password!, user!.PhoneNumber!);
        await mediator.Publish(resetPassword);
        return Ok();
    }
}