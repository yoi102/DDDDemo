using Commons;
using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.Controllers.Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controllers.Login;

[Route("login")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IIdentityRepository repository;
    private readonly IdentityDomainService domainService;

    public LoginController(IdentityDomainService domainService, IIdentityRepository repository)
    {
        this.domainService = domainService;
        this.repository = repository;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> CreateWorld()
    {
        if (await repository.FindByNameAsync(UserRoles.Administrator) is not null)
        {
            return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
        }
        User user = new(UserRoles.Administrator);
        var r = await repository.CreateAsync(user, "123456");
        Debug.Assert(r.Succeeded);
        var token = await repository.GenerateChangePhoneNumberTokenAsync(user, "18999999999");
        var cr = await repository.ChangePhoneNumberAsync(user.Id, "18999999999", token);
        Debug.Assert(cr.Succeeded);
        r = await repository.AddToRoleAsync(user, UserRoles.Administrator);
        Debug.Assert(r.Succeeded);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }
        var user = await repository.FindByIdAsync(Guid.Parse(userId));
        return user == null ? (ActionResult<UserResponse>)NotFound() : (ActionResult<UserResponse>)new UserResponse(user.Id, user.Email, user.PhoneNumber, user.CreationTime);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login-by-phone-and-password")]
    public async Task<ActionResult<string?>> LoginByPhoneAndPassword(LoginByPhoneAndPwdRequest request)
    {
        //todo：要通过行为验证码、图形验证码等形式来防止暴力破解
        (var checkResult, string? token) = await domainService.LoginByPhoneAndPasswordAsync(request.PhoneNumber, request.Password);
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
    [Route("login-by-username-and-password")]
    public async Task<ActionResult<string>> LoginByUserNameAndPassword(LoginByUserNameAndPwdRequest request)
    {
        (var checkResult, var token) = await domainService.LoginByUserNameAndPasswordAsync(request.UserName, request.Password);
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
    [Route("change-my-password")]
    public async Task<ActionResult> ChangeMyPassword(string password)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(nameIdentifier, out Guid userId);
        var resetPwdResult = await repository.ChangePasswordAsync(userId, password);
        return resetPwdResult.Succeeded ? Ok() : BadRequest(resetPwdResult.Errors.SumErrors());
    }
}