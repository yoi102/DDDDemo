using IdentityService.Domain.Entities;
using JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityService.Domain
{
    public class IdentityDomainService
    {
        private readonly IIdentityRepository repository;
        private readonly ITokenService tokenService;
        private readonly IOptions<JWTOptions> optJWT;

        public IdentityDomainService(IIdentityRepository repository,
             ITokenService tokenService, IOptions<JWTOptions> optJWT)
        {
            this.repository = repository;
            this.tokenService = tokenService;
            this.optJWT = optJWT;
        }

        private async Task<SignInResult> CheckUserNameAndPasswordAsync(string userName, string password)
        {
            var user = await repository.FindByNameAsync(userName);
            if (user is null)
            {
                return SignInResult.Failed;
            }
            //CheckPasswordSignInAsync会对于多次重复失败进行账号禁用
            var result = await repository.CheckForSignInAsync(user, password, true);
            return result;
        }
        private async Task<SignInResult> CheckPhoneNumAndPasswordAsync(string phoneNumber, string password)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNumber);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await repository.CheckForSignInAsync(user, password, true);
            return result;
        }

        //<(SignInResult Result, string? Token)>  元组的语法
        public async Task<(SignInResult Result, string? Token)> LoginByPhoneAndPasswordAsync(string phoneNumber, string password)
        {
            var checkResult = await CheckPhoneNumAndPasswordAsync(phoneNumber, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByPhoneNumberAsync(phoneNumber);
                string token = await BuildTokenAsync(user!);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }

        public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPasswordAsync(string userName, string password)
        {
            var checkResult = await CheckUserNameAndPasswordAsync(userName, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByNameAsync(userName);
                string token = await BuildTokenAsync(user!);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }

        private async Task<string> BuildTokenAsync(User user)
        {
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return tokenService.BuildToken(claims, optJWT.Value);
        }
    }
}
