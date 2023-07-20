using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.WebAPI.Controllers.Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controllers.Login;

[Route("[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IIdentityRepository identityRepository;
    private readonly IdentityDomainService identityDomainService;

    public LoginController(IdentityDomainService identityDomainService, IIdentityRepository identityRepository)
    {
        this.identityDomainService = identityDomainService;
        this.identityRepository = identityRepository;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> CreateWorld()
    {
        if (await identityRepository.FindByNameAsync("Admin") != null)
        {
            return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
        }
        User user = new("Admin");
        var r = await identityRepository.CreateAsync(user, "123456");
        Debug.Assert(r.Succeeded);
        var token = await identityRepository.GenerateChangePhoneNumberTokenAsync(user, "189999999999");
        var cr = await identityRepository.ChangePhoneNumberAsync(user.Id, "189999999999", token);
        Debug.Assert(cr.Succeeded);
        r = await identityRepository.AddToRoleAsync(user, "Admin");
        Debug.Assert(r.Succeeded);
        r = await identityRepository.AddToRoleAsync(user, "Admin");
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
        var user = await identityRepository.FindByIdAsync(Guid.Parse(userId));
        if (user == null)//可能用户注销了
        {
            return NotFound();
        }

        return new UserResponse(user.Id, user.Email, user.PhoneNumber, user.CreationTime);
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string?>> LoginByPhoneAndPassword(LoginByPhoneAndPwdRequest req)
    {
        //todo：要通过行为验证码、图形验证码等形式来防止暴力破解
        (var checkResult, string? token) = await identityDomainService.LoginByPhoneAndPasswordAsync(req.PhoneNum, req.Password);
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
    public async Task<ActionResult<string>> LoginByUserNameAndPassword(LoginByUserNameAndPwdRequest req)
    {
        (var checkResult, var token) = await identityDomainService.LoginByUserNameAndPasswordAsync(req.UserName, req.Password);
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
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(nameIdentifier, out Guid userId);
        var resetPwdResult = await identityRepository.ChangePasswordAsync(userId, req.Password);
        if (resetPwdResult.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(resetPwdResult.Errors.SumErrors());
        }
    }
}