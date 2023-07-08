using GameManagement.Api.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Administrator")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApiIdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(
            UserManager<ApiIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole()
        {
            return Ok();
        }
        [HttpPatch]
        public async Task<IActionResult> EditRole(Guid id)
        {
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            return NoContent();
        }
    }
}