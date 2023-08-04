using Commons;
using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IdentityService.Infrastructure
{
    internal class IdentityRepository : IIdentityRepository
    {
        private readonly IdentityUserManager userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<IdentityRepository> logger;

        public IdentityRepository(IdentityUserManager userManager, RoleManager<Role> roleManager, ILogger<IdentityRepository> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        public Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> FindByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User?> FindByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return this.userManager.CreateAsync(user, password);
        }

        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return userManager.AccessFailedAsync(user);
        }

        public async Task<SignInResult> ChangePhoneNumberAsync(Guid userId, string phoneNumber, string token)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ArgumentException($"{userId} 的用户不存在");
            }
            var changeResult = await this.userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
            if (!changeResult.Succeeded)
            {
                await this.userManager.AccessFailedAsync(user);
                string errMsg = changeResult.Errors.SumErrors();
                this.logger.LogWarning($"{phoneNumber} ChangePhoneNumberAsync失败，错误信息 {errMsg}");
                return SignInResult.Failed;
            }
            else
            {
                await ConfirmPhoneNumberAsync(user.Id);//确认手机号
                return SignInResult.Success;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string newPassword, string oldPassword)
        {
            if (newPassword.Length < 6)
            {
                IdentityError err = new()
                {
                    Code = "Password Invalid",
                    Description = "密码长度不能少于6"
                };
                return IdentityResult.Failed(err);
            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is not null)
            {
                if (userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, oldPassword) == PasswordVerificationResult.Failed)
                {
                    IdentityError err = new()
                    {
                        Description = "密码不正确"
                    };
                    return IdentityResult.Failed(err);
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetPwdResult = await userManager.ResetPasswordAsync(user, token, newPassword);
                return resetPwdResult;
            }

            return IdentityResult.Failed();
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
        {
            return this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                Role role = new() { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded == false)
                {
                    return result;
                }
            }
            return await userManager.AddToRoleAsync(user, roleName);
        }

        /// <summary>
        /// 尝试登录，如果lockoutOnFailure为true，则登录失败还会自动进行lockout计数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if (success)
            {
                return SignInResult.Success;
            }
            else
            {
                if (lockoutOnFailure)
                {
                    var r = await AccessFailedAsync(user);
                    if (!r.Succeeded)
                    {
                        throw new ApplicationException("AccessFailed failed");
                    }
                }
                return SignInResult.Failed;
            }
        }

        public async Task ConfirmPhoneNumberAsync(Guid id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumberConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        public async Task UpdatePhoneNumberAsync(Guid id, string phoneNumber)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumber = phoneNumber;
            await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            var userLoginStore = userManager.UserLoginStore;
            CancellationToken cancellationToken = default;
            //先删除aspnetuserlogins数据，再软删除User
            var logins = await userLoginStore.GetLoginsAsync(user, cancellationToken);
            foreach (var login in logins)
            {
                await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, cancellationToken);
            }
            user.SoftDelete();
            var result = await userManager.UpdateAsync(user);
            return result;
        }

        private static IdentityResult ErrorResult(string msg)
        {
            IdentityError idError = new() { Description = msg };
            return IdentityResult.Failed(idError);
        }

        public async Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNumber)
        {
            if (await FindByNameAsync(userName) != null)
            {
                return (ErrorResult($"已经存在用户名{userName}"), null, null);
            }
            if (await FindByPhoneNumberAsync(phoneNumber) != null)
            {
                return (ErrorResult($"已经存在手机号{phoneNumber}"), null, null);
            }
            User user = new(userName)
            {
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true
            };
            string password = GeneratePassword();
            var result = await CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (result, null, null);
            }
            result = await AddToRoleAsync(user, UserRoles.Administrator);
            return !result.Succeeded ? ((IdentityResult, User?, string? password))(result, null, null) : ((IdentityResult, User?, string? password))(IdentityResult.Success, user, password);
        }

        public async Task<(IdentityResult, User?, string? password)> ResetPasswordAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                return (ErrorResult("用户没找到"), null, null);
            }
            string password = GeneratePassword();
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, password);
            return !result.Succeeded ? ((IdentityResult, User?, string? password))(result, null, null) : ((IdentityResult, User?, string? password))(IdentityResult.Success, user, password);
        }

        private string GeneratePassword()
        {
            var options = userManager.Options.Password;
            int length = options.RequiredLength;
            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;
            StringBuilder password = new();
            Random random = new();
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));
            return password.ToString();
        }
    }
}