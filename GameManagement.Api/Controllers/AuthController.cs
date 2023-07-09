﻿using GameManagement.Api.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApiIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IConfiguration configuration,
            UserManager<ApiIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grantType)
        {
            return await IsValidUsernameAndPassword(username, password) ? Ok() : BadRequest();
        }

        private async Task<bool> IsValidUsernameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user is not null && await _userManager.CheckPasswordAsync(user, password);
        }

        //private string CreateToken(Company user)
        //{
        //    List<Claim> claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim(ClaimTypes.Role, "Admin")
        //    };

        //    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
        //        configuration.GetSection("AppSettings:Token").Value!));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(1),
        //        signingCredentials: creds);

        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //    return jwt;
        //}

        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}

        //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512(passwordSalt))
        //    {
        //        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        return computedHash.SequenceEqual(passwordHash);
        //    }
        //}
    }
}